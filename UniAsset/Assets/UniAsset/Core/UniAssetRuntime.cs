namespace UniAsset
{
    public class UniAssetRuntime : ASingletonMonoBehaviour<UniAssetRuntime>
    {
        public bool IsHotResProject { get; private set; }
        public string LocalResDir { get; private set; }
        public bool IsLoadAssetsByAssetDataBase { get; private set; }
    }
}