namespace UniAsset
{
    public class OnlineInitializeParameters : ResInitializeParameters
    {
        public string netResDir;

        public OnlineInitializeParameters ()
        {
            CertainPathExists (UniAssetConst.WWW_RES_PERSISTENT_DATA_PATH);
            AssetRoot = UniAssetConst.WWW_RES_PERSISTENT_DATA_PATH;
        }
    }
}