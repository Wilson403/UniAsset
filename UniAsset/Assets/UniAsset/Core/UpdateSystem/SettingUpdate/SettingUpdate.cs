using System;
using System.Collections;
using System.IO;
using Primise4CSharp;
using UnityEngine;

namespace UniAsset
{
    /// <summary>
    /// 配置文件更新
    /// </summary>
    public class SettingUpdate
    {
        Promise<SettingVo> _promise;
        string _localPath;

        public Promise<SettingVo> Start ()
        {
            
            _promise = new Promise<SettingVo> ();
            _localPath = FileSystem.CombinePaths (UniAssetRuntime.Ins.ResInitializeParameters.AssetRoot , "setting.json");
            if ( UniAssetRuntime.Ins.ResInitializeParameters is OnlineInitializeParameters onlineInitializeParameters && UniAssetRuntime.Ins.LocalData.IsUpdateSetting )
            {
                string netPath = FileSystem.CombinePaths (onlineInitializeParameters.netResDir , "setting.json");
                Debug.Log ($"[{netPath}] setting文件更新检查...");
                UniAssetRuntime.Ins.StartCoroutine (Update (netPath));
            }
            else
            {
                _promise.Resolve (new SettingVo ());
            }
            return _promise;
        }

        SettingVo LoadLocalSetting ()
        {
            SettingVo vo;
            try
            {
                string settingJsonStr = File.ReadAllText (_localPath);
                vo = LitJson.JsonMapper.ToObject<SettingVo> (settingJsonStr);
            }
            catch ( Exception e )
            {
                Debug.LogError (e.ToString ());
                vo = new SettingVo ();
            }
            return vo;
        }

        IEnumerator Update (string url)
        {
            Downloader loader = new Downloader (url , _localPath , DateTime.UtcNow.ToFileTimeUtc ().ToString ());
            while ( false == loader.IsDone )
            {
                yield return new WaitForEndOfFrame ();
            }

            if ( null != loader.Error )
            {
                Debug.LogErrorFormat (loader.Error);
                _promise.Reject (new Exception (loader.Error));
                yield break;
            }
            loader.Dispose ();

            SettingVo vo = LoadLocalSetting ();
            _promise.Resolve (vo);
            yield break;
        }
    }
}