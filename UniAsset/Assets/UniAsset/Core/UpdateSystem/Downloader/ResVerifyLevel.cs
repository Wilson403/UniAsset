namespace UniAsset
{
    /// <summary>
    /// 资源校验级别
    /// </summary>
    public enum ResVerifyLevel
    {
        /// <summary>
        /// 文件存在即通过
        /// </summary>
        LOW = 0,

        /// <summary>
        /// 文件存在，且校验CRC32值
        /// </summary>
        HIGHT = 1
    }
}