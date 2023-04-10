using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UniAsset;

namespace UniAssetEditor
{
    class AssetBundleSettingVo
    {
        [LabelText ("客户端版本")]
        public string appVer = "1.0";

        [LabelText ("资源包版本")]
        public string resPackageVer = "1.0";
    }

    class BuildAssetBundelEditorWin : OdinEditorWindow
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open ()
        {
            var win = GetWindow<BuildAssetBundelEditorWin> ("构建AssetBundle" , true);
            var rect = GUIHelper.GetEditorWindowRect ().AlignCenter (300 , 600);
            win.position = rect;
        }

        protected override void OnEnable ()
        {
            base.OnEnable ();
        }

        [LabelText ("版本设置")]
        [HideReferenceObjectPicker]
        public AssetBundleSettingVo vo = new AssetBundleSettingVo ();

        [Button ("Build" , buttonSize: ButtonSizes.Medium), PropertyOrder (-1)]
        void Build ()
        {
            new AssetBundleBuildCommand (UniAssetConst.PUBLISH_RES_ROOT_DIR , vo).Execute ();
            new ResJsonBuildCommand (UniAssetConst.PUBLISH_RES_ROOT_DIR , vo).Execute ();
            EditorMenu.GenerateAssetBundleName ();
            EditorUtil.OpenDirectory (UniAssetConst.PUBLISH_RES_ROOT_DIR);
        }

        [Button ("OpenAssetBundleDir" , buttonSize: ButtonSizes.Medium), PropertyOrder (-1)]
        void OpenAssetBundleDir ()
        {
            EditorUtil.OpenDirectory (UniAssetConst.PUBLISH_RES_ROOT_DIR);
        }
    }
}