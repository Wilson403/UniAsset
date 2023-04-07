namespace UniAsset
{
    /// <summary>
    /// 资源加载模式
    /// </summary>
    public enum ResLoadMode
    {
        /// <summary>
        /// 编辑器模式，可直接加载无需打包成AB
        /// </summary>
        EDITOR = 0,

        /// <summary>
        /// 离线模式加载本地AB包
        /// </summary>
        OFF_LINE = 1,

        /// <summary>
        /// 联机模式下载AB包加载
        /// </summary>
        ON_LINE = 2
    }

    public abstract class ResInitializeParameters
    {
        public string assetRoot;
    }
}