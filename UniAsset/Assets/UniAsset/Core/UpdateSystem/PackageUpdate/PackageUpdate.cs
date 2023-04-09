using System;
using System.Collections;
using System.IO;
using Primise4CSharp;
using UnityEngine;

namespace UniAsset
{
    /// <summary>
    /// 内嵌资源包解压
    /// 以文件流的形式进行解压
    /// </summary>
    public class PackageUpdate
    {
        private Promise _onLoad = new Promise ();

        public Promise Start (Action<float , long> onProgress)
        {
            Debug.Log ("「PackageUpdate」内嵌资源解压检查...");
            UniAssetRuntime.Ins.StartCoroutine (Run (onProgress));
            return _onLoad;
        }

        IEnumerator Run (Action<float , long> onProgress)
        {
            do
            {
                //检查程序是否第一次启动
                if ( UniAssetRuntime.Ins.LocalData.IsInit )
                {
                    break;
                }

                string packageZipFilePath = FileSystem.CombinePaths (UniAssetConst.STREAMING_ASSETS_PATH , UniAssetConst.PACKAGE_ZIP_FILE_NAME);

                //如果是Android真机环境下，需要用android原生代码将文件拷贝到可读写目录，再进行操作
                if ( Application.platform == RuntimePlatform.Android )
                {
                    string androidPackageZipTempPath = FileSystem.CombinePaths (UniAssetConst.PERSISTENT_DATA_PATH , UniAssetConst.PACKAGE_ZIP_FILE_NAME);

                    Debug.LogFormat ("压缩文件：{0}" , packageZipFilePath);
                    Debug.LogFormat ("临时压缩文件：{0}" , androidPackageZipTempPath);

                    //请求Android原生代码，用文件流形式复制文件到临时位置
                    AndroidJavaObject javaAssetFileCopy = new AndroidJavaObject ("pieces.jing.zerolib.file.AssetFileCopy");
                    bool isRequestSuccess = javaAssetFileCopy.Call<bool> ("copyAssetsFile" , "package.zip" , androidPackageZipTempPath);
                    if ( isRequestSuccess )
                    {
                        while ( false == javaAssetFileCopy.Call<bool> ("isDone") )
                        {
                            yield return new WaitForEndOfFrame ();
                        }

                        string error = javaAssetFileCopy.Call<string> ("error");
                        if ( error != null )
                        {
                            Debug.LogFormat ("copyAssetsFile出现问题：" + error);
                        }
                        else
                        {
                            packageZipFilePath = androidPackageZipTempPath;
                        }
                    }
                }

                //检查是否存在Package.zip
                if ( !File.Exists (packageZipFilePath) )
                {
                    Debug.LogWarningFormat ("解压的文件[{0}]不存在" , packageZipFilePath);
                    break;
                }

                Debug.Log ($"压缩文件：{packageZipFilePath}");
                Debug.Log ($"解压目录：{UniAssetRuntime.Ins.ResInitializeParameters.AssetRoot}");

                //解压Zip
                ZipHelper zh = new ZipHelper ();

                //将文件解压到可读写目录中
                zh.UnZip (packageZipFilePath , UniAssetRuntime.Ins.ResInitializeParameters.AssetRoot);

                while ( false == zh.IsDone )
                {
                    onProgress?.Invoke (zh.DecompessSize , zh.TotalSize);
                    yield return new WaitForEndOfFrame ();
                }

                if ( zh.Error != null )
                {
                    Debug.LogFormat ("解压出错：\n{0}" , zh.Error);
                }
                else
                {
                    Debug.LogFormat ("[{0}]解压完成" , UniAssetConst.PACKAGE_ZIP_FILE_NAME);
                    UniAssetRuntime.Ins.LocalData.IsInit = true;

                    if ( Application.platform == RuntimePlatform.Android && File.Exists (packageZipFilePath) )
                    {
                        File.Delete (packageZipFilePath);
                        Debug.LogFormat ("删除临时解压包[{0}]" , UniAssetConst.PACKAGE_ZIP_FILE_NAME);
                    }
                }

                //重新加载一次版本号文件，因为可能被覆盖了
                UniAssetRuntime.Ins.LocalResVer.CreateLocalResVerFile ();
                UniAssetRuntime.Ins.LocalResVer.Load ();
            }
            while ( false );
            _onLoad.Resolve ();
            yield break;
        }
    }
}