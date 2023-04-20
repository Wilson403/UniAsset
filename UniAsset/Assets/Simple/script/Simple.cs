using UniAsset;
using UnityEngine;
using UnityEngine.UI;

public class Simple : MonoBehaviour
{
    public Button btnOk;
    bool _isOk;

    private void Awake ()
    {
        var config = GetComponent<ResConfig> ();
        ResInitializeParameters initializeParameters = null;
        if ( config.resLoadMode == ResLoadMode.ASSET_DATA_BASE )
        {
            initializeParameters = new EditorInitializeParameters ();
        }
        else if ( config.resLoadMode == ResLoadMode.REMOTE_ASSET_BUNDLE )
        {
            initializeParameters = new OnlineInitializeParameters (config.netPath);
        }
        else if ( config.resLoadMode == ResLoadMode.LOCAL_ASSET_BUNDLE )
        {
            initializeParameters = new OfflineInitializeParameters ();
        }

        //初始化UniAsset运行环境
        UniAssetRuntime.Ins.Init (initializeParameters);

        //确保资源准备完成，完成后即可正常加载
        UniAssetRuntime.Ins.StartPreload ().Then (() =>
        {
            _isOk = true;
            LoadTest ();
        });

        btnOk.onClick.AddListener (LoadTest);
    }

    void LoadTest ()
    {
        if ( !_isOk )
        {
            return;
        }

        var assetInfo = ResMgr.Ins.Load<GameObject> (AssetBundleName.PREFAB_OTHER_PREFAB_01 , "prefab_01");
        var obj01 = assetInfo.Instantiate (transform);
        Object.Instantiate (obj01 , transform);
    }
}