namespace UniAsset
{
    public class AssetbundleInfo<T> : AssetInfo<T> where T : UnityEngine.Object
    {
        public AssetbundleInfo (T asset) : base (asset)
        {
        }
    }
}