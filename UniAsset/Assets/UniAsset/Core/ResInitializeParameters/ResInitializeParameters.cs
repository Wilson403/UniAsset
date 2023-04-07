using System.IO;

namespace UniAsset
{
    /// <summary>
    /// 资源加载模式
    /// </summary>
    public enum ResLoadMode
    {
        /// <summary>
        /// 使用AssetDataBase接口来加载资源，只能在编辑器下使用
        /// </summary>
        ASSET_DATA_BASE = 0,

        /// <summary>
        /// 本地加载AssetBundle
        /// </summary>
        LOCAL_ASSET_BUNDLE = 1,

        /// <summary>
        /// 联机模式下载AssetBundle加载
        /// </summary>
        REMOTE_ASSET_BUNDLE = 2
    }

    public abstract class ResInitializeParameters
    {
        public string assetRoot;

        /// <summary>
        /// 确保资源目录存在
        /// </summary>
        /// <param name="path"></param>
        protected void CertainPathExists (string path)
        {
            if ( false == Directory.Exists (path) )
            {
                Directory.CreateDirectory (path);
            }
        }
    }
}