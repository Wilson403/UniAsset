namespace UniAsset
{
    public class OnlineInitializeParameters : ResInitializeParameters
    {
        public string netResDir;

        public OnlineInitializeParameters (string netResDir)
        {
            netResDir = netResDir ?? "";
            this.netResDir = netResDir;
            this.netResDir = FileSystem.CombinePaths (this.netResDir , UniAssetConst.PLATFORM_DIR_NAME);
            CertainPathExists (UniAssetConst.WWW_RES_PERSISTENT_DATA_PATH);
            AssetRoot = UniAssetConst.WWW_RES_PERSISTENT_DATA_PATH;
        }
    }
}