using System;
using System.IO;
using UnityEngine;

namespace UniAsset
{
    /// <summary>
    /// 本地版本文件数据模型
    /// </summary>
    public class LocalResVerModel : ResVerModel
    {
        const string FILE_NAME = "local_res_ver";
        string _path;

        public LocalResVerModel ()
        {
            Load ();
        }

        public void Load ()
        {
            _path = FileSystem.CombinePaths (UniAssetConst.GENERATES_PERSISTENT_DATA_PATH , FILE_NAME);
            if ( File.Exists (_path) )
            {
                TryLoadResVerVo (_path , out vo);
            }
            else
            {
                vo = new ResVerVo (); //新数据初始化
            }

            if ( vo.items == null )
            {
                vo.items = new ResVerItem [0];
            }
            else
            {
                ConformingLocalRes ();
            }
        }

        /// <summary>
        /// 尝试加载资源版本信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="resVerVo"></param>
        private void TryLoadResVerVo (string path , out ResVerVo resVerVo)
        {
            try
            {
                resVerVo = LitJson.JsonMapper.ToObject<ResVerVo> (File.ReadAllText (path));
            }
            catch ( Exception e )
            {
                Debug.LogError ($"读取资源版本信息出错中断，点击Unity恢复按钮可继续执行, 路径：{path}，详细信息：{e}");
                resVerVo = new ResVerVo (); //新数据初始化
            }
        }

        /// <summary>
        /// 创建本地的资源版本描述文件
        /// </summary>
        public void CreateLocalResVerFile ()
        {
            string resFilePath = FileSystem.CombinePaths (UniAssetRuntime.Ins.ResInitializeParameters.AssetRoot , UniAssetConst.RES_JSON_FILE_NAME);
            if ( !File.Exists (resFilePath) )
            {
                Debug.Log ($"路径{resFilePath}不存在，跳过资源描述文件写入步骤");
                return;
            }

            TryLoadResVerVo (resFilePath , out ResVerVo resVerVo);
            foreach ( ResVerItem item in resVerVo.items )
            {
                SetVerAndSave (item.name , item.version);
            }
            vo.resPackageVer = resVerVo.resPackageVer;
            Save ();
        }

        /// <summary>
        /// 检查本地的文件，是否存在或者版本号是否一致
        /// </summary>
        public void ConformingLocalRes ()
        {
            foreach ( ResVerItem item in vo.items )
            {
                string filePath = FileSystem.CombinePaths (UniAssetRuntime.Ins.ResInitializeParameters.AssetRoot , item.name);
                if ( !File.Exists (filePath) )
                {
                    RemoveVer (item.name);
                }
            }
            Save ();
        }

        /// <summary>
        /// 保存资源包版本号
        /// </summary>
        public void SaveResPackageVer ()
        {
            string settingFilePath = FileSystem.CombinePaths (UniAssetRuntime.Ins.ResInitializeParameters.AssetRoot , UniAssetConst.SETTING_FILE_NAME);
            vo.resPackageVer = "0";
            if ( File.Exists (settingFilePath) )
            {
                SettingVo settingVo = LitJson.JsonMapper.ToObject<SettingVo> (File.ReadAllText (settingFilePath));
                if ( settingVo.resPackageVerDict.TryGetValue (UtilResVersionCompare.Ins.GetAppMainVersion () , out ClientResVerVo clientResVerVo) )
                {
                    vo.resPackageVer = clientResVerVo.currentVer;
                }
            }
            Save ();
            Debug.Log ($"保存资源包版本号{vo.resPackageVer}");
        }

        /// <summary>
        /// 设置文件版本号
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public void SetVerAndSave (string name , string version)
        {
            SetVer (name , version);
            Save ();
        }

        /// <summary>
        /// 移除指定文件的版本信息
        /// </summary>
        /// <returns>The ver.</returns>
        /// <param name="name">Name.</param>
        public void RemoveVerAndSave (string name)
        {
            RemoveVer (name);
            Save ();
        }

        /// <summary>
        /// 清理所有版本信息
        /// </summary>
        public void ClearVerAndSave ()
        {
            ClearVer ();
            Save ();
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save ()
        {
            File.WriteAllText (_path , LitJson.JsonMapper.ToJson (vo));
        }
    }
}