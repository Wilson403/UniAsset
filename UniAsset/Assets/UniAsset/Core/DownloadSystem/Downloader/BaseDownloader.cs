using System;
using System.IO;
using UnityEngine;

namespace UniAsset
{
    public abstract class BaseDownloader
    {
        private bool _isAbort = false;
        private bool _isStart = false;

        protected string error;
        protected ulong latestDownloadBytes;
        protected float latestDownloadRealtime;
        protected float downloadProgress = 0f;
        protected ulong downloadedBytes = 0;
        protected DownloadState steps;
        protected int timeout = 15;

        protected readonly string url;
        protected readonly long size;
        protected readonly string savePath;
        protected readonly string version;
        protected readonly ResVerifyLevel resVerifyLevel;
        protected readonly Action<object> onLoaded;
        protected readonly object loadedData;

        /// <summary>
        /// 初始化下载类
        /// </summary>
        /// <param name="url">下载文件的URL地址</param>
        /// <param name="savePath">保存文件的本地地址</param>
        /// <param name="size">文件大小</param>
        /// <param name="version">URL对应文件的版本号</param>
        /// <param name="resVerifyLevel"></param>
        /// <param name="onLoaded"></param>
        /// <param name="data"></param>
        public BaseDownloader (
            string url ,
            string savePath ,
            long size ,
            string version = null ,
            ResVerifyLevel resVerifyLevel = ResVerifyLevel.LOW ,
            Action<object> onLoaded = null ,
            object data = null)
        {
            this.url = url;
            this.savePath = savePath;
            this.size = size;
            this.version = version;
            this.resVerifyLevel = resVerifyLevel;
            this.onLoaded = onLoaded;
            this.loadedData = data;
            steps = DownloadState.NONE;
        }

        /// <summary>
        /// 下载进度
        /// </summary>
        public float DownloadProgress
        {
            get { return downloadProgress; }
        }

        /// <summary>
        /// 已经下载的总字节数
        /// </summary>
        public ulong DownloadedBytes
        {
            get { return downloadedBytes; }
        }

        /// <summary>
        /// 文件尺寸
        /// </summary>
        public long FileSize
        {
            get { return size; }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error
        {
            get { return error; }
        }

        protected void CheckTimeout ()
        {
            // 注意：在连续时间段内无新增下载数据及判定为超时
            if ( _isAbort == false )
            {
                if ( latestDownloadBytes != DownloadedBytes )
                {
                    latestDownloadBytes = DownloadedBytes;
                    latestDownloadRealtime = Time.realtimeSinceStartup;
                }

                float offset = Time.realtimeSinceStartup - latestDownloadRealtime;
                if ( offset > timeout )
                {
                    SetError ("Web file request timeout");
                    _isAbort = true;
                }
            }
        }

        protected void SetError (string error)
        {
            Dispose ();

            //删除文件
            if ( File.Exists (savePath) )
            {
                File.Delete (savePath);
            }
            this.error = error;
            steps = DownloadState.FAILED;
            Debug.LogErrorFormat ("下载失败 [{0}] ：{1}" , url , error);
        }

        /// <summary>
        /// 是否完成：无论成功还是失败都算完成了
        /// </summary>
        /// <returns></returns>
        public bool IsDone ()
        {
            return steps == DownloadState.DONE || steps == DownloadState.FAILED;
        }

        public virtual void Update () 
        {
            if ( !_isStart ) 
            {
                Start ();
                _isStart = true;
            }
        }

        protected abstract void Start ();
        public abstract void Dispose ();
    }
}