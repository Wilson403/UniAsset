namespace UniAsset
{
    public class OnlineInitializeParameters : ResInitializeParameters
    {
        public string NetResDir { get; private set; }

        public OnlineInitializeParameters (string netResDir)
        {
            netResDir = netResDir ?? "";
            NetResDir = netResDir;
            NetResDir = FileSystem.CombinePaths (this.NetResDir , UniAssetConst.PLATFORM_DIR_NAME);
            CertainPathExists (UniAssetConst.WWW_RES_PERSISTENT_DATA_PATH);
            AssetRoot = UniAssetConst.WWW_RES_PERSISTENT_DATA_PATH;
        }
    }
}