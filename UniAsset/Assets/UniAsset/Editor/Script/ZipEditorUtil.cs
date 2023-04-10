using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using UniAsset;

namespace UniAssetEditor
{
    /// <summary>
    /// @创建者: 林逸群
    /// @创建日期: 2020/11/8 16:27:4
    /// @功能描述: 压缩的编辑器工具
    /// @修改者：
    /// @修改日期：
    /// @修改描述：
    /// </summary>
    public class ZipHelper4UnityEditor : SafeSingleton<ZipHelper4UnityEditor>
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

        private string _targetDir;

        public void ZipFile (string fileToZip , string filePathInZip , string zipedFile , int compressionLevel)
        {
            if ( !System.IO.File.Exists (fileToZip) )
            {
                throw new System.IO.FileNotFoundException ("file not exist: " + fileToZip);
            }

            using ( System.IO.FileStream zipFile = System.IO.File.Create (zipedFile) )
            {
                using ( ZipOutputStream zipStream = new ZipOutputStream (zipFile) )
                {
                    using ( System.IO.FileStream streamToZip = new System.IO.FileStream (fileToZip , System.IO.FileMode.Open , System.IO.FileAccess.Read) )
                    {
                        ZipEntry zipEntry = new ZipEntry (filePathInZip);

                        zipStream.PutNextEntry (zipEntry);

                        zipStream.SetLevel (compressionLevel);

                        byte [] buffer = new byte [4096];

                        int sizeRead = 0;

                        try
                        {
                            do
                            {
                                sizeRead = streamToZip.Read (buffer , 0 , buffer.Length);
                                zipStream.Write (buffer , 0 , sizeRead);
                            }
                            while ( sizeRead > 0 );
                        }
                        catch ( System.Exception ex )
                        {
                            throw ex;
                        }

                        streamToZip.Close ();
                    }

                    zipStream.Finish ();
                    zipStream.Close ();
                }

                zipFile.Close ();
            }
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="targetDir">压缩文件夹路径</param>
        /// <param name="zipFile">压缩文件夹保存位置</param>
        /// <param name="whiteExtList">扩展名白名单，如果不为null，则只压缩白名单中指定后缀的文件</param>
        public void ZipDir (string targetDir , string zipFile , string [] whiteExtList = null)
        {
            _targetDir = targetDir;

            try
            {
                ZipOutputStream s = new ZipOutputStream (File.Create (zipFile));
                s.SetLevel (9);
                byte [] buffer = new byte [4096];
                AddEntrys (s , buffer , targetDir , whiteExtList);
                s.Finish ();
                s.Close ();
            }
            catch ( Exception e )
            {
                Error = e.Message;
            }

            IsDone = true;
            Progress = 1f;
        }

        void AddEntrys (ZipOutputStream s , byte [] buffer , string dir , string [] whiteExtList)
        {
            string [] filenames = Directory.GetFiles (dir);
            string [] dirs = Directory.GetDirectories (dir);

            //处理文件
            foreach ( string file in filenames )
            {
                #region 扩展名白名单校验
                if ( null != whiteExtList )
                {
                    string ext = Path.GetExtension (file);

                    bool isExtInWhiteList = false;
                    for ( int i = 0 ; i < whiteExtList.Length ; i++ )
                    {
                        if ( whiteExtList [i] == ext )
                        {
                            isExtInWhiteList = true;
                            break;
                        }
                    }

                    if ( false == isExtInWhiteList )
                    {
                        continue;
                    }
                }
                #endregion

                string saveFile = file.Replace (_targetDir , "");
                saveFile = saveFile.Replace ("\\" , "/");
                ZipEntry entry = new ZipEntry (saveFile);
                entry.DateTime = DateTime.Now;
                s.PutNextEntry (entry);

                using ( FileStream fs = File.OpenRead (file) )
                {
                    int sourceBytes;
                    do
                    {
                        sourceBytes = fs.Read (buffer , 0 , buffer.Length);
                        s.Write (buffer , 0 , sourceBytes);
                    } while ( sourceBytes > 0 );
                }
            }

            //处理文件夹
            foreach ( string subDir in dirs )
            {
                AddEntrys (s , buffer , subDir , whiteExtList);
            }
        }
    }
}