namespace UniAsset
{
    public class AssetInfo<T> where T : UnityEngine.Object
    {
        public readonly T asset;

        public AssetInfo (T asset)
        {
            this.asset = asset;
        }
    }
}