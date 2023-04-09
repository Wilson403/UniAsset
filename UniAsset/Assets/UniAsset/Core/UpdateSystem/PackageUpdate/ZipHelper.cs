using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace UniAsset
{
    /// <summary>
    /// Zip解压助手
    /// </summary>
    public class ZipHelper
    {
        /// <summary>
        /// 进度
        /// </summary>
        public float Progress { get; private set; } = 0f;

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsDone { get; private set; } = false;

        /// <summary>
        /// 错误内容
        /// </summary>
        public string Error { get; private set; } = null;

        /// <summary>
        /// 已解压的字节数
        /// </summary>
        public long DecompessSize { get; private set; } = 0;

        /// <summary>
        /// 总字节数
        /// </summary>
        public long TotalSize { get; private set; } = 0;

        string _zipFile;
        string _targetDir;
        byte [] _zipBytes;


        public ZipHelper ()
        {

        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="zipFile">压缩文件路径</param>
        /// <param name="targetDir">解压目录</param>
        public void UnZip (string zipFile , string targetDir)
        {
            _zipFile = zipFile;
            UnZip (targetDir);
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="stream">压缩文件流</param>
        /// <param name="targetDir">解压目录</param>
        void UnZip (string targetDir)
        {
            _targetDir = targetDir;
            ZipConstants.DefaultCodePage = 0;
            ZipConstants.DefaultCodePage = System.Text.Encoding.GetEncoding ("gbk").CodePage; //解决中文乱码问题
            Thread thread = new Thread (new ThreadStart (ProcessUnZip));
            thread.Start ();
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="bytes">二进制数据</param>
        /// <param name="targetDir">解压目录</param>
        public void UnZip (byte [] bytes , string targetDir)
        {
            _zipBytes = bytes;
            UnZip (targetDir);
        }

        Stream GetNewStream ()
        {
            if ( _zipBytes != null )
            {
                return new MemoryStream (_zipBytes);
            }
            else if ( _zipFile != null )
            {
                return File.OpenRead (_zipFile);
            }
            return null;
        }

        void ProcessUnZip ()
        {
            try
            {
                //第一次打开 获取文件总数
                ZipInputStream s = new ZipInputStream (GetNewStream ());
                List<ZipEntry> entryList = new List<ZipEntry> ();
                long totalSize = 0;
                ZipEntry entry;
                while ( ( entry = s.GetNextEntry () ) != null )
                {
                    if ( entry.IsFile )
                    {
                        entryList.Add (entry);
                        totalSize += entry.Size;
                    }
                }
                TotalSize = totalSize;
                DecompessSize = 0;

                long total = entryList.Count;
                long current = 0;
                entryList.Clear ();
                s.Close ();

                //创建LUA脚本目录
                if ( false == Directory.Exists (_targetDir) )
                {
                    Directory.CreateDirectory (_targetDir);
                }

                //第二次打开 
                s = new ZipInputStream (GetNewStream ());

                while ( ( entry = s.GetNextEntry () ) != null )
                {
                    string targetPath = FileSystem.CombinePaths (_targetDir , entry.Name);

                    if ( entry.IsDirectory )
                    {
                        Directory.CreateDirectory (targetPath);
                    }
                    else if ( entry.IsFile )
                    {
                        string dirName = Path.GetDirectoryName (targetPath);
                        if ( false == Directory.Exists (dirName) )
                        {
                            Directory.CreateDirectory (dirName);
                        }

                        FileStream fs = File.Create (targetPath);
                        int size = 2048;
                        byte [] data = new byte [2048];
                        while ( true )
                        {
                            size = s.Read (data , 0 , data.Length);
                            if ( size > 0 )
                            {
                                fs.Write (data , 0 , size);
                                DecompessSize += size;
                            }
                            else
                            {
                                fs.Close ();
                                break;
                            }
                        }
                        Progress = ++current / ( float ) total;
                        Thread.Sleep (1);
                    }
                }

                s.Close ();
            }
            catch ( Exception e )
            {
                Error = e.Message;
            }

            IsDone = true;
            Progress = 1f;
        }
    }
}