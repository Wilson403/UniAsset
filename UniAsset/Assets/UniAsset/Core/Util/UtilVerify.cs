using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace UniAsset
{
    public class UtilVerify : SafeSingleton<UtilVerify>
    {
        /// <summary>
        /// 获取字符串的CRC32
        /// </summary>
        public string StringCRC32 (string str)
        {
            byte [] buffer = Encoding.UTF8.GetBytes (str);
            return BytesCRC32 (buffer);
        }

        /// <summary>
        /// 获取文件的CRC32
        /// </summary>
        public string FileCRC32 (string filePath)
        {
            if ( !File.Exists (filePath) )
            {
                return "";
            }

            try
            {
                using ( FileStream fs = new FileStream (filePath , FileMode.Open , FileAccess.Read , FileShare.Read) )
                {
                    return StreamCRC32 (fs);
                }
            }
            catch ( Exception e )
            {
                Debug.LogError (e);
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取数据流的CRC32
        /// </summary>
        public string StreamCRC32 (Stream stream)
        {
            CRC32Algorithm hash = new CRC32Algorithm ();
            byte [] hashBytes = hash.ComputeHash (stream);
            return ToString (hashBytes);
        }

        /// <summary>
        /// 获取字节数组的CRC32
        /// </summary>
        public string BytesCRC32 (byte [] buffer)
        {
            CRC32Algorithm hash = new CRC32Algorithm ();
            byte [] hashBytes = hash.ComputeHash (buffer);
            return ToString (hashBytes);
        }

        /// <summary>
        /// 判断字符串是不是数字类型
        /// </summary>
        /// <param name="str">输入的字符串</param>
        /// <returns></returns>
        public bool IsNumeric (string str)
        {
            if ( str == null || str.Length == 0 ) return false;
            for ( int i = 0 ; i < str.Length ; i++ )
            {
                return char.IsNumber (str [i]);
            }
            return true;
        }

        /// <summary>
        /// 网络可用
        /// </summary>
        public bool NetAvailable
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        /// <summary>
        /// 是否是无线
        /// </summary>
        public bool IsWifi
        {
            get
            {
                return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
            }
        }

        private string ToString (byte [] hashBytes)
        {
            string result = BitConverter.ToString (hashBytes);
            result = result.Replace ("-" , "");
            return result.ToLower ();
        }
    }
}