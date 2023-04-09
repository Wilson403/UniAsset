using System.Collections;
using System.IO;
using Primise4CSharp;
using UnityEngine;

namespace UniAsset
{
    /// <summary>
    /// AssetBundle资源的Manifest描述文件，该文件很重要，描述了AssetBundle资源之间的依赖关系
    /// </summary>
    public class ManifestFileUpdate
    {
        private readonly string _netPath;
        private readonly string _manifestName;
        private readonly bool _isFirstVer;
        private readonly Promise _promise;

        public ManifestFileUpdate (bool isFirstVer)
        {
            _promise = new Promise ();
            _isFirstVer = isFirstVer;
            _manifestName = UniAssetConst.AssetBundleManifestName;
            string netPath = ( UniAssetRuntime.Ins.ResInitializeParameters as OnlineInitializeParameters ).netResDir;
            string ver = isFirstVer ? UniAssetRuntime.Ins.Setting.GetFirstNetResPackageVer () : UniAssetRuntime.Ins.Setting.GetCurrentNetResPackageVer ();
            _netPath = Path.Combine (netPath , UtilResVersionCompare.Ins.GetAppMainVersion () , ver);
        }

        public Promise Start (bool isNeedUpdate = true)
        {
            Debug.Log ("「ManifestFileUpdate」Manifest描述文件更新检查...");
            if ( isNeedUpdate && !UniAssetRuntime.Ins.GetResVerModel (_isFirstVer).IsSameVer (_manifestName , UniAssetRuntime.Ins.LocalResVer) )
            {
                Debug.Log ("「ManifestFileUpdate」Manifest正在更新...");
                UniAssetRuntime.Ins.StartCoroutine (Update (Path.Combine (_netPath , _manifestName) , UniAssetRuntime.Ins.GetResVerModel (_isFirstVer).GetVer (_manifestName)));
            }
            else
            {
                Debug.Log ("「ManifestFileUpdate」Manifest无需更新");
                InitResMgr ();
            }
            return _promise;
        }

        private void InitResMgr ()
        {
            ResMgr.Ins.Init ();
            _promise.Resolve ();
        }

        private IEnumerator Update (string url , string ver)
        {
            Downloader loader = new Downloader (url , FileSystem.CombinePaths (UniAssetRuntime.Ins.ResInitializeParameters.AssetRoot , _manifestName) , ver , ResVerifyLevel.HIGHT);
            while ( false == loader.IsDone )
            {
                yield return new WaitForEndOfFrame ();
            }

            if ( null != loader.Error )
            {
                _promise.Reject (new System.Exception (loader.Error));
                yield break;
            }

            loader.Dispose ();
            UniAssetRuntime.Ins.LocalResVer.SetVerAndSave (_manifestName , ver);
            InitResMgr ();
            yield break;
        }
    }
}