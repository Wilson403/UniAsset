using System.Text;
using Primise4CSharp;
using UnityEngine;

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

            #region 调试日志
            StringBuilder sb = new StringBuilder ();
            sb.Append ("资源包版本的检查结果如下: \n");
            sb.Append ($"本地资源号: {UniAssetRuntime.Ins.LocalResVer.VO.resPackageVer}\n");
            sb.Append ($"是否执行资源更新 ：{result != 0 || ALWAYS_UPDATE_RES}");
            Debug.Log (sb.ToString ());
            #endregion

            return promise;
        }
    }
}