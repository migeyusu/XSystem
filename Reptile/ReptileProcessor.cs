using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Castle.Core.Internal;
using Xunit;

namespace Reptile
{
    /* 爬虫以流水线形式实现，分为多个Stage：1.生成/取得 URL 2.获得页面 3.从页面分离数据 4.持久化
     * URL的生成支持两种方式：url creater和context.add，前者运行一次后获取指定数量的url，
     * 后者允许在处理阶段被写入新的url，并允许中断
     */

    public class ReptileProcessor : IDisposable
    {
        protected BlockingCollection<string> UriBuffer;

        protected BlockingCollection<Page> PageBuffer;

        protected ReptileContext Context;

        private readonly Func<IEnumerable<string>> _urlCreateFunc;

        private readonly Action<Page, ReptileContext> _decodeAction;

        private readonly int _downloadDegreeOfParallel;

        private readonly int _decodeDegreeOfParallel;

        private volatile bool _work;

        /// <summary>
        /// 计数线程的运行状态
        /// </summary>
        private int _decodingThreadCount = 0, _grabbingThreadCount = 0;

        private long _fetchSpentTime;
        private long _decodeSpentTime;

        public bool Working { get; private set; }

        public ConcurrentBag<ErrorPage> ErrorUrl { get; set; } = new ConcurrentBag<ErrorPage>();

        /// <summary>
        /// 网页下载时间
        /// </summary>
        public long FetchSpentTime {
            get { return _fetchSpentTime; }
            set { _fetchSpentTime = value; }
        }

        /// <summary>
        /// 页面解析时间
        /// </summary>
        public long DecodeSpentTime {
            get { return _decodeSpentTime; }
            set { _decodeSpentTime = value; }
        }

        /// <summary>   
        /// 所有缓冲区都已排空且未进行抓取
        /// </summary>
        public event Action PipeEmptied;

        /// <summary>
        /// 传入页面处理方法
        /// </summary>
        /// <param name="decodeAction">页面解析</param>
        /// <param name="downloadParallel">页面下载并行度</param>
        /// <param name="decodeParallel">页面解析并行度</param>
        /// <param name="urlCreateFunc">url初始化</param>
        public ReptileProcessor(Action<Page, ReptileContext> decodeAction, int downloadParallel,
            int decodeParallel, Func<IEnumerable<string>> urlCreateFunc = null)
        {
            _urlCreateFunc = urlCreateFunc;
            _decodeAction = decodeAction ?? throw new ArgumentNullException();
            _downloadDegreeOfParallel = downloadParallel;
            _decodeDegreeOfParallel = decodeParallel;
        }

        protected void InternalStart()
        {
            _work = true;
            Working = true;
            for (var i = 0; i < _downloadDegreeOfParallel; i++) {
                ThreadPool.QueueUserWorkItem(state => {
                    CookieContainer container = null;
                    var stopwatch = new Stopwatch();
                    foreach (var url in UriBuffer.GetConsumingEnumerable()) {
                        //grab
                        try {
                            Interlocked.Increment(ref _grabbingThreadCount);
                            stopwatch.Start();
                            //using (var webClient = new WebClient()) {
                            //    webClient.Headers[HttpRequestHeader.UserAgent] =
                            //        "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                            //    var downloadString = webClient.DownloadString(url);
                            //    PageBuffer.Add(new Page() {
                            //        Url = url,
                            //        Content = downloadString
                            //    });
                            //}
                            var webRequest = (HttpWebRequest) WebRequest.Create(url);
                            if (container != null) {
                                webRequest.CookieContainer = container;
                            }
                            webRequest.UserAgent =
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.108 Safari/537.36";
                            webRequest.KeepAlive = true;
                            using (var webResponse = (HttpWebResponse) webRequest.GetResponse()) {
                                if (container == null) {
                                    container = new CookieContainer();
                                    foreach (Cookie responseCookie in webResponse.Cookies) {
                                        container.Add(responseCookie);
                                    }
                                }
                                using (var responseStream = webResponse.GetResponseStream()) {
                                    using (var streamReader = new StreamReader(responseStream)) {
                                        PageBuffer.Add(new Page() {
                                            Url = url,
                                            Content = streamReader.ReadToEnd()
                                        });
                                    }
                                }
                            }
                        }
                        catch (Exception e) {
                            ErrorUrl.Add(new ErrorPage {Error = 1, Exception = e, Url = url});
                        }
                        finally {
                            stopwatch.Stop();
                            Interlocked.Decrement(ref _grabbingThreadCount);
                        }
                    }
                    Interlocked.Add(ref _fetchSpentTime, stopwatch.ElapsedMilliseconds);
                });
            }
            for (var i = 0; i < _decodeDegreeOfParallel; i++) {
                ThreadPool.QueueUserWorkItem(state => {
                    var stopwatch = new Stopwatch();
                    foreach (var page in PageBuffer.GetConsumingEnumerable()) {
                        try {
                            Interlocked.Increment(ref _decodingThreadCount);
                            stopwatch.Start();
                            _decodeAction(page, Context);
                        }
                        catch (Exception e) {
                            ErrorUrl.Add(new ErrorPage() {Error = 2, Exception = e, Url = page.Url});
                        }
                        finally {
                            stopwatch.Stop();
                            Interlocked.Decrement(ref _decodingThreadCount);
                        }
                    }
                    Interlocked.Add(ref _decodeSpentTime, stopwatch.ElapsedMilliseconds);
                });
            }
            //监控线程，当所有缓冲区都为空并且页面解析不在进行中，触发事件
            Task.Run(() => {
                Thread.Sleep(500);
                while (_work) {
                    Thread.Sleep(300);
                    if (UriBuffer.Count == 0 && PageBuffer.Count == 0 &&
                        Interlocked.CompareExchange(ref _grabbingThreadCount, 0, 0) == 0 &&
                        Interlocked.CompareExchange(ref _decodingThreadCount, 0, 0) == 0) {
                        Thread.Sleep(100);
                        if (UriBuffer.Count == 0 && PageBuffer.Count == 0 &&
                            Interlocked.CompareExchange(ref _grabbingThreadCount, 0, 0) == 0 &&
                            Interlocked.CompareExchange(ref _decodingThreadCount, 0, 0) == 0) {
                            OnPipeEmpty();
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 开始抓取
        /// </summary>
        public void Start(string seed = null)
        {
            UriBuffer = _urlCreateFunc != null
                ? new BlockingCollection<string>(new ConcurrentBag<string>(_urlCreateFunc()))
                : new BlockingCollection<string>();
            PageBuffer = new BlockingCollection<Page>();
            Context = new ReptileContext(UriBuffer);
            if (seed != null) {
                UriBuffer.Add(seed);
            }
            InternalStart();
        }

        public void Stop()
        {
            InternalStop();
        }

        protected void InternalStop()
        {
            if (!Working) {
                return;
            }
            _work = false;
            Working = false;
            UriBuffer.CompleteAdding();
            PageBuffer.CompleteAdding();
        }

        protected virtual void OnPipeEmpty()
        {
            PipeEmptied?.Invoke();
        }


        public void Dispose()
        {
            InternalStop();
            UriBuffer.Dispose();
            PageBuffer.Dispose();
            ErrorUrl.Clear();
        }
    }

    public class ReptileContext
    {
        /// <summary>
        /// 待抓取的url集合
        /// </summary>
        private readonly BlockingCollection<string> _urls;

        public ReptileContext(BlockingCollection<string> urls)
        {
            _urls = urls;
        }

        public void AddUrl(string url)
        {
            _urls.Add(url);
        }

        ///// <summary>
        /////指示是否退出
        ///// </summary>
        //public CancellationToken CancellationToken { get; set; }
    }

    [Serializable]
    public struct ErrorPage
    {
        public string Url { get; set; }

        public Exception Exception { get; set; }

        /// <summary>
        /// 1:网页无法获取，2:解析出现错误
        /// </summary>
        public byte Error { get; set; }
    }
}