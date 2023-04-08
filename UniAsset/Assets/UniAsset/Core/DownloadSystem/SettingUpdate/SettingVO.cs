using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace UniAsset
{
    /// <summary>
    /// 客户端版本号数据
    /// </summary>
    public struct ClientVo
    {
        /// <summary>
        /// 客户端版本
        /// </summary>
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
        /// 是否允许回滚到低版本资源
        /// </summary>
        public bool isRevertLowResVer;

        /// <summary>
        /// 启动资源组列表
        /// </summary>
        public string [] startupResGroups;

        /// <summary>
        /// 配置的参数
        /// </summary>
        public Dictionary<string , string> startupParams;
    }
}