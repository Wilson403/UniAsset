namespace UniAsset
{
    public class OfflineInitializeParameters : ResInitializeParameters
    {
        public OfflineInitializeParameters ()
        {
#if UNITY_EDITOR
            AssetRoot = UniAssetConst.PUBLISH_RES_ROOT_DIR;
#else
            AssetRoot = FileSystem.CombinePaths (UniAssetConst.PERSISTENT_DATA_PATH , UniAssetConst.UNIASSET_LIBRARY_DIR , "Release" , "res" , UniAssetConst.PLATFORM_DIR_NAME);
#endif
            CertainPathExists (AssetRoot);
        }
    }
}