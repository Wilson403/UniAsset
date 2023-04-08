using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace UniAsset
{
    /// <summary>
    /// 配置文件更新
    /// </summary>
    public class SettingUpdate
    {
        Action _onLoaded;
        Action<string> _onError;
        string _localPath;

        public void Start (Action onLoaded , Action<string> onError)
        {
            Debug.Log ("「SettingUpdate」配置文件更新检查...");
            _onLoaded = onLoaded;
            _onError = onError;
            _localPath = FileSystem.CombinePaths (ResMgr.Ins.InitializeParameters.AssetRoot , "setting.json");

            if ( ResMgr.Ins.InitializeParameters is OnlineInitializeParameters onlineInitializeParameters && UniAssetRuntime.Ins.LocalData.IsUpdateSetting )
            {
                string netPath = FileSystem.CombinePaths (onlineInitializeParameters.netResDir , "setting.json");
                Debug.Log ($"配置文件: {netPath}");
                UniAssetRuntime.Ins.StartCoroutine (Update (netPath));
            }
            else
            {
                UniAssetRuntime.Ins.setting = new SettingVo ();
                _onLoaded ();
            }
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
                if ( null != _onError )
                {
                    _onError.Invoke (loader.Error);
                }
                yield break;
            }
            loader.Dispose ();

            SettingVo vo = LoadLocalSetting ();
            UniAssetRuntime.Ins.setting = vo;
            _onLoaded ();
            yield break;
        }
    }
}