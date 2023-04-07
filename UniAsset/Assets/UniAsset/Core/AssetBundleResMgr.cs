using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniAsset
{
    public class AssetBundleResMgr : AResMgr
    {
        /// <summary>
        /// 资源描述
        /// </summary>
        AssetBundleManifest _manifest;

        /// <summary>
        /// 已加载的AB资源字典
        /// </summary>
        Dictionary<string , AssetBundle> _loadedABDic;

        public AssetBundleResMgr (string manifestFilePath)
        {
            try
            {
                UnloadAll ();
                _loadedABDic = new Dictionary<string , AssetBundle> ();
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
        internal void Inherit (AssetBundleResMgr source)
        {
            _loadedABDic = source._loadedABDic;
        }

        /// <summary>
        /// 如果
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
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

        public override T Load<T> (string abName , string assetName)
        {
            MakeABNameNotEmpty (ref abName);
            abName = ABNameWithExtension (abName);
            AssetBundle ab = LoadAssetBundle (abName);
            T asset = ab.LoadAsset<T> (assetName);
            if ( null == asset )
            {
                Debug.LogErrorFormat ("获取的资源不存在： AssetBundle: {0}  Asset: {1}" , abName , assetName);
            }
            return asset;
        }

        public override void LoadAsync (string abName , string assetName , Action<UnityEngine.Object> onLoaded , Action<float> onProgress = null)
        {
            MakeABNameNotEmpty (ref abName);
            abName = ABNameWithExtension (abName);
            AssetBundle ab = LoadAssetBundle (abName);
            ResMgr.Ins.StartCoroutine (LoadAsync (ab , assetName , onLoaded , onProgress));
        }

        IEnumerator LoadAsync (AssetBundle ab , string assetName , Action<UnityEngine.Object> onLoaded , Action<float> onProgress)
        {
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

            //加载完成
            onLoaded.Invoke (abr.asset);
        }

        public override void LoadScene (string abName , string assetName , LoadSceneMode loadSceneMode = LoadSceneMode.Single , Action complete = null , Action<float> process = null , Action<string> error = null)
        {
            MakeABNameNotEmpty (ref abName);
            abName = ABNameWithExtension (abName);
            AssetBundle ab = LoadAssetBundle (abName);
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
            if ( _loadedABDic.ContainsKey (abName) )
            {
                AssetBundle ab = _loadedABDic [abName];
                _loadedABDic.Remove (abName);
                ab.Unload (isUnloadAllLoaded);
                //Debug.LogFormat("释放AB：{0}", abName);

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
            foreach ( KeyValuePair<string , AssetBundle> loadedEntry in _loadedABDic )
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
            if ( null != _loadedABDic )
            {
                foreach ( AssetBundle cached in _loadedABDic.Values )
                {
                    cached.Unload (isUnloadAllLoaded);
                }
                _loadedABDic.Clear ();
            }

            ResMgr.Ins.DoGC ();
        }

        /// <summary>
        /// 加载AB包，自动处理依赖问题
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AssetBundle LoadAssetBundle (string abName)
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

            AssetBundle ab;
            if ( _loadedABDic.ContainsKey (abName) )
            {
                ab = _loadedABDic [abName];
            }
            else
            {
                ab = AssetBundle.LoadFromFile (abPath);
                _loadedABDic [abName] = ab;
            }

            //依赖检查
            string [] dependList = _manifest.GetAllDependencies (abName);
            foreach ( string depend in dependList )
            {
                //string dependPath = Path.Combine(_rootDir, depend);
                if ( false == _loadedABDic.ContainsKey (depend) )
                {
                    _loadedABDic [depend] = LoadAssetBundle (depend);
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
                return LoadAssetBundle (ABNameWithExtension (abName)).Contains (assetName);
            }
            catch ( Exception e )
            {
                Debug.LogError ($"检查资源是否存在时出错了,详细信息：{e}");
                return false;
            }
        }
    }
}