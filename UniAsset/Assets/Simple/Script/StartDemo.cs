using UniAsset;
using UnityEngine;

public class StartDemo : MonoBehaviour
{
    private void Awake ()
    {
        UniAssetRuntime.Ins.Init ();
        ResMgr.Ins.Init ();
        GameObject.Instantiate (ResMgr.Ins.Load<GameObject> (AssetBundleName.PREFAB_OTHER_PREFAB_01 , "prefab_01"));
    }
}