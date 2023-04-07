namespace UniAsset
{
    public class OfflineInitializeParameters : ResInitializeParameters
    {
        public OfflineInitializeParameters ()
        {
            string parentPath = "";
#if UNITY_EDITOR
            parentPath = UniAssetConst.PUBLISH_RES_ROOT_DIR;
#else
            parentPath = FileSystem.CombinePaths (UniAssetConst.PERSISTENT_DATA_PATH , UniAssetConst.UNIASSET_LIBRARY_DIR , "Release" , "res" , UniAssetConst.PLATFORM_DIR_NAME);
#endif
            CertainPathExists (parentPath);
            assetRoot = FileSystem.CombinePaths (parentPath , UniAssetConst.AssetBundleManifestName);
        }
    }
}