using System.Diagnostics;
using UniAsset;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class LocalAssetBundle : MonoBehaviour
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
            initializeParameters = new OnlineInitializeParameters (config.netPath);
        }
        else if ( config.resLoadMode == ResLoadMode.LOCAL_ASSET_BUNDLE )
        {
            initializeParameters = new OfflineInitializeParameters ();
        }

        UniAssetRuntime.Ins.Init (initializeParameters);
        UniAssetRuntime.Ins.StartPreload ().Then (() =>
        {
            var prefab01 = ResMgr.Ins.Load<GameObject> (AssetBundleName.PREFAB_OTHER_PREFAB_01 , "prefab_01").Instantiate ();
            Object.Instantiate (prefab01);
        });
    }
}