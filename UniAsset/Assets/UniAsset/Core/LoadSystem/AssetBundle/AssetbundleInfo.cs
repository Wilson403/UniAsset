using UnityEngine;

namespace UniAsset
{
    public class AssetbundleInfo
    {
        public readonly AssetBundle assetBundle;
        public readonly string assetBundleName;

        public AssetbundleInfo (AssetBundle assetBundle , string assetBundleName)
        {
            this.assetBundle = assetBundle;
            this.assetBundleName = assetBundleName;
        }
    }
}