using System.Collections.Generic;
using System.IO;
using UniAsset;
using UnityEditor;
using UnityEngine;

namespace UniAssetEditor
{
    /// <summary>
    /// 版本号文件生成命令。针对目标目录中的所有文件生成一个res.json文件，描述其MD5以及文件大小
    /// </summary>
    class ResJsonBuildCommand
    {
        readonly string _sourceDir;
        readonly ResVerVo _res;
        List<string> _files;
        AssetBundleSettingVo _assetBundeBundleSettingVo;

        public ResJsonBuildCommand (string sourceDir , AssetBundleSettingVo assetBundeBundleSettingVo)
        {
            _assetBundeBundleSettingVo = assetBundeBundleSettingVo;
            _sourceDir = FileSystem.CombineDirs (true , sourceDir , _assetBundeBundleSettingVo.appVer , _assetBundeBundleSettingVo.resPackageVer , UniAssetConst.AB_DIR_NAME);
            _res = new ResVerVo ();
        }

        public void Execute ()
        {
            if ( false == Directory.Exists (_sourceDir) )
            {
                EditorUtility.DisplayDialog ("错误" , "目标目录并不存在: " + _sourceDir , "确定");
                return;
            }


            var filePath = FileSystem.CombinePaths (_sourceDir.Replace (UniAssetConst.AB_DIR_NAME , "") , UniAssetConst.RES_JSON_FILE_NAME);

            //首先删除旧的
            if ( File.Exists (filePath) )
            {
                File.Delete (filePath);
            }

            BuildResVerVo ();
            File.WriteAllText (filePath , LitJson.JsonMapper.ToPrettyJson (_res));
        }

        void BuildResVerVo ()
        {
            _files = new List<string> ();

            EditorUtility.DisplayProgressBar ("正在生成 " + UniAssetConst.RES_JSON_FILE_NAME , "即将开始" , 0f);

            _files.Clear ();

            ScanningFiles (_sourceDir);

            List<ResVerItem> items = new List<ResVerItem> ();
            for ( int i = 0 ; i < _files.Count ; i++ )
            {
                var file = _files [i];
                var fileName = Path.GetFileName (file);
                if ( fileName.Equals (UniAssetConst.SETTING_FILE_NAME) )
                {
                    Debug.Log ($"文件{fileName}不写入描述");
                    continue;
                }
                EditorUtility.DisplayProgressBar ("正在生成 " + UniAssetConst.RES_JSON_FILE_NAME , string.Format ("文件:{0}" , file) , ( ( float ) i / items.Count ));
                FileInfo fi = new FileInfo (file);
                ResVerItem item = new ResVerItem
                {
                    name = file.Replace (_sourceDir , "").Replace ("\\" , "/") ,
                    version = UtilVerify.Ins.FileCRC32 (file) ,
                    size = fi.Length
                };
                items.Add (item);
            }
            _res.items = items.ToArray ();
            _res.resPackageVer = _assetBundeBundleSettingVo.resPackageVer;
            EditorUtility.ClearProgressBar ();
        }

        void ScanningFiles (string dir)
        {
            string [] files = Directory.GetFiles (dir);
            _files.AddRange (files);

            string [] subDirs = Directory.GetDirectories (dir);
            foreach ( var subDir in subDirs )
            {
                //是目录
                ScanningFiles (subDir);
            }
        }
    }
}