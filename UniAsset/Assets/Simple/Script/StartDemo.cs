using UniAsset;
using UnityEngine;

public class StartDemo : MonoBehaviour
{
    private void Awake ()
    {
        ResMgr.Ins.Init (new EditorInitializeParameters ());
        GameObject.Instantiate (ResMgr.Ins.Load<GameObject> (AssetBundleName.PREFAB_OTHER_PREFAB_01 , "prefab_01"));
    }
}