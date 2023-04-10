using System.IO;

namespace UniAsset
{
    public class OfflineInitializeParameters : ResInitializeParameters
    {
        public OfflineInitializeParameters ()
        {
#if UNITY_EDITOR
            string localSettingFile = FileSystem.CombinePaths (UniAssetConst.PUBLISH_RES_ROOT_DIR , "setting.json");
            SettingVo vo = LitJson.JsonMapper.ToObject<SettingVo> (File.ReadAllText (localSettingFile));
            AssetRoot = FileSystem.CombinePaths (UniAssetConst.PUBLISH_RES_ROOT_DIR , UtilResVersionCompare.Ins.GetAppMainVersion () , vo.GetCurrentNetResPackageVer ());
#else
            AssetRoot = FileSystem.CombinePaths (UniAssetConst.PERSISTENT_DATA_PATH , UniAssetConst.UNIASSET_LIBRARY_DIR , "Release" , "res" , UniAssetConst.PLATFORM_DIR_NAME );
#endif
            CertainPathExists (AssetRoot);
        }
    }
}