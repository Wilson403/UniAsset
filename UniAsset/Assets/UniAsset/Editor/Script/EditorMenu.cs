using System.IO;
using UniAsset;
using UnityEditor;
using UnityEngine;

#pragma warning disable IDE0051
#pragma warning disable IDE0052
namespace UniAssetEditor
{
    /// <summary>
    /// 框架编辑器菜单
    /// </summary>
    public class EditorMenu
    {
        [MenuItem ("UniAsset/Build AssetBundle" , false , 50)]
        public static void HotResBuild ()
        {
            BuildAssetBundelEditorWin.Open ();
        }

        [MenuItem ("UniAsset/GenerateAssetBundleName" , false , 60)]
        public static void GenerateAssetBundleName ()
        {
            new GenerateAssetBundleNameModule ().Start ();
        }

        [MenuItem ("UniAsset/Setting" , false , 70)]
        public static void OpenUniAssetSettingEditorWin ()
        {
            UniAssetSettingEditorWin.Open ();
        }

        [MenuItem ("UniAsset/Create ZIP to StreamingAssets" , false , 80)]
        public static void CreateZIP ()
        {
            var resTargetDir = Path.Combine (Application.dataPath.Replace ("Assets" , "") , UniAssetConst.PUBLISH_RES_ROOT_DIR + "/");
            if ( !Directory.Exists (resTargetDir) )
            {
                Debug.LogError ($"资源目录不存在: {resTargetDir}");
                return;
            }

            var savePath = Path.Combine (Application.streamingAssetsPath , UniAssetConst.PACKAGE_ZIP_FILE_NAME);
            if ( !Directory.Exists (Application.streamingAssetsPath) )
            {
                Directory.CreateDirectory (Application.streamingAssetsPath);
            }
            ZipHelper4UnityEditor.Ins.ZipDir (resTargetDir , savePath);
            Debug.Log ($"资源文件压缩完毕[{savePath}]");
            AssetDatabase.Refresh ();
        }
    }
}