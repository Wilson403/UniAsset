using UniAsset;
using UnityEditor;

namespace UniAssetEditor
{
    public class UniAssetEditorConst
    {
        /// <summary>
        /// 当前发布平台
        /// </summary>
        static public BuildTarget BUILD_PLATFORM
        {
            get
            {
                BuildTarget platform;
#if UNITY_STANDALONE
                platform = BuildTarget.StandaloneWindows;
#elif UNITY_IPHONE
        platform = BuildTarget.iOS;
#elif UNITY_ANDROID
                platform = BuildTarget.Android;
#endif
                return platform;
            }
        }

        /// <summary>
        /// AssetBundle打包缓存目录
        /// </summary>
        static public string ASSET_BUNDLE_CACHE_DIR = FileSystem.CombineDirs (false , UniAssetConst.UNIASSET_LIBRARY_DIR , "ReleaseCache" , UniAssetConst.AB_DIR_NAME);

        /// <summary>
        /// 编辑器配置文件目录
        /// </summary>
        static public string EDITOR_CONFIG_DIR = FileSystem.CombineDirs (false , UniAssetConst.UNIASSET_LIBRARY_DIR , "EditorConfigs");
    }
}