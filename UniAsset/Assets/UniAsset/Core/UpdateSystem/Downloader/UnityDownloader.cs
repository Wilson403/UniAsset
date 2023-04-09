using System;
using UnityEngine;
using UnityEngine.Networking;

namespace UniAsset
{
    /// <summary>
    /// 资源下载器
    /// </summary>
    public class UnityDownloader : BaseDownloader
    {
        private UnityWebRequest _webRequest = null;

        public UnityDownloader (string url , string savePath , long size , string version = null , ResVerifyLevel resVerifyLevel = ResVerifyLevel.LOW , Action<object> onLoaded = null , object data = null)
            : base (url , savePath , size , version , resVerifyLevel , onLoaded , data)
        {
            
        }

        protected override void Start ()
        {
            _webRequest = new UnityWebRequest (this.url , UnityWebRequest.kHttpVerbGET);
            latestDownloadRealtime = Time.realtimeSinceStartup;
        }

        public override void Update ()
        {
            base.Update ();

            if ( IsDone () )
            {
                return;
            }

            if ( steps == DownloadState.NONE )
            {
                DownloadHandlerFile handler = new DownloadHandlerFile (savePath);
                handler.removeFileOnAbort = true;
                _webRequest.downloadHandler = handler;
                _webRequest.disposeDownloadHandlerOnDispose = true;
                _webRequest.SendWebRequest ();
                steps = DownloadState.CHECK_DOWNLOAD;
            }

            if ( steps == DownloadState.CHECK_DOWNLOAD )
            {
                downloadProgress = _webRequest.downloadProgress;
                downloadedBytes = _webRequest.downloadedBytes;

                //下载未完成前不继续往下执行，同时检查是否下载超时
                if ( !_webRequest.isDone )
                {
                    CheckTimeout ();
                    return;
                }

                //发生了错误，中止
                if ( _webRequest.isNetworkError || _webRequest.isHttpError )
                {
                    SetError (_webRequest.error);
                    return;
                }

                //检测文件的完整性
                long fileSize = FileSystem.GetFileSize (savePath);
                if ( size != fileSize )
                {
                    SetError ($"文件大小不一致: {fileSize} / {size}");
                    return;
                }

                error = string.Empty;
                steps = DownloadState.DONE;
                onLoaded?.Invoke (loadedData);

                //一切都没问题后释放下载器
                Dispose ();
            }
        }

        public override void Dispose ()
        {
            if ( _webRequest != null )
            {
                _webRequest.Abort ();
                _webRequest.Dispose ();
                _webRequest = null;
            }
        }
    }
}