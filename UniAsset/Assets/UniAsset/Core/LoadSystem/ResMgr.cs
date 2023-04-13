using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UniAsset.LocalDataModel;

namespace UniAsset
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResMgr : SafeSingleton<ResMgr>
    {
        private BaseResLoader _loader;

        /// <summary>
        /// 资源根目录
        /// </summary>
        public string RootDir
        {
            get
            {
                return _loader.RootDir;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="initializeParameters"></param>
        /// <exception cref="System.Exception"></exception>
        public void Init ()
        {
            switch ( UniAssetRuntime.Ins.ResLoadMode )
            {
                case ResLoadMode.REMOTE_ASSET_BUNDLE:
                case ResLoadMode.LOCAL_ASSET_BUNDLE:
                    string manifestFilePath = FileSystem.CombinePaths (UniAssetRuntime.Ins.ResInitializeParameters.AssetRoot , UniAssetConst.AB_DIR_NAME , UniAssetConst.AssetBundleManifestName);
                    AssetBundleResLoader newMgr = new AssetBundleResLoader (manifestFilePath);
                    if ( _loader != null && _loader is AssetBundleResLoader )
                    {
                        //替换旧的需要继承一下已加载字典
                        newMgr.Inherit (_loader as AssetBundleResLoader);
                    }
                    _loader = newMgr;
                    Debug.Log ($"初始化资源管理器... 资源来源：[AssetBundle]  Manifest路径：{manifestFilePath}");
                    break;

                case ResLoadMode.ASSET_DATA_BASE:
                    _loader = new AssetDataBaseResLoader (UniAssetRuntime.Ins.ResInitializeParameters.AssetRoot);
                    Debug.Log ($"初始化资源管理器... 资源来源：[AssetDataBase] 资源根目录：{UniAssetRuntime.Ins.ResInitializeParameters.AssetRoot}");
                    break;
            }
        }

        internal T Load<T> (object assetBundleName)
        {
            throw new NotImplementedException ();
        }

        /// <summary>
        /// 执行一次内存回收(该接口开销大，可能引起卡顿)
        /// </summary>
        public void DoGC ()
        {
            //移除没有引用的资源
            Resources.UnloadUnusedAssets ();
            GC.Collect ();
        }

        /// <summary>
        /// 得到AB资源的依赖
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public string [] GetDepends (string abName)
        {
            return _loader.GetDepends (abName);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="abName">资源包名称</param>
        /// <param name="isUnloadAllLoaded">是否卸载Hierarchy中的资源</param>
        /// <param name="isUnloadDepends">是否卸载关联的资源</param>
        public void Unload (string abName , bool isUnloadAllLoaded = false , bool isUnloadDepends = true)
        {
            _loader.Unload (abName , isUnloadAllLoaded , isUnloadDepends);
        }

        /// <summary>
        /// 卸载所有资源
        /// </summary>
        /// <param name="isUnloadAllLoaded">是否卸载Hierarchy中的资源</param>
        public void UnloadAll (bool isUnloadAllLoaded = false)
        {
            _loader.UnloadAll (isUnloadAllLoaded);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public AssetInfo<T> Load<T> (string abName , string assetName) where T : UnityEngine.Object
        {
            // 如果任一路径值无效，那么返回空值
            if ( string.IsNullOrEmpty (abName) || string.IsNullOrEmpty (assetName) )
            {
                return null;
            };

            AssetInfo<T> result = null;
            try
            {
                result = _loader.Load<T> (abName , assetName);
            }
            catch ( Exception e )
            {
                Debug.LogError ($"LoadRes过程中捕获到一个错误，_mgr:{_loader},abName:{abName},assetName:{assetName},详细信息:{e}");
            }

            if ( result == null || !result.asset )
            {
                Debug.LogError ($"不存在资源：AB[{abName}] RES[{assetName}]");
            };

            return result;
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="loadSceneMode"></param>
        /// <param name="complete"></param>
        /// <param name="process"></param>
        /// <param name="error"></param>
        public void LoadScene (string abName , string assetName , LoadSceneMode loadSceneMode = LoadSceneMode.Single , Action complete = null , Action<float> process = null , Action<string> error = null)
        {
            _loader.LoadScene (abName , assetName , loadSceneMode , complete , process , error);
        }

        /// <summary>
        /// 通过资源路径加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public AssetInfo<T> Load<T> (string assetPath) where T : UnityEngine.Object
        {
            string abName;
            string assetName;
            SeparateAssetPath (assetPath , out abName , out assetName);
            return Load<T> (abName , assetName);
        }

        /// <summary>
        /// 异步加载一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">资源包名称</param>
        /// <param name="assetName">资源名称</param>
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public void LoadAsync (string abName , string assetName , Action<UnityEngine.Object> onLoaded , Action<float> onProgress = null)
        {
            _loader.LoadAsync (abName , assetName , onLoaded , onProgress);
        }

        /// <summary>
        /// 异步加载一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">资源路径</param>        
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public void LoadAsync (string assetPath , Action<UnityEngine.Object> onLoaded , Action<float> onProgress = null)
        {
            string abName;
            string assetName;
            SeparateAssetPath (assetPath , out abName , out assetName);
            _loader.LoadAsync (abName , assetName , onLoaded , onProgress);
        }

        /// <summary>
        /// 将资源所在路径以及资源名合并成一个完整的资源路径
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public string LinkAssetPath (string abName , string assetName)
        {
            if ( abName == null )
            {
                abName = "";
            }

            if ( assetName == null )
            {
                assetName = null;
            }

            return FileSystem.CombinePaths (abName , assetName);
        }

        /// <summary>
        /// 将一个资源路径拆分为资源父路径以及资源名
        /// </summary>
        /// <param name="assetPath"></param>
        public void SeparateAssetPath (string assetPath , out string abName , out string assetName)
        {
            if ( assetPath == null )
            {
                assetPath = "";
            };

            abName = Path.GetDirectoryName (assetPath);
            assetName = Path.GetFileName (assetPath);

            // 确保去除后缀名
            string [] splited = assetName.Split (new char [] { '.' });
            if ( 1 < splited.Count () )
            {
                assetName = splited [0];
            };
        }

        public bool AssetIsExists (string abName , string assetName)
        {
            return _loader.AssetIsExists (abName , assetName);
        }
    }
}