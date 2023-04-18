using UnityEngine;

namespace UniAsset
{
    /// <summary>
    /// 用来监听GameObject的生命周期，被创建时通知引用次数增加，销毁时通知引用次数减少
    /// </summary>
    public class GameObjectLifeListener : MonoBehaviour
    {
        public string abName;
        public string assetName;

        public void AddRefCount ()
        {
            if ( !string.IsNullOrEmpty (abName) && !string.IsNullOrEmpty (assetName) )
            {
                var assetInfo = ResMgr.Ins.GetAssetInfo (abName , assetName);
                if ( assetInfo != null )
                {
                    assetInfo.AddRefCount ();
                }
            }
        }

        private void Awake ()
        {
            AddRefCount ();
        }

        private void OnDestroy ()
        {
            var assetInfo = ResMgr.Ins.GetAssetInfo (abName , assetName);
            if ( assetInfo != null )
            {
                assetInfo.SubRefCount ();
            }
        }
    }
}