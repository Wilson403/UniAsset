using System;

namespace UniAsset
{
    /// <summary>
    /// 加载信息
    /// </summary>
    public struct LoadInfoVo
    {
        /// <summary>
        /// 加载对象URL
        /// </summary>
        public string url;

        /// <summary>
        /// 保存位置
        /// </summary>
        public string savePath;

        /// <summary>
        /// 文件版本号
        /// </summary>
        public string version;

        /// <summary>
        /// 加载完成的回调
        /// </summary>
        public Action<object> onLoaded;

        /// <summary>
        /// 加载完成回调携带的数据
        /// </summary>
        public object data;

        /// <summary>
        /// 加载文件的大小(bytes)
        /// </summary>
        public long fileSize;

        /// <summary>
        /// 加载失败的次数
        /// </summary>
        public int loadFailCount;

        public LoadInfoVo (string url , string savePath , string version , long fileSize , Action<object> onLoaded , object data)
        {
            this.url = url;
            this.savePath = savePath;
            this.version = version;
            this.fileSize = fileSize;
            this.onLoaded = onLoaded;
            this.data = data;
            this.loadFailCount = 0;
        }
    }
}