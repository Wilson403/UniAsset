namespace UniAsset
{
    public class OfflineInitializeParameters : ResInitializeParameters
    {
        public OfflineInitializeParameters () 
        {
            assetRoot = FileSystem.CombinePaths (ResMgr.Ins.LocalResDir , UniAssetConst.AssetBundleManifestName);
        }
    }
}