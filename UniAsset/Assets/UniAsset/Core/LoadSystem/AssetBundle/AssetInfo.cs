namespace UniAsset
{
    public class AssetInfo
    {
        public AssetbundleInfo bundleInfo;
        public UnityEngine.Object asset;
        public int RefCount { get; private set; }

        public AssetInfo ()
        {

        }

        public AssetInfo (UnityEngine.Object asset)
        {
            this.asset = asset;
        }

        public void AddRefCount ()
        {
            RefCount++;
            if ( RefCount == 1 )
            {
                bundleInfo?.AddRefercingAsset (this);
            }
        }

        public void SubRefCount ()
        {
            RefCount--;
        }
    }
}