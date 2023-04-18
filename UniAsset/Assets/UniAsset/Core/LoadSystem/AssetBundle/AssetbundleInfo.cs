﻿using System.Collections.Generic;
using UnityEngine;

namespace UniAsset
{
    /// <summary>
    /// 资源包体信息
    /// </summary>
    public class BundleInfo
    {
        /// <summary>
        /// 对应的AssetBundle对象
        /// </summary>
        public readonly AssetBundle assetBundle;

        /// <summary>
        /// 对应的AssetBundle名称
        /// </summary>
        public readonly string assetBundleName;

        /// <summary>
        /// 依赖其它的资源集合
        /// </summary>
        public readonly HashSet<BundleInfo> Dependencys;

        /// <summary>
        /// 被其它资源依赖集合
        /// </summary>
        public readonly HashSet<BundleInfo> Dependents;

        /// <summary>
        /// Asset引用集合
        /// </summary>
        public readonly HashSet<AssetInfo> ReferenceAssets;

        public BundleInfo (AssetBundle assetBundle , string assetBundleName)
        {
            this.assetBundle = assetBundle;
            this.assetBundleName = assetBundleName;
            Dependencys = new HashSet<BundleInfo> ();
            Dependents = new HashSet<BundleInfo> ();
            ReferenceAssets = new HashSet<AssetInfo> ();
        }

        /// <summary>
        /// 添加引用中的资源
        /// </summary>
        /// <param name="assetInfo"></param>
        public void AddRefercingAsset (AssetInfo assetInfo)
        {
            ReferenceAssets.Add (assetInfo);
        }

        /// <summary>
        /// 移除引用中的资源
        /// </summary>
        /// <param name="assetInfo"></param>
        public void SubRefercingAsset (AssetInfo assetInfo)
        {
            ReferenceAssets.Remove (assetInfo);
        }
    }
}