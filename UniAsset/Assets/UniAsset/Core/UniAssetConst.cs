using System.IO;
using UnityEngine;

namespace UniAsset
{
    /// <summary>
    /// 常量集合
    /// </summary>
    public class UniAssetConst
    {
        /// <summary>
        /// 热更AssetBundle资源的目录名称
        /// </summary>
        public const string AB_DIR_NAME = "ab";

        /// <summary>
        /// 热更DLL资源的目录名称
        /// </summary>
        public const string DLL_DIR_NAME = "dll";

        /// <summary>
        /// IL2CPP Patch目录
        /// </summary>
        public const string IL2CPP_PATCH = "il2cpp_patch";

        /// <summary>
        /// @Scripts中的代码启动类
        /// </summary>
        public const string LOGIC_SCRIPT_STARTUP_CLASS_NAME = "ZeroHot.Main";

        /// <summary>
        /// @Scripts中的代码启动方法
        /// </summary>
        public const string LOGIC_SCRIPT_STARTUP_METHOD = "Startup";

        /// <summary>
        /// @Scripts中的进入游戏方法
        /// </summary>
        public const string ENTER_GAME_METHOD = "EnterGame";

        /// <summary>
        /// 配置资源的目录名称
        /// </summary>
        public const string CONFIG_DIR_NAME = "configs";

        /// <summary>
        /// 热更DLL的文件名称（不含后缀）
        /// </summary>
        public const string DLL_FILE_NAME = "scripts";

        /// <summary>
        /// 资源版本描述文件的名称
        /// </summary>
        public const string RES_JSON_FILE_NAME = "resVer";

        /// <summary>
        /// 启动配置文件名称
        /// </summary>
        public const string SETTING_FILE_NAME = "setting.json";

        /// <summary>
        /// 预置文件压缩包名称
        /// </summary>
        public const string PACKAGE_ZIP_FILE_NAME = "package.zip";

        /// <summary>
        /// 战斗用到的预制体二进制文件的名称
        /// </summary>
        public const string FIGHTING_CFG_PREFAB_BYTES = "fightingCfgPrefab.bytes";

        /// <summary>
        /// 用于播放服务端的记录
        /// </summary>
        public const string SERVER_RECORD_BYTES = "a_fight.json";

        /// <summary>
        /// AssetBundle文件存储的后缀名
        /// </summary>
        public const string AB_EXTENSION = ".ab";

        /// <summary>
        /// 存储AssetBundle之间依赖关系的manifest文件
        /// </summary>
        public const string MANIFEST_FILE_NAME = "manifest";

        /// <summary>
        /// 直接放在Assets/@Resources目录下的资源，会被打包到root_assets.ab文件中
        /// </summary>
        public const string ROOT_AB_FILE_NAME = "root_assets";

        /// <summary>
        /// 配置MD5描述文件名
        /// </summary>
        public const string CONFIG_MD5_FILE_NAME = "configMd5.txt";

        /// <summary>
        /// 存放下载资源的临时目录名称
        /// </summary>
        public const string TEMP_RES_DIR_NAME = "tempRes";

        /// <summary>
        /// 是否测试解压
        /// </summary>
        public const bool IS_TEST_UNZIP = false;

        #region 基于项目根目录的路径

        /// <summary>
        /// 资源根目录
        /// </summary>
        public const string ASSET_ROOT_DIR = "Assets/UnityResource";

        /// <summary>
        /// 热更配置在项目中的根目录
        /// </summary>
        public const string HOT_CONFIGS_ROOT_DIR = "Assets/@Configs";

        /// <summary>
        /// UniAsset的Library目录
        /// </summary>
        public const string UNIASSET_LIBRARY_DIR = "LibraryUniAsset";

        #endregion

        static string _platformDirName = null;

        /// <summary>
        /// 平台目录
        /// </summary>
        public static string PLATFORM_DIR_NAME
        {
            get
            {
                if ( null == _platformDirName )
                {
#if UNITY_STANDALONE
                    _platformDirName = "pc";
#elif UNITY_IPHONE
        _platformDirName = "ios";
#elif UNITY_ANDROID
                    _platformDirName = "android";
#endif
                }

                return _platformDirName;
            }
        }

        static string _wwwStreamingAssetsPath = null;

        /// <summary>
        /// 可用WWW加载资源的streamingAssets目录地址
        /// </summary>
        public static string WWW_STREAMING_ASSETS_PATH
        {
            get
            {
                if ( null == _wwwStreamingAssetsPath )
                {
                    _wwwStreamingAssetsPath = Application.streamingAssetsPath;
#if UNITY_IPHONE || UNITY_EDITOR_OSX
                    //如果在编辑器下，或是PC平台或iOS平台，则要加上file://才能读取资源
                    _wwwStreamingAssetsPath = "file://" + _wwwStreamingAssetsPath;
#endif
                }
                return _wwwStreamingAssetsPath;
            }
        }

        /// <summary>
        /// 可用WWW加载资源的streamingAssets下的Hot目录
        /// </summary>
        public static string WWW_STREAMING_ASSETS_PATH_HOT
        {
            get
            {
                return FileSystem.CombinePaths (WWW_STREAMING_ASSETS_PATH , "Hot");
            }
        }

        /// <summary>
        /// streamingAssets目录地址
        /// </summary>
        public static string STREAMING_ASSETS_PATH
        {
            get
            {
                return Application.streamingAssetsPath;
            }
        }

        /// <summary>
        /// streamingAssets下的Hot目录
        /// </summary>
        public static string STREAMING_ASSETS_PATH_HOT
        {
            get
            {
                return FileSystem.CombinePaths (STREAMING_ASSETS_PATH , "Hot");
            }
        }

        static string _persistentDataPath = null;
        /// <summary>
        /// 可读写目录地址
        /// </summary>
        public static string PERSISTENT_DATA_PATH
        {
            get
            {
                if ( null == _persistentDataPath )
                {
                    _persistentDataPath = Application.persistentDataPath;
#if UNITY_EDITOR
                    _persistentDataPath = FileSystem.CombineDirs (false , Directory.GetParent (Application.dataPath).FullName , UNIASSET_LIBRARY_DIR , "RuntimeCaches");
#elif UNITY_STANDALONE
                _persistentDataPath = FileSystem.CombineDirs(false, Application.dataPath, "Caches");
#endif
                }
                return _persistentDataPath;
            }
        }

        static string _wwwResPath;
        /// <summary>
        /// 网络下载的更新资源存储的目录
        /// </summary>
        public static string WWW_RES_PERSISTENT_DATA_PATH
        {
            get
            {
                if ( _wwwResPath == null )
                {
                    _wwwResPath = FileSystem.CombineDirs (false , PERSISTENT_DATA_PATH , "zero" , "res");
                }
                return _wwwResPath;
            }
        }

        static string _generatesPath;
        /// <summary>
        /// 框架生成文件存放地址
        /// </summary>
        public static string GENERATES_PERSISTENT_DATA_PATH
        {
            get
            {
                if ( _generatesPath == null )
                {
                    _generatesPath = FileSystem.CombineDirs (false , PERSISTENT_DATA_PATH , "zero" , "generated");
                }
                return _generatesPath;
            }
        }

        static string _publicResRootDir;
        /// <summary>
        /// 热更资源发布目录
        /// </summary>
        public static string PUBLISH_RES_ROOT_DIR
        {
            get
            {
                if ( _publicResRootDir == null )
                {
                    _publicResRootDir = FileSystem.CombineDirs (false , UNIASSET_LIBRARY_DIR , "Release" , "res" , PLATFORM_DIR_NAME);
                }
                return _publicResRootDir;
            }
        }

        static string _manifestName;
        /// <summary>
        /// AB的Manifest文件名
        /// </summary>
        public static string AssetBundleManifestName
        {
            get
            {
                if ( _manifestName == null )
                {
                    _manifestName = FileSystem.CombinePaths (AB_DIR_NAME , MANIFEST_FILE_NAME + AB_EXTENSION);
                }
                return _manifestName;
            }
        }
    }
}