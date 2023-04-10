using System;
using System.Collections;
using Primise4CSharp;
using UnityEngine;

namespace UniAsset
{
    /// <summary>
    /// 资源更新（只需要指定需要下载的资源，依赖的资源会自动添加到下载队列）
    /// </summary>
    public class ResUpdate
    {
        /// <summary>
        /// 是否需要更新资源的Manifest文件至最新版本（默认为true）
        /// </summary>
        private readonly bool _needUpdateManifestFile = true;

        /// <summary>
        /// 是否为首版本(首版本：即APP对应的首个资源包，至少存在一个资源包，若只有一个资源包，那么它就是首个资源包)
        /// </summary>
        private readonly bool _isFirstVer;
        private Action<float , long> _onProgress;
        private string [] _groups;
        private readonly Promise<bool> _promise;

        public ResUpdate (bool isFirstVer , bool needUpdateManifestFile = true)
        {
            _promise = new Promise<bool> ();
            _isFirstVer = isFirstVer;
            _needUpdateManifestFile = needUpdateManifestFile;
        }

        public Promise<bool> Start (string [] groups , Action<float , long> onProgress = null)
        {
            _onProgress = onProgress;
            _groups = groups;
            var resUpdateCheckerPromise = new ResUpdateChecker (_isFirstVer , _needUpdateManifestFile).Start (_groups);
            resUpdateCheckerPromise.Then (OnResUpdateChecked);
            resUpdateCheckerPromise.Catch (_promise.Reject);
            return _promise;
        }

        private void OnResUpdateChecked (string [] needUpdateResList)
        {
            //如果下载列表长度为0，直接下一步
            if ( needUpdateResList.Length == 0 )
            {
                _promise.Resolve (false);
                return;
            }
            UniAssetRuntime.Ins.StartCoroutine (UpdateGroups (needUpdateResList));
        }

        IEnumerator UpdateGroups (string [] needUpdateResList)
        {
            //实例化一个资源组下载器
            GroupDownloader groupLoader = new GroupDownloader ();

            //穷举更新列表，并加入下载器
            for ( int i = 0 ; i < needUpdateResList.Length ; i++ )
            {

                string resName = needUpdateResList [i];
                string url = FileSystem.CombinePaths (UniAssetRuntime.Ins.GetAssetBundleUrl (_isFirstVer) , resName);
                string savePath = FileSystem.CombinePaths (UniAssetRuntime.Ins.ResInitializeParameters.AssetRoot , resName);
                ResVerItem netItem = UniAssetRuntime.Ins.GetResVerModel (_isFirstVer).Get (resName);
                //将要下载的文件依次添加入下载器
                groupLoader.AddLoad (url , savePath , netItem.version , netItem.size , OnItemLoaded , netItem);
            }

            //启动下载器开始下载
            groupLoader.StartLoad ();

            //判断是否所有资源下载完成，如果没有，返回一个下载的进度（该进度表示的整体进度）
            do
            {
                _onProgress.Invoke (groupLoader.LoadedSize , groupLoader.TotalSize);
                yield return new WaitForEndOfFrame ();
            }
            while ( !groupLoader.IsDone );

            //判断下载是否返回错误
            if ( null != groupLoader.Error )
            {
                _promise.Reject (new Exception (groupLoader.Error));
                groupLoader.Destroy ();
                yield break;
            }

            _promise.Resolve (needUpdateResList.Length > 0);
            groupLoader.Destroy ();
        }

        private void OnItemLoaded (object obj)
        {
            ResVerItem item = ( ResVerItem ) obj;
            UniAssetRuntime.Ins.LocalResVer.SetVerAndSave (item.name , item.version);
            Debug.Log ($"下载完成：{item.name} Ver:{item.version}");
        }
    }
}