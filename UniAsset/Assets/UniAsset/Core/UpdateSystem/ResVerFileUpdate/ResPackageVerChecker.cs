using Primise4CSharp;

namespace UniAsset
{
    /// <summary>
    /// 检查资源包版本，由此判断要下载那个版本的资源
    /// </summary>
    public class ResPackageVerChecker
    {
        /// <summary>
        /// 总是更新资源
        /// </summary>
        const bool ALWAYS_UPDATE_RES = false;

        public Promise<bool> Start ()
        {
            Promise<bool> promise = new Promise<bool> ();
            int result = UtilResVersionCompare.Ins.CompareAppResVersion ();
            promise.Resolve (result != 0 || ALWAYS_UPDATE_RES);
            return promise;
        }
    }
}