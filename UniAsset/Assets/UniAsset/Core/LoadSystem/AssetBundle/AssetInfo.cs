using UnityEngine;

namespace UniAsset
{
    public interface IAssetInfo
    {

    }

    public class AssetInfo<T> : IAssetInfo where T : UnityEngine.Object
    {
        public readonly string abName;
        public readonly string assetName;
        private const int minLifeTime = 3;
        private readonly float _createTime;
        private T _asset;

        /// <summary>
        /// 引用次数
        /// </summary>
        public int RefCount { get; private set; }

        /// <summary>
        /// 对应的UnityEngine.Object类型资源
        /// </summary>
        public T Asset
        {
            get
            {
                return _asset;
            }
            set
            {
                _asset = value;
                var bundleInfo = ResMgr.Ins.GetBundleInfo (abName);
                if ( bundleInfo != null )
                {
                    bundleInfo.AddRefercingAsset (this);
                }
            }
        }

        public AssetInfo (string abName , string assetName)
        {
            this.abName = abName;
            this.assetName = assetName;
            _createTime = System.DateTime.Now.Second;
        }

        /// <summary>
        /// 增加引用数
        /// </summary>
        public void AddRefCount ()
        {
            RefCount++;
            Debug.Log ($"[AddRefCount] abName:[{abName}],assetName:[{assetName}],RefCount:{RefCount}");
        }

        /// <summary>
        /// 减少引用数
        /// </summary>
        public void SubRefCount ()
        {
            if ( RefCount == 0 )
            {
                return;
            }

            RefCount--;
            TryUnload ();
            Debug.Log ($"[SubRefCount] abName:[{abName}],assetName:[{assetName}],RefCount:{RefCount}");
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public GameObject Instantiate (Transform parent = null)
        {
            if ( IsGameObjectType () )
            {
                var newObj = UnityEngine.Object.Instantiate (Asset , parent) as GameObject;
                var component = newObj.AddComponent<GameObjectLifeListener> ();
                component.assetName = assetName;
                component.abName = abName;
                component.AddRefCount ();
                return newObj;
            }
            Debug.LogError ($"{Asset}不是GameObject类型，无法进行实例化");
            return null;
        }

        /// <summary>
        /// 尝试卸载这个资源
        /// </summary>
        public void TryUnload ()
        {
            if ( RefCount > 0 )
            {
                return;
            }

            //一个资源被创建出来的未达到最小时间周期，先不销毁
            //if ( System.DateTime.Now.Second - _createTime < minLifeTime )
            //{
            //    return;
            //}

            //GameObject无法用UnloadAsset卸载，直接置空等Resources.UnloadUnusedAssets释放
            if ( Asset is GameObject )
            {
                Asset = null;
            }
            //Sprite要卸载texture
            else if ( Asset is Sprite sprite )
            {
                Resources.UnloadAsset (sprite.texture);
            }
            else
            {
                Resources.UnloadAsset (Asset);
            }

            var bundleInfo = ResMgr.Ins.GetBundleInfo (abName);
            if ( bundleInfo != null )
            {
                bundleInfo.SubRefercingAsset (this);
            }
        }

        /// <summary>
        /// 是否为GameObject类型
        /// </summary>
        /// <returns></returns>
        private bool IsGameObjectType ()
        {
            return Asset is GameObject;
        }
    }
}