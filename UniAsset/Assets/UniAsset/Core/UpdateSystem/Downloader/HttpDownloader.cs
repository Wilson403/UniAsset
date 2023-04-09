using System;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;

namespace UniAsset
{
    /// <summary>
    /// 资源下载器
    /// </summary>
    public class Downloader
    {
        class HttpDownloader
        {
            /// <summary>
            /// 下载取消的错误
            /// </summary>
            public const string ERROR_CANCEL = "ERROR_CANCEL";

            /// <summary>
            /// 下载缓冲区大小
            /// </summary>
            public const int BUFFER_SIZE = 65536;

            /// <summary>
            /// 下载过程中
            /// </summary>
            public event Action<HttpDownloader , long , long> onProgress;

            /// <summary>
            /// 下载完成
            /// </summary>
            public event Action<HttpDownloader , string> onComplete;

            /// <summary>
            /// 下载的Url
            /// </summary>
            public string Url { get; private set; }

            /// <summary>
            /// 保存地址
            /// </summary>
            public string SavePath { get; private set; }

            /// <summary>
            /// 已下载大小
            /// </summary>
            public long LoadedSize { get; private set; } = 0;

            /// <summary>
            /// 总大小
            /// </summary>
            public long TotalSize { get; private set; } = 0;

            /// <summary>
            /// 错误信息
            /// </summary>
            public string Error { get; private set; } = null;

            /// <summary>
            /// 是否正在下载中
            /// </summary>
            public bool IsDownloading
            {
                get
                {
                    return _thread != null ? true : false;
                }
            }

            Thread _thread;
            FileStream _fs;

            public HttpDownloader (string url , string savePath)
            {
                Url = url;
                SavePath = savePath;
            }

            public void Download ()
            {
                if ( null == _thread )
                {
                    _thread = new Thread (new ThreadStart (DownloadProcess));
                    _thread.Start ();
                }
                else
                {
                    throw new Exception ("Is Downloading");
                }
            }

            public void Cancel ()
            {
                onProgress = null;
                onComplete = null;
                _thread = null;
                if ( _fs != null )
                {
                    _fs.Close ();
                }
            }

            void DownloadProcess ()
            {
                try
                {
                    // 设置参数
                    HttpWebRequest request = WebRequest.Create (Url) as HttpWebRequest;

                    //发送请求并获取相应回应数据
                    HttpWebResponse response = request.GetResponse () as HttpWebResponse;

                    LoadedSize = 0;
                    TotalSize = response.ContentLength;

                    //直到request.GetResponse()程序才开始向目标网页发送Post请求
                    Stream rs = response.GetResponseStream ();

                    //创建本地文件写入流
                    _fs = new FileStream (SavePath , FileMode.Create , FileAccess.Write , FileShare.Write);
                    byte [] buffer = new byte [BUFFER_SIZE];

                    while ( _thread != null )
                    {
                        int size = rs.Read (buffer , 0 , buffer.Length);
                        if ( size > 0 )
                        {
                            LoadedSize += size;
                            _fs.Write (buffer , 0 , size);
                            onProgress?.Invoke (this , LoadedSize , TotalSize);
                            Thread.Sleep (1);
                        }
                        else
                        {
                            break;
                        }
                    }

                    _fs.Close ();
                    rs.Close ();

                    if ( null == _thread )
                    {
                        Error = ERROR_CANCEL;
                    }
                    else
                    {
                        Error = null;
                        _thread = null;
                    }
                }
                catch ( Exception e )
                {
                    Error = e.Message;
                }
                finally
                {
                    onComplete?.Invoke (this , Error);
                }
            }
        }

        /// <summary>
        /// 下载连接数限制
        /// PS:修改该值可以直接简单的限制HTTP下载请求的并发数
        /// </summary>
        public static int downloadConnectionLimit = 65500;

        HttpDownloader _httpDownloader;

        bool _isDone;

        /// <summary>
        /// 是否操作完成
        /// </summary>
        public bool IsDone
        {
            get
            {
                if ( false == _isDone )
                {
                    CheckTimeout ();
                }
                return _isDone;
            }
        }

        float _progress;

        /// <summary>
        /// 操作进度
        /// </summary>
        public float Progress
        {
            get
            {
                return _progress;
            }
        }

        string _error;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error
        {
            get
            {
                return _error;
            }
        }

        long _totalSize;

        /// <summary>
        /// 文件总大小
        /// </summary>
        public long TotalSize
        {
            get
            {
                return _totalSize;
            }
        }

        long _loadedSize;

        /// <summary>
        /// 已完成大小
        /// </summary>
        public long LoadedSize
        {
            get
            {
                return _loadedSize;
            }
        }

        private float _downloadSpeed;
        /// <summary>
        /// 下载速度
        /// </summary>
        public float DownloadSpeed
        {
            get
            {
                int seconds = DateTime.Now.Subtract (_firstDownloadDT).Seconds;
                if ( seconds != 0 )
                {
                    _downloadSpeed = LoadedSize / 1024 / seconds;
                }
                return _downloadSpeed;
            }
        }

        /// <summary>
        /// 是否已销毁
        /// </summary>
        public bool IsDisposeed
        {
            get { return _httpDownloader == null ? true : false; }
        }

        string _version;
        string _savePath;
        string _url;
        ResVerifyLevel _resVerifyLevel;

        /// <summary>
        /// 下载的URL地址
        /// </summary>
        public string Url
        {
            get { return _url; }
        }

        /// <summary>
        /// 文件的保存路径
        /// </summary>
        public string SavePath
        {
            get { return _savePath; }
        }

        /// <summary>
        /// 下载超时的设置，当指定毫秒内下载进度没有改变时，视为下载超时。
        /// </summary>
        public int timeout = 15000;

        /// <summary>
        /// 首次下载时间
        /// </summary>
        readonly DateTime _firstDownloadDT;

        /// <summary>
        /// 最后进度改变的时间
        /// </summary>
        DateTime _lastProgressChangedDT;

        /// <summary>
        /// 初始化下载类
        /// </summary>
        /// <param name="url">下载文件的URL地址</param>
        /// <param name="savePath">保存文件的本地地址</param>
        /// <param name="version">URL对应文件的版本号</param>
        /// <param name="resVerifyLevel"></param>
        public Downloader (string url , string savePath , string version = null , ResVerifyLevel resVerifyLevel = ResVerifyLevel.LOW)
        {
            _url = url;
            _savePath = savePath;
            _resVerifyLevel = resVerifyLevel;
            _version = version;

            string saveDir = Path.GetDirectoryName (savePath);
            if ( Directory.Exists (saveDir) == false )
            {
                Directory.CreateDirectory (saveDir);
            }

            if ( null != version )
            {
                string flag;
                if ( url.Contains ("?") )
                {
                    flag = "&";
                }
                else
                {
                    flag = "?";
                }

                url += string.Format ("{0}unity_download_ver={1}" , flag , version);
            }

            try
            {
                Uri uri = new Uri (url);
                ServicePoint serverPoint = ServicePointManager.FindServicePoint (uri);
                serverPoint.ConnectionLimit = downloadConnectionLimit;
                serverPoint.Expect100Continue = false;
                _progress = 0;
                _firstDownloadDT = DateTime.Now;
                _lastProgressChangedDT = DateTime.Now;
                _httpDownloader = new HttpDownloader (url , savePath);
                _httpDownloader.onProgress += OnDownloadProgress;
                _httpDownloader.onComplete += OnDownloadComplete;
                _httpDownloader.Download ();
            }
            catch ( Exception ex )
            {
                _isDone = true;
                _error = ex.Message;
            }
        }

        private void OnDownloadComplete (HttpDownloader loader , string error)
        {
            if ( error != null )
            {
                SetError (error);
            }
            else if ( _loadedSize < _totalSize )
            {
                SetError ($"文件大小不一致: {_loadedSize} / {_totalSize}");
            }
            _isDone = true;
        }

        private void OnDownloadProgress (HttpDownloader loader , long loaded , long total)
        {
            _lastProgressChangedDT = DateTime.Now;
            _loadedSize = loader.LoadedSize;
            _totalSize = total;
            if ( 0 == _totalSize )
            {
                _progress = 0;
            }
            else
            {
                _progress = _loadedSize / ( float ) _totalSize;
            }
        }

        /// <summary>
        /// 销毁对象，会停止所有的下载
        /// </summary>
        public void Dispose ()
        {
            if ( _httpDownloader != null )
            {
                _httpDownloader.onProgress -= OnDownloadProgress;
                _httpDownloader.onComplete -= OnDownloadComplete;
                _httpDownloader.Cancel ();
                _httpDownloader = null;
                if ( false == _isDone )
                {
                    SetError ("Canceled");
                    _isDone = true;
                }
            }
        }

        /// <summary>
        /// 检查是否超时
        /// </summary>
        void CheckTimeout ()
        {
            TimeSpan ts = DateTime.Now - _lastProgressChangedDT;
            if ( ts.TotalMilliseconds > timeout )
            {
                SetError ("TimeOut");
            }
        }

        void SetError (string error)
        {
            Dispose ();

            //删除文件
            if ( File.Exists (SavePath) )
            {
                File.Delete (SavePath);
            }
            _error = error;
            Debug.LogErrorFormat ("下载失败 [{0}] ：{1}" , _url , error);
        }
    }
}