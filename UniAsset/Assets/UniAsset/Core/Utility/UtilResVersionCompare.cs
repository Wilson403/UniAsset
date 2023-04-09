using System.Text;
using System;
using UnityEngine;

namespace UniAsset
{
    public class UtilResVersionCompare : SafeSingleton<UtilResVersionCompare>
    {
        /// <summary>
        /// 比较APP主版本
        /// 同网络上的比较
        /// </summary>
        /// <returns></returns>
        public int CompareAppMainVersion ()
        {
            int localmainVer = GetAppMainVersionNum (); //本地主版本号
            int netMainVer; //网络主版本号
            int result; //对比结果

            //尝试解析网络上的主版本号
            try
            {
                netMainVer = int.Parse (UniAssetRuntime.Ins.Setting.client.version);
            }
            catch ( Exception e )
            {
                netMainVer = 0;
                Debug.Log ($"网络app主版号{UniAssetRuntime.Ins.Setting.client.version}解析错误: {e}");
            }

            //若本地大于网络的
            if ( localmainVer > netMainVer )
            {
                result = 1;
            }
            //若本地小于网络的
            else if ( localmainVer < netMainVer )
            {
                result = -1;
            }
            //两者相等
            else
            {
                result = 0;
            }

            Debug.Log ($"网络app主版号：{UniAssetRuntime.Ins.Setting.client.version}");

            return result;
        }

        /// <summary>
        /// 比较APP资源版本
        /// </summary>
        /// <returns></returns>
        public int CompareAppResVersion ()
        {
            //信息来源有问题就直接返回0
            if ( UniAssetRuntime.Ins.Setting.resPackageVerDict == null )
            {
                return 0;
            }

            UniAssetRuntime.Ins.Setting.resPackageVerDict.TryGetValue (GetAppMainVersion () , out ClientResVerVo clientResVerVo);
            clientResVerVo.currentVer = string.IsNullOrEmpty (clientResVerVo.currentVer) ? "0" : clientResVerVo.currentVer;
            string localResPackageVer = UniAssetRuntime.Ins.LocalResVer.VO.resPackageVer;
            localResPackageVer = string.IsNullOrEmpty (localResPackageVer) ? "0" : localResPackageVer;
            Debug.Log ($"网络资源号：{clientResVerVo.currentVer},本地资源号：{localResPackageVer}");
            return CheckVersionCode (localResPackageVer , clientResVerVo.currentVer);
        }

        /// <summary>
        /// 获取应用程序的主版本字符串
        /// </summary>
        /// <returns></returns>
        public string GetAppMainVersion ()
        {
            string [] versionArr = Application.version.Split ('-');
            if ( versionArr != null && versionArr.Length > 0 )
            {
                return versionArr [0];
            }
            return "";
        }

        /// <summary>
        /// 获取应用程序的主版本号
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetAppMainVersionNum ()
        {
            int mainVerNumber = 0;
            string mainVer = GetAppMainVersion ();
            if ( !string.IsNullOrEmpty (mainVer) )
            {
                try
                {
                    string [] mainVerArr = mainVer.Split ('.');
                    StringBuilder sb = new StringBuilder ();
                    foreach ( string item in mainVerArr )
                    {
                        sb.Append (item);
                    }
                    mainVerNumber = int.Parse (sb.ToString ());
                }
                catch ( Exception e )
                {
                    throw new Exception ($"应用程序的主版本号获取失败: {e}");
                }
            }
            else
            {
                throw new Exception ("无效的版本参数，应用程序的主版本号获取失败");
            }
            return mainVerNumber;
        }

        /// <summary>
        /// 检查版本编码，如果编码1大于编码2，则返回1，等于返回0，小于返回-1
        /// </summary>
        /// <param name="code1"></param>
        /// <param name="code2"></param>
        /// <param name="isIgnoreLength"></param>
        /// <returns></returns>
        public int CheckVersionCode (string code1 , string code2 , bool isIgnoreLength = false)
        {
            string [] locals = code1.Split (new char [] { '.' } , StringSplitOptions.RemoveEmptyEntries);
            string [] nets = code2.Split (new char [] { '.' } , StringSplitOptions.RemoveEmptyEntries);

            //长度检查
            if ( !isIgnoreLength && locals.Length != nets.Length )
            {
                return -1;
            }

            for ( int i = 0 ; i < locals.Length ; i++ )
            {
                int lc = int.Parse (locals [i]);
                int nc = int.Parse (nets [i]);

                if ( lc > nc )
                {
                    return 1;
                }
                else if ( lc < nc )
                {
                    return -1;
                }
            }

            return 0;
        }

        /// <summary>
        /// 版本数字转字符串
        /// </summary>
        /// <param name="ver"></param>
        /// <returns></returns>
        public string VerNumToString (string ver)
        {
            StringBuilder sb = new StringBuilder ();
            for ( int i = 0 ; i < ver.Length ; i++ )
            {
                sb.Append (ver [i]);
                if ( i != ver.Length - 1 )
                {
                    sb.Append ('.');
                }
            }
            return sb.ToString ();
        }
    }
}