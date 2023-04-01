using System.IO;
using UnityEngine;

namespace UniAsset
{
    public class UniAssetRuntime : ASingletonMonoBehaviour<UniAssetRuntime>
    {
        public string LocalResDir { get; private set; }
        public bool IsLoadAssetsByAssetDataBase { get; private set; }
        public bool IsLoadAssetsFromNet { get; private set; }

        public void Init ()
        {
            switch ( Application.platform )
            {
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.OSXEditor:
                    if ( IsLoadAssetsFromNet )
                    {
                        LocalResDir = UniAssetConst.WWW_RES_PERSISTENT_DATA_PATH;
                    }
                    else
                    {
                        //编辑器下
                        if ( Application.platform == RuntimePlatform.WindowsEditor ||
                             Application.platform == RuntimePlatform.OSXEditor ||
                             Application.platform == RuntimePlatform.LinuxEditor )
                        {
                            LocalResDir = UniAssetConst.PUBLISH_RES_ROOT_DIR;
                        }
                        else
                        {
                            LocalResDir = FileSystem.CombineDirs (false , UniAssetConst.PERSISTENT_DATA_PATH , UniAssetConst.UNIASSET_LIBRARY_DIR , "Release" , "res" , UniAssetConst.PLATFORM_DIR_NAME);
                        }
                    }
                    break;
                default:
                    throw new System.Exception (string.Format ("暂时不支持平台：{0}" , Application.platform));
            }

            //确保本地资源目录存在
            if ( false == Directory.Exists (LocalResDir) )
            {
                Directory.CreateDirectory (LocalResDir);
            }
            Debug.Log ($"Local Res Dir: {LocalResDir}");
        }
    }
}