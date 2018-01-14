﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reptile
{
    public class HttpRequest
    {
        private readonly ConcurrentDictionary<string, IPAddress> _dnsCache;
        private const int IpPort = 80;
        private const int BufferSize = 16384;
        public int WaitTime { get; set; }

        public HttpRequest()
        {
            WaitTime = 3400;
            _dnsCache = new ConcurrentDictionary<string, IPAddress>();
        }

        public int ReceiveLength { get; set; }

        private string Request(string url, string host)
        {
            if (url.Contains("http://"))
                url = url.Substring(7, url.Length - 7);
            var head = new StringBuilder();
            head.Append("GET ");
            var position = url.IndexOf('/');
            head.Append(url.Substring(position, url.Length - position));
            head.Append(" HTTP/1.1");
            head.Append("\r\n");
            head.Append("Host:");
            head.Append(host);
            head.Append("\r\n");
            head.Append("\r\n");
            return head.ToString();
        }

        /// <summary>
        /// 同步
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        //public string GetHtml(string url)
        //{
        //    if (url.Contains("https://")) {
        //        url = url.Substring(8, url.Length - 8);
        //    }
        //    else if (url.Contains("http://")) {
        //        url = url.Substring(7, url.Length - 7);
        //    }
        //    else {
        //        throw new ArgumentException("未识别的URL");
        //    }
        //    string page = null;
        //    using (var ms = new MemoryStream()) {
        //        var position = url.IndexOf('/') + 1;
        //        var client = new TcpClient();
        //        var host = url.Substring(0, position - 1);
        //        var accomplished = false;
        //        NetworkStream stream = null;
        //        var read = true;
        //        Task.Run(() => {
        //            try
        //            {
        //                if (!_dnsCache.TryGetValue(host, out IPAddress value))
        //                {
        //                    value = Dns.GetHostAddresses(host)[0];
        //                    _dnsCache.TryAdd(host, value);
        //                }
        //                client.Connect(new IPEndPoint(value, IpPort));
        //                stream = client.GetStream();
        //                var buffer = new byte[BufferSize];
        //                var head = Encoding.UTF8.GetBytes(Request(url, host));
        //                stream.Write(head, 0, head.Length);
        //                while (read)
        //                {
        //                    position = stream.Read(buffer, 0, buffer.Length);
        //                    ms.Write(buffer, 0, position);
        //                    if (Encoding.UTF8.GetString(ms.ToArray()).Contains("</html>"))
        //                    {
        //                        read = false;
        //                    }
        //                }
        //                page = Encoding.UTF8.GetString(ms.ToArray());
        //                ReceiveLength = ms.ToArray().Length;
        //                stream.Close();
        //                client.Close();
        //                if (!page.Contains("</html>"))
        //                {
        //                    page = null;
        //                }
        //                accomplished = true;
        //            }
        //            catch (Exception)
        //            {
        //                read = false;
        //                accomplished = true;
        //                page = null;
        //            }

        //        });
        //        var i = 0;
        //        while (i < 3000)
        //        {
        //            Thread.Sleep(300);
        //            i += 300;
        //            if (accomplished)
        //            {
        //                break;
        //            }
        //        }
        //        if (!read) return page;
        //        stream?.Close();
        //        client.Close();
        //    }
        //    return page;
        //}

        public string GetHtml(string url)
        {
            if (url.Contains("https://"))
                url = url.Substring(8, url.Length - 8);
            else if (url.Contains("http://"))
                url = url.Substring(7, url.Length - 7);
            else
                throw new ArgumentException("未识别的URL");
            string page = null;
            using (var memoryStream = new MemoryStream())
            {
                var position = url.IndexOf('/') + 1;
                var client = new TcpClient();
                var host = url.Substring(0, position - 1);
                var accomplished = false;
                NetworkStream stream = null;
                var read = true;
                Task.Run(() =>
                {
                    try
                    {
                        if (!_dnsCache.TryGetValue(host, out IPAddress value))
                        {
                            value = Dns.GetHostAddresses(host)[0];
                            _dnsCache.TryAdd(host, value);
                        }
                        client.Connect(new IPEndPoint(value, IpPort));
                        stream = client.GetStream();
                        var buffer = new byte[BufferSize];
                        var head = Encoding.UTF8.GetBytes(Request(url, host));
                        stream.Write(head, 0, head.Length);
                        while (read)
                        {
                            position = stream.Read(buffer, 0, buffer.Length);
                            memoryStream.Write(buffer, 0, position);
                            if (Encoding.UTF8.GetString(memoryStream.ToArray()).Contains("</html>"))
                            {
                                read = false;
                            }
                        }
                        page = Encoding.UTF8.GetString(memoryStream.ToArray());
                        ReceiveLength = memoryStream.ToArray().Length;
                        stream.Close();
                        client.Close();
                        if (!page.Contains("</html>"))
                        {
                            page = null;
                        }
                        accomplished = true;
                    }
                    catch (Exception)
                    {
                        read = false;
                        accomplished = true;
                        page = null;
                    }

                });
                var i = 0;
                while (i < 3000)
                {
                    Thread.Sleep(300);
                    i += 300;
                    if (accomplished)
                    {
                        break;
                    }
                }
                if (!read) return page;
                stream?.Close();
                client.Close();
            }
            return page;
        }

    }
}
