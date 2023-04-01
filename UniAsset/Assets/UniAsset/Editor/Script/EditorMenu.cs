using UniAsset;
using UniAssetEditor;
using UnityEditor;

#pragma warning disable IDE0051
#pragma warning disable IDE0052
namespace ZeroEditor
{
    /// <summary>
    /// 框架编辑器菜单
    /// </summary>
    public class EditorMenu
    {
        [MenuItem ("UniAsset/Build AssetBundle" , false , 50)]
        public static void HotResBuild ()
        {
            new AssetBundleBuildCommand (UniAssetConst.PUBLISH_RES_ROOT_DIR).Execute ();
            EditorUtil.OpenDirectory (FileSystem.CombineDirs (false , UniAssetConst.PUBLISH_RES_ROOT_DIR));
        }

        [MenuItem ("UniAsset/GenerateAssetBundleName" , false , 60)]
        public static void GenerateAssetBundleName ()
        {
            new GenerateAssetBundleNameModule ().Start ();
        }
    }
}