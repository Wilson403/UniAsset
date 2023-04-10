using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace UniAsset
{
    /// <summary>
    /// 客户端版本号数据
    /// </summary>
    public struct ClientVo
    {
        [LabelText("客户端版本")]
        public string version;
    }

    /// <summary>
    /// 客户端资源版本号数据
    /// </summary>
    public struct ClientResVerVo
    {
        [LabelText ("首个版本")]
        public string firstVer;

        [LabelText ("当前版本")]
        public string currentVer;
    }

    public struct SettingVo
    {
        public ClientVo client;

        /// <summary>
        /// 资源包版本字典
        /// </summary>
        public Dictionary<string , ClientResVerVo> resPackageVerDict;

        /// <summary>
        /// 启动资源组列表
        /// </summary>
        public string [] startupResGroups;

        /// <summary>
        /// 配置的参数
        /// </summary>
        public Dictionary<string , string> startupParams;

        /// <summary>
        /// 获取首个网络资源包版本
        /// </summary>
        /// <returns></returns>
        public string GetFirstNetResPackageVer ()
        {
            if ( resPackageVerDict.TryGetValue (UtilResVersionCompare.Ins.GetAppMainVersion () , out ClientResVerVo clientResVerVo) )
            {
                return clientResVerVo.firstVer;
            }
            return "";
        }

        /// <summary>
        /// 获取当前网络资源包版本
        /// </summary>
        /// <returns></returns>
        public string GetCurrentNetResPackageVer ()
        {
            if ( resPackageVerDict.TryGetValue (UtilResVersionCompare.Ins.GetAppMainVersion () , out ClientResVerVo clientResVerVo) )
            {
                return clientResVerVo.currentVer;
            }
            return "";
        }

        /// <summary>
        /// 资源版本是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsUsefulResVer ()
        {
            if ( resPackageVerDict.TryGetValue (UtilResVersionCompare.Ins.GetAppMainVersion () , out ClientResVerVo vo) )
            {
                return !string.IsNullOrEmpty (vo.currentVer) && !vo.currentVer.Equals (vo.firstVer);
            }
            return false;
        }
    }
}