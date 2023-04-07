namespace UniAsset
{
    public class OnlineInitializeParameters : ResInitializeParameters
    {
        public OnlineInitializeParameters ()
        {
            CertainPathExists (UniAssetConst.WWW_RES_PERSISTENT_DATA_PATH);
            assetRoot = FileSystem.CombinePaths (UniAssetConst.WWW_RES_PERSISTENT_DATA_PATH , UniAssetConst.AssetBundleManifestName);
        }
    }
}