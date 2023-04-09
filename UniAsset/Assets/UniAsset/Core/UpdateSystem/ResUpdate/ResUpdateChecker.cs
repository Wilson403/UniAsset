using System;
using System.Collections.Generic;
using System.Text;
using Primise4CSharp;

namespace UniAsset
{
    /// <summary>
    /// 检查指定的资源是否需要更新，如果需要，会返回需要更新的资源的列表。
    /// </summary>
    public class ResUpdateChecker
    {
        private readonly bool _needUpdateManifestFile = true;
        private readonly bool _isFirstVer;
        private readonly Promise<string []> _promise;
        private string [] _groups;

        public ResUpdateChecker (bool isFirstVer , bool needUpdateManifestFile = true)
        {
            _isFirstVer = isFirstVer;
            _needUpdateManifestFile = needUpdateManifestFile;
            _promise = new Promise<string []> ();
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="groups"></param>
        public Promise<string []> Start (string [] groups)
        {
            _groups = groups;
            StartUpdateManifest ();
            return _promise;
        }

        /// <summary>
        /// 开始加载Manifest文件（获取依赖用的）
        /// </summary>
        private void StartUpdateManifest ()
        {
            if ( _needUpdateManifestFile )
            {
                Promise manifestFilePromise = new ManifestFileUpdate (_isFirstVer).Start ();
                manifestFilePromise.Then (StartCheckRes);
                manifestFilePromise.Catch (_promise.Reject);
            }
            else
            {
                StartCheckRes ();
            }
        }

        /// <summary>
        /// 进行资源检查
        /// </summary>
        private void StartCheckRes ()
        {
            //整理出所有需要资源的清单（包括依赖的）
            HashSet<string> itemSet = new HashSet<string> ();
            for ( int i = 0 ; i < _groups.Length ; i++ )
            {
                string group = _groups [i];
                List<string> itemList = GetItemsInGroup (group);
                foreach ( string itemName in itemList )
                {
                    itemSet.Add (itemName);
                }
            }

            //开始检查版本，找出需要更新的资源
            List<string> needUpdateList = new List<string> ();
            foreach ( string itemName in itemSet )
            {
                string localVer = UniAssetRuntime.Ins.LocalResVer.GetVer (itemName);
                ResVerItem netItem = UniAssetRuntime.Ins.GetResVerModel (_isFirstVer).Get (itemName);

                //版本不一致，加入更新队列
                if ( localVer != netItem.version )
                {
                    needUpdateList.Add (itemName);
                }
            }

            //如果为首个版本且当前版本有效，进行资源排除
            if ( _isFirstVer && UniAssetRuntime.Ins.Setting.IsUsefulResVer () )
            {
                StringBuilder sb = new StringBuilder ();
                sb.Append ("正在将首个资源包与当前资源版本进行比对，移除当前版本需要下载的资源\n");
                //穷举当前资源版本信息项，将当前版本有的文件从更新列表里移除
                foreach ( ResVerItem item in UniAssetRuntime.Ins.GetResVerModel (false).VO.items )
                {
                    if ( needUpdateList.Contains (item.name) )
                    {
                        needUpdateList.Remove (item.name);
                        sb.Append ($"移除了资源：{item.name}\n");
                    }
                }
                _promise.Resolve (needUpdateList.ToArray ());
            }
            else
            {
                _promise.Resolve (needUpdateList.ToArray ());
            }
        }

        /// <summary>
        /// 获取组里的资源项
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private List<string> GetItemsInGroup (string group)
        {
            List<string> nameList = new List<string> ();
            List<ResVerItem> itemList = UniAssetRuntime.Ins.GetResVerModel (_isFirstVer).FindGroup (group);
            foreach ( ResVerItem item in itemList )
            {
                nameList.Add (item.name);

                //如果是首个版本，进行依赖获取
                List<string> depends = new List<string> ();
                if ( _isFirstVer && _needUpdateManifestFile )
                {
                    depends = GetAllDepends (item.name);
                    nameList.AddRange (depends);
                }
            }
            return nameList;
        }

        /// <summary>
        /// 得到目标所有的依赖
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        private List<string> GetAllDepends (string itemName)
        {
            List<string> nameList = new List<string> ();
            string abDir = ResMgr.Ins.RootDir.Replace (UniAssetRuntime.Ins.ResInitializeParameters.AssetRoot + "/" , "");
            abDir += "/";
            if ( false == itemName.StartsWith (abDir) )
            {
                return nameList;
            }

            string abName = itemName.Replace (abDir , "");
            string [] abDependList = ResMgr.Ins.GetDepends (abName);
            foreach ( string ab in abDependList )
            {
                nameList.Add (FileSystem.CombinePaths (abDir , ab));
            }

            return nameList;
        }
    }
}