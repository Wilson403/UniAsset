namespace UniAsset
{
    public class UniAssetRuntime : ASingletonMonoBehaviour<UniAssetRuntime>
    {
        public string LocalResDir { get; private set; }
        public bool IsLoadAssetsByAssetDataBase { get; private set; }
    }
}