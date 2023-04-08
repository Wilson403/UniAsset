using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UniAsset
{
    /// <summary>
    /// 文件版本号数据模型(默认只读）
    /// </summary>
    public class ResVerModel
    {
        protected ResVerVo vo;

        public ResVerVo VO
        {
            get
            {
                return vo;
            }
        }

        public ResVerModel ()
        {

        }

        public ResVerModel (ResVerVo vo)
        {
            this.vo = vo;
            if ( this.vo.items == null )
            {
                this.vo.items = new ResVerItem [0];
            }
        }

        /// <summary>
        /// 是否有对应资源的版本信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains (string name)
        {
            foreach ( ResVerItem item in vo.items )
            {
                if ( item.name == name )
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 得到资源文件项
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ResVerItem Get (string name)
        {
            foreach ( ResVerItem item in vo.items )
            {
                if ( item.name == name )
                {
                    return item;
                }
            }
            return new ResVerItem ();
        }

        /// <summary>
        /// 得到文件的版本号
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetVer (string name)
        {
            foreach ( ResVerItem item in vo.items )
            {
                if ( item.name == name )
                {
                    return item.version;
                }
            }

            return null;
        }

        /// <summary>
        /// 设置文件版本号
        /// </summary>
        /// <returns>The ver.</returns>
        /// <param name="name">Name.</param>
        public void SetVer (string name , string version)
        {
            //如果存在则更新
            for ( int i = 0 ; i < vo.items.Length ; i++ )
            {
                if ( vo.items [i].name == name )
                {
                    vo.items [i].version = version;
                    return; //添加并返回函数
                }
            }

            //如果不存在则添加
            ResVerItem [] items = new ResVerItem [vo.items.Length + 1];
            Array.Copy (vo.items , items , vo.items.Length);
            ResVerItem newItem = new ResVerItem ();
            newItem.name = name;
            newItem.version = version;
            items [vo.items.Length] = newItem;
            vo.items = items;
        }

        /// <summary>
        /// 移除一个文件的版本号
        /// </summary>
        /// <param name="name"></param>
        public void RemoveVer (string name)
        {
            for ( int i = 0 ; i < vo.items.Length ; i++ )
            {
                if ( vo.items [i].name == name )
                {
                    ResVerItem [] items = new ResVerItem [vo.items.Length - 1];
                    if ( i == 0 )
                    {
                        Array.Copy (vo.items , 1 , items , 0 , items.Length);
                    }
                    else if ( i == items.Length )
                    {
                        Array.Copy (vo.items , 0 , items , 0 , items.Length);
                    }
                    else
                    {
                        //拷贝前部分
                        Array.Copy (vo.items , 0 , items , 0 , i);
                        //拷贝后部分
                        Array.Copy (vo.items , i + 1 , items , i , items.Length - i);
                    }
                    vo.items = items;
                    break;
                }
            }
        }

        /// <summary>
        /// 清除所有的版本信息
        /// </summary>
        public void ClearVer ()
        {
            vo.items = new ResVerItem [0];
        }


        /// <summary>
        /// 查找资源
        /// <para>查找以name字符串为开头的资源，格式可以为 "res/h." 或 "res/ab/" </para>
        /// <para>如果没有以"."或"/"结尾，则会自动查超所有符合[name加上"."或"/"结尾]的文件</para>
        /// <para>输入 "" 或者 "/" 则会返回所有的资源</para>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<ResVerItem> FindGroup (string name)
        {
            if ( null == name )
            {
                return null;
            }

            if ( name == "" || name == "/" )
            {
                List<ResVerItem> totalList = new List<ResVerItem> (vo.items);
                return totalList;
            }

            bool isFuzzy = true;
            string fuzzyName0 = null;
            string fuzzyName1 = null;
            if ( name.EndsWith ("/") || name.Contains (".") )
            {
                isFuzzy = false;
            }

            if ( isFuzzy )
            {
                fuzzyName0 = name + "/";
                fuzzyName1 = name + ".";
            }

            List<ResVerItem> list = new List<ResVerItem> ();
            for ( int i = 0 ; i < vo.items.Length ; i++ )
            {
                if ( isFuzzy )
                {
                    if ( vo.items [i].name.StartsWith (fuzzyName0) || vo.items [i].name.StartsWith (fuzzyName1) || vo.items [i].name == name )
                    {
                        list.Add (vo.items [i]);
                    }
                }
                else
                {
                    if ( vo.items [i].name.StartsWith (name) || vo.items [i].name == name )
                    {
                        list.Add (vo.items [i]);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 和目标比较资源是否版本相同
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsSameVer (string name , ResVerModel target)
        {
            if ( GetVer (name) == target.GetVer (name) )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 校验哈希值
        /// </summary>
        public bool CheckHash ()
        {
            bool isUseful = true;
            foreach ( ResVerItem item in vo.items )
            {
                string filePath = FileSystem.CombinePaths (ResMgr.Ins.InitializeParameters.AssetRoot , item.name);
                string localHash = UtilVerify.Ins.FileCRC32 (filePath);
                if ( !File.Exists (filePath) || localHash != item.version )
                {
                    Debug.LogError ($"{item.name}校验失败,目标结果:{item.version},本地:{localHash}");
                    UniAssetRuntime.Ins.LocalResVer.SetVerAndSave (item.name , "");
                    isUseful = false;
                }
            }
            return isUseful;
        }

        /// <summary>
        /// 找到目标资源中和我方资源不同版本的内容
        /// <para>已方有而对方没有的资源被忽略</para>
        /// <para>对方有而已方没有的资源被包括</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public List<ResVerItem> FindGroupDifferent (string name , ResVerModel target)
        {
            List<ResVerItem> selfGroup = FindGroup (name);
            List<ResVerItem> targetGroup = target.FindGroup (name);

            int targetIdx = targetGroup.Count;
            while ( --targetIdx > -1 )
            {
                ResVerItem targetItem = targetGroup [targetIdx];

                int selfIdx = selfGroup.Count;
                while ( --selfIdx > -1 )
                {
                    ResVerItem selfItem = selfGroup [selfIdx];

                    if ( selfItem.Equals (targetItem) )
                    {
                        //两边都有的对象，从两边的数组移除，减少之后的运算开销
                        targetGroup.RemoveAt (targetIdx);
                        selfGroup.RemoveAt (selfIdx);
                        break;
                    }
                }
            }
            return targetGroup;
        }

        /// <summary>
        /// 是否含有资源文件
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains (ResVerItem item)
        {
            foreach ( ResVerItem selfItem in vo.items )
            {
                if ( selfItem.Equals (item) )
                {
                    return true;
                }
            }
            return false;
        }
    }
}