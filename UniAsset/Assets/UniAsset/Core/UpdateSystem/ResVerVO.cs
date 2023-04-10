namespace UniAsset
{
    /// <summary>
    /// 资源项
    /// </summary>
    public class ResVerItem
    {
        /// <summary>
        /// 资源名称
        /// </summary>
        public string name;

        /// <summary>
        /// 版本号
        /// </summary>
        public string version;

        /// <summary>
        /// 文件大小(字节为单位)
        /// </summary>
        public long size;
    }

    /// <summary>
    /// 资源版本号数据
    /// </summary>
    public class ResVerVo
    {
        /// <summary>
        /// 资源项列表
        /// </summary>
        public ResVerItem [] items;

        /// <summary>
        /// 资源包版本
        /// </summary>
        public string resPackageVer;
    }
}