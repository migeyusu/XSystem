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

        //protected ConcurrentDictionary<string, bool> UrlCache;

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
        public event Action InnerEnd;

        /// <summary>
        /// 传入页面处理方法
        /// </summary>
        /// <param name="decodeAction">页面解析</param>
        /// <param name="downloadparallel">页面下载并行度</param>
        /// <param name="decodeparallel">页面解析并行度</param>
        /// <param name="urlCreateFunc">url初始化</param>
        public ReptileProcessor(Action<Page, ReptileContext> decodeAction, int downloadparallel,
            int decodeparallel,Func<IEnumerable<string>> urlCreateFunc = null)
        {
            _urlCreateFunc = urlCreateFunc;
            _decodeAction = decodeAction ?? throw new ArgumentNullException();
            this._downloadDegreeOfParallel = downloadparallel;
            this._decodeDegreeOfParallel = decodeparallel;
        }

        protected void InternalStart()
        {
            _work = true;
            Working = true;
            for (var i = 0; i < _downloadDegreeOfParallel; i++) {
                ThreadPool.QueueUserWorkItem(state => {
                    var stopwatch = new Stopwatch();
                    foreach (var s in UriBuffer.GetConsumingEnumerable()) {
                        //grab
                        try {
                            Interlocked.Increment(ref _grabbingThreadCount);
                            stopwatch.Start();
                            var webRequest = WebRequest.Create(s);
                            using (var webResponse = webRequest.GetResponse()) {
                                using (var responseStream = webResponse.GetResponseStream()) {
                                    using (var streamReader = new StreamReader(responseStream)) {
                                        PageBuffer.Add(new Page() {
                                            Url = s,
                                            Content = streamReader.ReadToEnd()
                                        });
                                    }
                                }
                            }
                        }
                        catch (Exception e) {
                            ErrorUrl.Add(new ErrorPage {Error = 1, Exception = e, Url = s});
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
                            _decodeAction(page, this.Context);
                        }
                        catch (Exception e)
                        {
                            ErrorUrl.Add(new ErrorPage() { Error = 2, Exception = e, Url = page.Url });
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
                            OnInnerEnd();
                            return;
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

        protected virtual void OnInnerEnd()
        {
            InnerEnd?.Invoke();
        }


        public void Dispose()
        {
            InternalStop();
            UriBuffer.Dispose();
            PageBuffer.Dispose();
        }
    }

    public class ReptileContext
    {
        /// <summary>
        /// 待抓取的url集合
        /// </summary>
        private BlockingCollection<string> _urls;

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

        public static void Save(IEnumerable<ErrorPage> errorPages)
        {
            
        }

    }
}