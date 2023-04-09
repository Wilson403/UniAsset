using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

namespace UniAssetEditor
{
    class UniAssetSettingEditorWin : OdinMenuEditorWindow
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open ()
        {
            var win = GetWindow<UniAssetSettingEditorWin> ("UniAsset设置界面" , true);
            var rect = GUIHelper.GetEditorWindowRect ().AlignCenter (1000 , 600);
            win.position = rect;
        }

        protected override void OnEnable ()
        {
            base.OnEnable ();
        }

        protected override OdinMenuTree BuildMenuTree ()
        {
            OdinMenuTree tree = new OdinMenuTree ();
            tree.Config.DrawSearchToolbar = true;
            tree.Add ("发布Setting描述文件" , new BuildSettingJsonModule (this));
            return tree;
        }
    }
}