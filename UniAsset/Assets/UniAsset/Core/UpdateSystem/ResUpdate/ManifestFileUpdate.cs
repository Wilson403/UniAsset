using System.Collections;
using Primise4CSharp;
using UnityEngine;

namespace UniAsset
{
    /// <summary>
    /// AssetBundle资源的Manifest描述文件，该文件很重要，描述了AssetBundle资源之间的依赖关系
    /// </summary>
    public class ManifestFileUpdate
    {
        private readonly string _manifestPath;
        private readonly bool _isFirstVer;
        private readonly Promise _promise;

        public ManifestFileUpdate (bool isFirstVer)
        {
            _promise = new Promise ();
            _isFirstVer = isFirstVer;
            _manifestPath = FileSystem.CombinePaths (UniAssetConst.AB_DIR_NAME , UniAssetConst.AssetBundleManifestName);
        }

        public Promise Start (bool isNeedUpdate = true)
        {
            if ( isNeedUpdate && !UniAssetRuntime.Ins.GetResVerModel (_isFirstVer).IsSameVer (_manifestPath , UniAssetRuntime.Ins.LocalResVer) )
            {
                string url = FileSystem.CombinePaths (UniAssetRuntime.Ins.GetRootUrl (_isFirstVer) , _manifestPath);
                UniAssetRuntime.Ins.StartCoroutine (Update (url , UniAssetRuntime.Ins.GetResVerModel (_isFirstVer).GetVer (_manifestPath)));
                Debug.Log ($"[{url}] Manifest正在更新...");
            }
            else
            {
                Debug.Log ("Manifest无需更新");
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
            Downloader loader = new Downloader (url , FileSystem.CombinePaths (UniAssetRuntime.Ins.ResInitializeParameters.AssetRoot , _manifestPath) , ver , ResVerifyLevel.HIGHT);
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
            UniAssetRuntime.Ins.LocalResVer.SetVerAndSave (_manifestPath , ver);
            InitResMgr ();
            yield break;
        }
    }
}