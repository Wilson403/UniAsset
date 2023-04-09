using System;
using System.Collections.Generic;

namespace UniAsset
{
    /// <summary>
    /// 资源下载队列
    /// </summary>
    public class GroupDownloader
    {
        private readonly int _maxDownloadNum = 30;
        private readonly Dictionary<string , BaseDownloader> _allDownloaderDict = new Dictionary<string , BaseDownloader> ();
        private readonly List<string> _downloadingList = new List<string> ();
        private readonly List<string> _removeList = new List<string> ();
        private bool _isUpdate = false;
        private bool _isLoadding = false;

        long _loadedSize = 0;
        /// <summary>
        /// 下载完成文件大小
        /// </summary>
        public long LoadedSize
        {
            get
            {
                return _loadedSize;
            }
        }

        long _totalSize = 0;
        /// <summary>
        /// 下载文件总大小
        /// </summary>
        public long TotalSize
        {
            get
            {
                return _totalSize;
            }
        }

        private string _error;
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

        bool _isDone;
        public bool IsDone
        {
            get
            {
                return _isDone;
            }
        }

        public GroupDownloader ()
        {
            UniAssetRuntime.Ins.onUpdate += Update;
        }

        /// <summary>
        /// 更新全部下载器
        /// </summary>
        public void Update ()
        {
            if ( !_isUpdate )
            {
                return;
            }

            _removeList.Clear ();

            foreach ( KeyValuePair<string , BaseDownloader> pair in _allDownloaderDict )
            {
                if ( _downloadingList.Count >= _maxDownloadNum )
                {
                    break;
                }

                if ( !_downloadingList.Contains (pair.Key) )
                {
                    _downloadingList.Add (pair.Key);
                }
            }

            foreach ( string key in _downloadingList )
            {
                BaseDownloader downloader = _allDownloaderDict [key];
                downloader.Update ();
                if ( downloader.IsDone () )
                {
                    if ( !string.IsNullOrEmpty (downloader.Error) )
                    {
                        _error = downloader.Error;
                    }
                    _removeList.Add (key);
                    _loadedSize += downloader.FileSize;
                }
            }

            //移除完成的下载器
            foreach ( string key in _removeList )
            {
                _allDownloaderDict.Remove (key);
                _downloadingList.Remove (key);
            }

            //不存在任何下载器，即下载完成了
            if ( _allDownloaderDict.Count == 0 )
            {
                _isUpdate = false;
                _isDone = true;
                UniAssetRuntime.Ins.LocalResVer.SaveResPackageVer ();
            }
        }

        /// <summary>
        /// 销毁全部下载器
        /// </summary>
        public void Destroy ()
        {
            foreach ( KeyValuePair<string , BaseDownloader> pair in _allDownloaderDict )
            {
                BaseDownloader downloader = pair.Value;
                downloader.Dispose ();
            }
            _allDownloaderDict.Clear ();
            _removeList.Clear ();
            UniAssetRuntime.Ins.onUpdate -= Update;
        }

        /// <summary>
        /// 添加到下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        /// <param name="version"></param>
        /// <param name="fileSize"></param>
        /// <param name="onLoaded"></param>
        /// <param name="data"></param>
        public void AddLoad (string url , string savePath , string version , long fileSize = 1 , Action<object> onLoaded = null , object data = null)
        {
            if ( _isLoadding )
            {
                return;
            }

            _totalSize += fileSize;
            if ( _allDownloaderDict.TryGetValue (url , out BaseDownloader downloader) )
            {
                return;
            }
            _allDownloaderDict.Add (url , new UnityDownloader (url , savePath , fileSize , version , ResVerifyLevel.HIGHT , onLoaded , data));
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        public void StartLoad ()
        {
            if ( _isLoadding )
            {
                return;
            }

            _isUpdate = true;
        }
    }
}