using System;
using System.Collections;
using System.IO;
using Primise4CSharp;
using UnityEngine;

namespace UniAsset
{
    /// <summary>
    /// 网络端版本号描述文件加载
    /// </summary>
    public class ResVerFileUpdate
    {
        /// <summary>
        /// 文件本地路径
        /// </summary>
        private readonly string _localPath;
        private Promise _promise;

        public ResVerFileUpdate ()
        {
            _promise = new Promise ();
            _localPath = FileSystem.CombinePaths (UniAssetRuntime.Ins.ResInitializeParameters.AssetRoot , UniAssetConst.RES_JSON_FILE_NAME);
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="onLoaded"></param>
        /// <param name="onError"></param>
        public Promise Start ()
        {
            var url = UniAssetRuntime.Ins.GetRootUrl (true);
            Debug.Log ($"「{url}」首个资源版本号文件更新检查...");
            UniAssetRuntime.Ins.StartCoroutine (UpdateResVerFile (url , OnUpdateFirstResVerComplete));
            return _promise;
        }

        /// <summary>
        /// 当首个版本文件更新完成时
        /// </summary>
        private void OnUpdateFirstResVerComplete ()
        {
            TryLoadResVerFile (out UniAssetRuntime.Ins.firstNetResVer);
            if ( UniAssetRuntime.Ins.Setting.IsUsefulResVer () )
            {
                var url = UniAssetRuntime.Ins.GetRootUrl (false);
                Debug.Log ($"「{url}」最新资源版本号文件更新检查...");
                UniAssetRuntime.Ins.StartCoroutine (UpdateResVerFile (url , () =>
                {
                    TryLoadResVerFile (out UniAssetRuntime.Ins.currentNetResVer);
                    _promise.Resolve ();
                }));
            }
            else
            {
                _promise.Resolve ();
            }
        }

        /// <summary>
        /// 尝试加载ResVer文件
        /// </summary>
        /// <returns></returns>
        private bool TryLoadResVerFile (out ResVerModel resVerModel)
        {
            try
            {
                ResVerVo vo = LitJson.JsonMapper.ToObject<ResVerVo> (File.ReadAllText (_localPath));
                resVerModel = new ResVerModel (vo);
                return true;
            }
            catch ( Exception e )
            {
                resVerModel = new ResVerModel ();
                _promise.Reject (e);
                return false;
            }
        }

        /// <summary>
        /// 更新ResVer文件
        /// </summary>
        /// <param name="netPath"></param>
        /// <param name="updateComplete"></param>
        /// <returns></returns>
        IEnumerator UpdateResVerFile (string netPath , Action updateComplete)
        {
            //从远程服务器下载资源版本描述文件
            Downloader loader = new Downloader (FileSystem.CombinePaths (netPath , UniAssetConst.RES_JSON_FILE_NAME) , _localPath , DateTime.UtcNow.ToFileTimeUtc ().ToString ());
            while ( false == loader.IsDone )
            {
                yield return new WaitForEndOfFrame ();
            }

            if ( null != loader.Error )
            {
                _promise.Reject (new Exception (loader.Error));
                yield break;
            }

            loader.Dispose ();
            updateComplete?.Invoke ();
            yield break;
        }
    }
}