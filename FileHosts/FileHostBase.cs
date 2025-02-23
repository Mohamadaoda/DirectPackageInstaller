﻿using System.Collections.Generic;
using System.Net;
using System.Web;
using static DirectPackageInstaller.Program;

namespace DirectPackageInstaller.FileHosts
{
    public abstract class FileHostBase
    {
        string HostName { get; }

        public abstract bool IsValidUrl(string URL);
        public abstract DownloadInfo GetDownloadInfo(string URL);

        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.93 Safari/537.36 Edg/96.0.1054.53";

        protected string DownloadString(string URL)
        {
            lock (HttpClient)
            {
                HttpClient.Headers.Clear();
                HttpClient.Headers["user-agent"] = UserAgent;
                HttpClient.Headers["referer"] = HttpUtility.UrlEncode(URL);

                return HttpClient.DownloadString(URL);
            }
        }

        protected string PostString(string URL, string ContentType, string Data)
        {
            lock (HttpClient)
            {
                HttpClient.Headers.Clear();
                HttpClient.Headers["content-type"] = ContentType;
                HttpClient.Headers["user-agent"] = UserAgent;
                HttpClient.Headers["referer"] = HttpUtility.UrlEncode(URL);
                
                return HttpClient.UploadString(URL, Data);
            }
        }

        protected (byte[] Data, WebHeaderCollection Headers) DownloadRequest(string URL)
        {
            lock (HttpClient)
            {
                HttpClient.Headers.Clear();
                HttpClient.Headers["user-agent"] = UserAgent;
                HttpClient.Headers["referer"] = HttpUtility.UrlEncode(URL);

                return (HttpClient.DownloadData(URL), HttpClient.ResponseHeaders);
            }
        }

        public static FileHostBase[] Hosts => new FileHostBase[] {
                new ZippyShare(), new Mediafire(), new GoogleDrive(), 
                new PixelDrain(), new OneFichier()
        };
    }

    public struct DownloadInfo {
        public string Url;
        public List<(string Key, string Value)> Headers;
        public Cookie[] Cookies;
        public WebProxy Proxy;

        public bool SingleConnection;
    }
}
