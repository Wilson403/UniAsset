using UnityEngine;

namespace UniAsset
{
    public class AssetInfo
    {
        public string abName;
        public string assetName;
        public BundleInfo bundleInfo;
        public UnityEngine.Object asset;
        public int RefCount { get; private set; }

        public AssetInfo ()
        {

        }

        public AssetInfo (UnityEngine.Object asset)
        {
            this.asset = asset;
            bundleInfo?.AddRefercingAsset (this);
        }

        /// <summary>
        /// 增加引用数
        /// </summary>
        public void AddRefCount ()
        {
            RefCount++;
            if ( RefCount == 1 )
            {
                bundleInfo?.AddRefercingAsset (this);
            }
        }

        /// <summary>
        /// 减少引用树
        /// </summary>
        public void SubRefCount ()
        {
            RefCount--;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <returns></returns>
        public GameObject Instantiate ()
        {
            if ( asset is GameObject gameObject )
            {
                var newObj = UnityEngine.Object.Instantiate (gameObject);
                var component = newObj.AddComponent<GameObjectLifeListener> ();
                component.assetName = assetName;
                component.abName = abName;
                component.AddRefCount ();
                return newObj;
            }
            Debug.LogError ($"{asset}不是GameObject类型，无法进行实例化");
            return null;
        }
    }
}