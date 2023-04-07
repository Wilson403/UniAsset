namespace UniAsset
{
    public class OnlineInitializeParameters : ResInitializeParameters
    {
        public OnlineInitializeParameters ()
        {
            assetRoot = FileSystem.CombinePaths (ResMgr.Ins.LocalResDir , UniAssetConst.AssetBundleManifestName);
        }
    }
}