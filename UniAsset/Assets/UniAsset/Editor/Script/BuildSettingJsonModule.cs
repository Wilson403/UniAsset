using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using UniAsset;
using UnityEditor;
using UnityEngine;

#pragma warning disable IDE0051
#pragma warning disable IDE0052
namespace UniAssetEditor
{
    public class BuildSettingJsonModule : AEditorModule
    {
        /// <summary>
        /// 设置数据
        /// </summary>
        SettingVo _cfg;

        public BuildSettingJsonModule (EditorWindow editorWin) : base (editorWin)
        {
            SetCfg (EditorUtil.LoadConfig<SettingVo> (UniAssetConst.SETTING_FILE_NAME));
        }

        void SetCfg (SettingVo cfg)
        {
            this._cfg = cfg;
            version = cfg.client.version;
            startupResGroups = cfg.startupResGroups;
            startupParams = cfg.startupParams;
            resPackageVerDict = cfg.resPackageVerDict;
            if ( null == startupParams )
            {
                startupParams = new Dictionary<string , string> ();
            }
        }

        void UpdateCfg ()
        {
            _cfg.client.version = version;
            _cfg.startupResGroups = startupResGroups;
            _cfg.startupParams = startupParams;
            _cfg.resPackageVerDict = resPackageVerDict;
        }

        [Title ("setting.json 热更配置文件" , TitleAlignment = TitleAlignments.Centered)]
        [Button ("保存配置" , buttonSize: ButtonSizes.Medium), PropertyOrder (-1)]
        void SaveConfig ()
        {
            UpdateCfg ();
            EditorUtil.SaveConfig (_cfg , UniAssetConst.SETTING_FILE_NAME);
        }

        [Button ("复制本地「setting.json」参数" , buttonSize: ButtonSizes.Medium), PropertyOrder (-1)]
        void LoadExistSettingJson ()
        {
            var selectedFile = EditorUtility.OpenFilePanel ("选择文件" , Application.dataPath , "json");
            if ( false == string.IsNullOrEmpty (selectedFile) )
            {
                try
                {
                    var jsonStr = File.ReadAllText (selectedFile);
                    SetCfg (LitJson.JsonMapper.ToObject<SettingVo> (jsonStr));
                }
                catch ( Exception e )
                {
                    Debug.LogError ("读取选择的setting.json文件失败：" + selectedFile);
                    Debug.LogError (e);
                }
            }
        }

        [InfoBox ("当客户端主版本号低于setting文件主版本号时，将打开更新页面")]
        [LabelText ("客户端版本")]
        public string version;

        [ShowInInspector, DictionaryDrawerSettings (KeyLabel = "APP主版本" , ValueLabel = "资源版本数据")]
        public Dictionary<string , ClientResVerVo> resPackageVerDict = new Dictionary<string , ClientResVerVo> ();

        [Space (10)]
        [InfoBox ("客户端启动运行所必需下载的网络资源组，通过指定group名称来批量下载。如果要下载所有资源，则指定为[/]即可")]
        [LabelText ("启动资源组"), ListDrawerSettings (NumberOfItemsPerPage = 7 , Expanded = false)]
        public string [] startupResGroups;

        [Title ("启动参数配置"), ShowInInspector]
        public Dictionary<string , string> startupParams;


        [Button ("发布「setting.json」" , buttonSize: ButtonSizes.Medium), PropertyOrder (999)]
        void BuildSettingJsonFile ()
        {
            if ( false == Directory.Exists (UniAssetConst.PUBLISH_RES_ROOT_DIR) )
            {
                Directory.CreateDirectory (UniAssetConst.PUBLISH_RES_ROOT_DIR);
            }

            var filePath = FileSystem.CombinePaths (UniAssetConst.PUBLISH_RES_ROOT_DIR , UniAssetConst.SETTING_FILE_NAME);
            if ( File.Exists (filePath) && false == EditorUtility.DisplayDialog ("警告！" , "已存在文件「setting.json」，是否覆盖？" , "Yes" , "No") )
            {
                return;
            }

            UpdateCfg ();
            string jsonStr = LitJson.JsonMapper.ToPrettyJson (_cfg);
            File.WriteAllText (filePath , jsonStr);
            EditorUtil.SaveConfig (_cfg , UniAssetConst.SETTING_FILE_NAME);

            //打开目录
            EditorUtil.OpenDirectory (UniAssetConst.PUBLISH_RES_ROOT_DIR);
        }
    }
}