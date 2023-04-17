using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniAsset
{
    public class AssetBundleResLoader : BaseResLoader
    {
        /// <summary>
        /// 资源描述
        /// </summary>
        readonly AssetBundleManifest _manifest;

        /// <summary>
        /// 已加载的AB字典
        /// </summary>
        Dictionary<string , AssetbundleInfo> _loadedAssetBundleDic;

        /// <summary>
        /// 已加载的Asset字典
        /// </summary>
        Dictionary<string , AssetInfo> _loadedAssetDic;

        public AssetBundleResLoader (string manifestFilePath)
        {
            try
            {
                UnloadAll ();
                _loadedAssetBundleDic = new Dictionary<string , AssetbundleInfo> ();
                _loadedAssetDic = new Dictionary<string , AssetInfo> ();
                RootDir = FileSystem.StandardizeBackslashSeparator (Path.GetDirectoryName (manifestFilePath));
                AssetBundle ab = AssetBundle.LoadFromFile (manifestFilePath);
                _manifest = ab.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
                if ( _manifest == null )
                {
                    throw new Exception (string.Format ("错误的 Manifest File: {0}" , manifestFilePath));
                }
                ab.Unload (false);
            }
            catch ( Exception e )
            {
                Debug.LogError (e);
            }
        }

        /// <summary>
        /// 让已加载的AB资源字典继承源资源管理器
        /// </summary>
        /// <param name="source"></param>
        internal void Inherit (AssetBundleResLoader source)
        {
            _loadedAssetBundleDic = source._loadedAssetBundleDic;
        }

        void MakeABNameNotEmpty (ref string abName)
        {
            if ( string.IsNullOrEmpty (abName) )
            {
                abName = UniAssetConst.ROOT_AB_FILE_NAME;
            }
        }

        public override string [] GetDepends (string abName)
        {
            MakeABNameNotEmpty (ref abName);
            abName = ABNameWithExtension (abName);
            string [] dependList = _manifest.GetAllDependencies (abName);
            return dependList;
        }

        private AssetInfo GetAssetInfo (string abName , string assetName)
        {
            string key = FileSystem.CombinePaths (abName , assetName);
            if ( !_loadedAssetDic.TryGetValue (key , out AssetInfo assetInfo) )
            {
                assetInfo = new AssetInfo ();
                if ( _loadedAssetBundleDic.ContainsKey (abName) )
                {
                    assetInfo.bundleInfo = _loadedAssetBundleDic [abName];
                }
                _loadedAssetDic [key] = assetInfo;
            }
            return assetInfo;
        }

        public override AssetInfo Load<T> (string abName , string assetName)
        {
            MakeABNameNotEmpty (ref abName);
            AssetInfo assetInfo = GetAssetInfo (abName , assetName);
            abName = ABNameWithExtension (abName);

            if ( assetInfo.RefCount == 0 )
            {
                AssetBundle ab = LoadAssetBundle (abName).assetBundle;
                T asset = ab.LoadAsset<T> (assetName);
                if ( null == asset )
                {
                    Debug.LogError ($"获取的资源不存在： AssetBundle: {abName}  Asset: {assetName}");
                }
                assetInfo.asset = asset;
            }
            assetInfo.AddRefCount ();
            return assetInfo;
        }

        public override void LoadAsync (string abName , string assetName , Action<AssetInfo> onLoaded , Action<float> onProgress = null)
        {
            UniAssetRuntime.Ins.StartCoroutine (LoadAsyncByCoroutine (abName , assetName , onLoaded , onProgress));
        }

        IEnumerator LoadAsyncByCoroutine (string abName , string assetName , Action<AssetInfo> onLoaded , Action<float> onProgress)
        {
            MakeABNameNotEmpty (ref abName);
            AssetInfo assetInfo = GetAssetInfo (abName , assetName);
            abName = ABNameWithExtension (abName);

            if ( assetInfo.RefCount == 0 )
            {
                AssetBundle ab = LoadAssetBundle (abName).assetBundle;
                AssetBundleRequest abr = ab.LoadAssetAsync<GameObject> (assetName);

                do
                {
                    if ( onProgress != null )
                    {
                        onProgress.Invoke (abr.progress);
                    }
                    yield return new WaitForEndOfFrame ();
                }
                while ( false == abr.isDone );
                assetInfo.asset = abr.asset;
            }

            assetInfo.AddRefCount ();
            onLoaded.Invoke (assetInfo);
        }

        public override void LoadScene (string abName , string assetName , LoadSceneMode loadSceneMode = LoadSceneMode.Single , Action complete = null , Action<float> process = null , Action<string> error = null)
        {
            MakeABNameNotEmpty (ref abName);
            abName = ABNameWithExtension (abName);
            AssetBundle ab = LoadAssetBundle (abName).assetBundle;
            string [] sceneNameArray = ab.GetAllScenePaths ();
            for ( int i = 0 ; i < sceneNameArray.Length ; i++ )
            {
                if ( sceneNameArray [i].Contains (assetName) )
                {
                    base.LoadScene (string.Empty , sceneNameArray [i] , loadSceneMode , complete , process , error);
                    break;
                }
            }
        }

        public override void Unload (string abName , bool isUnloadAllLoaded = false , bool isUnloadDepends = true)
        {
            MakeABNameNotEmpty (ref abName);
            abName = ABNameWithExtension (abName);
            if ( _loadedAssetBundleDic.ContainsKey (abName) )
            {
                AssetbundleInfo ab = _loadedAssetBundleDic [abName];
                _loadedAssetBundleDic.Remove (abName);
                ab.assetBundle.Unload (isUnloadAllLoaded);
                Debug.LogFormat ("释放AB：{0}" , abName);
                if ( isUnloadDepends )
                {
                    string [] dependList = _manifest.GetAllDependencies (abName);
                    foreach ( string depend in dependList )
                    {
                        if ( false == CheckDependencies (depend) )
                        {
                            Unload (depend , isUnloadAllLoaded , isUnloadDepends);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检查ab资源是否被已加载的资源依赖
        /// </summary>
        /// <param name="ab"></param>
        /// <param name="depend"></param>
        /// <returns></returns>
        bool CheckDependencies (string ab)
        {
            foreach ( KeyValuePair<string , AssetbundleInfo> loadedEntry in _loadedAssetBundleDic )
            {
                string [] entryDepends = _manifest.GetAllDependencies (loadedEntry.Key);
                foreach ( string entryDepend in entryDepends )
                {
                    if ( ab == entryDepend )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void UnloadAll (bool isUnloadAllLoaded = false)
        {
            if ( null != _loadedAssetBundleDic )
            {
                foreach ( AssetbundleInfo cached in _loadedAssetBundleDic.Values )
                {
                    cached.assetBundle.Unload (isUnloadAllLoaded);
                }
                _loadedAssetBundleDic.Clear ();
            }

            ResMgr.Ins.DoGC ();
        }

        /// <summary>
        /// 加载AB包，自动处理依赖问题
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AssetbundleInfo LoadAssetBundle (string abName)
        {
            MakeABNameNotEmpty (ref abName);
            abName = ABNameWithExtension (abName);
            string abPath = FileSystem.CombinePaths (RootDir , abName);
            if ( false == File.Exists (abPath) )
            {
                //加载的AB资源不存在
                Debug.LogErrorFormat ("[{0}] 不存在" , abPath);
                return null;
            }

            AssetbundleInfo ab;
            if ( _loadedAssetBundleDic.ContainsKey (abName) )
            {
                ab = _loadedAssetBundleDic [abName];
            }
            else
            {
                ab = new AssetbundleInfo (AssetBundle.LoadFromFile (abPath) , abName);
                _loadedAssetBundleDic [abName] = ab;
            }

            //依赖检查
            string [] dependList = _manifest.GetAllDependencies (abName);
            foreach ( string depend in dependList )
            {
                if ( false == _loadedAssetBundleDic.ContainsKey (depend) )
                {
                    AssetbundleInfo depAssetBundle = LoadAssetBundle (depend);
                    _loadedAssetBundleDic [depend] = depAssetBundle;
                    ab.Dependencys.Add (depAssetBundle);
                    depAssetBundle.Dependents.Add (ab);
                }
            }
            return ab;
        }

        /// <summary>
        /// 资源是否存在
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public override bool AssetIsExists (string abName , string assetName)
        {
            if ( !File.Exists (Path.Combine (RootDir , abName)) )
            {
                return false;
            }

            string [] depends = GetDepends (abName);
            //穷举依赖数组，如果有任意一个依赖丢失，判断为资源不存在
            for ( int i = 0 ; i < depends.Length ; i++ )
            {
                MakeABNameNotEmpty (ref depends [i]);
                if ( false == File.Exists (FileSystem.CombinePaths (RootDir , ABNameWithExtension (depends [i]))) )
                {
                    return false;
                }
            }

            try
            {
                MakeABNameNotEmpty (ref abName);
                return LoadAssetBundle (ABNameWithExtension (abName)).assetBundle.Contains (assetName);
            }
            catch ( Exception e )
            {
                Debug.LogError ($"检查资源是否存在时出错了,详细信息：{e}");
                return false;
            }
        }
    }
}