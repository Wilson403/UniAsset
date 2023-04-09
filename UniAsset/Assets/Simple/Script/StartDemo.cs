using UniAsset;
using UnityEngine;

public class StartDemo : MonoBehaviour
{
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
            initializeParameters = new OnlineInitializeParameters ();
        }
        else if ( config.resLoadMode == ResLoadMode.LOCAL_ASSET_BUNDLE )
        {
            initializeParameters = new OfflineInitializeParameters ();
        }
        UniAssetRuntime.Ins.Init (initializeParameters);
        GameObject.Instantiate (ResMgr.Ins.Load<GameObject> (AssetBundleName.PREFAB_OTHER_PREFAB_01 , "prefab_01"));
    }
}