using UniAsset;
using UnityEngine;

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
            var prefab01 = ResMgr.Ins.Load<GameObject> (AssetBundleName.PREFAB_OTHER_PREFAB_01 , "prefab_01");
            var prefab02 = ResMgr.Ins.Load<GameObject> (AssetBundleName.PREFAB_OTHER_PREFAB_01 , "prefab_01");
            var prefab03 = ResMgr.Ins.Load<GameObject> (AssetBundleName.PREFAB_SKILL , "Test01");

            Debug.LogWarning ($"{prefab01.GetHashCode ()} = {prefab02.GetHashCode ()}");

            ResMgr.Ins.Unload (AssetBundleName.PREFAB_OTHER_PREFAB_01 , false , false);


            //prefab01 = ResMgr.Ins.Load<GameObject> (AssetBundleName.PREFAB_OTHER_PREFAB_01 , "prefab_01");
            //prefab02 = ResMgr.Ins.Load<GameObject> (AssetBundleName.PREFAB_OTHER_PREFAB_01 , "prefab_01");
            //Debug.LogWarning ($"{prefab01.GetHashCode ()} = {prefab02.GetHashCode ()} ");

            Debug.LogWarning (1);
            GameObject.Instantiate (prefab01);
            Debug.LogWarning (2);
            GameObject.Instantiate (prefab03);
            Debug.LogWarning (3);
        });
    }
}