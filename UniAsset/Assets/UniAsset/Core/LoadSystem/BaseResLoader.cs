using System;
using UnityEngine.SceneManagement;

namespace UniAsset
{
    /// <summary>
    /// 资源加载抽象基类
    /// </summary>
    public abstract class BaseResLoader
    {
        /// <summary>
        /// 如果AB名称没有后缀，则加上后缀名
        /// </summary>
        /// <param name="abName"></param>
        protected string ABNameWithExtension (string abName)
        {
            if ( false == abName.EndsWith (UniAssetConst.AB_EXTENSION) )
            {
                abName += UniAssetConst.AB_EXTENSION;
            }
            abName = FileSystem.StandardizeBackslashSeparator (abName);
            return abName;
        }

        /// <summary>
        /// 如果AB名称有后缀，则去掉
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        protected string ABNameWithoutExtension (string abName)
        {
            if ( abName.EndsWith (UniAssetConst.AB_EXTENSION) )
            {
                abName = abName.Replace (UniAssetConst.AB_EXTENSION , "");
            }
            abName = FileSystem.StandardizeBackslashSeparator (abName);
            return abName;
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
        public virtual void LoadScene (string abName , string assetName , LoadSceneMode loadSceneMode = LoadSceneMode.Single , Action complete = null , Action<float> process = null , Action<string> error = null)
        {
            //默认使用EditorSetting的方式来进行场景加载
            UtilScene.Ins.LoadScene (assetName , loadSceneMode: loadSceneMode , complete: complete , process: process , error: error);
        }

        /// <summary>
        /// AssetBundle文件的根目录
        /// </summary>
        public string RootDir { get; protected set; }

        /// <summary>
        /// 得到资源的依赖
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public abstract string [] GetDepends (string abName);

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isUnloadAllLoaded"></param>
        /// <param name="isUnloadDepends"></param>
        public abstract void Unload (string abName , bool isUnloadAllLoaded = false , bool isUnloadDepends = true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isUnloadAllLoaded"></param>
        public abstract void UnloadAll (bool isUnloadAllLoaded = false);

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public abstract AssetInfo<T> Load<T> (string abName , string assetName) where T : UnityEngine.Object;

        /// <summary>
        /// 异步获取一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public abstract void LoadAsync (string abName , string assetName , Action<UnityEngine.Object> onLoaded , Action<float> onProgress = null);


        /// <summary>
        /// 资源是否存在
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public abstract bool AssetIsExists (string abName , string assetName);
    }
}