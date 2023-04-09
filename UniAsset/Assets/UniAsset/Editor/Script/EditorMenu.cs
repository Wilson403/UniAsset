using UniAsset;
using UnityEditor;

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
            new AssetBundleBuildCommand (UniAssetConst.PUBLISH_ASSETBUNDLE_DIR).Execute ();
            GenerateAssetBundleName ();
            EditorUtil.OpenDirectory (FileSystem.CombineDirs (false , UniAssetConst.PUBLISH_RES_ROOT_DIR));
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
    }
}