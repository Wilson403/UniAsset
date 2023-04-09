using Sirenix.OdinInspector;
using UnityEditor;

namespace UniAssetEditor
{
    public class AEditorModule
    {
        /// <summary>
        /// 关联的编辑器窗口
        /// </summary>
        [HideInEditorMode]
        public readonly EditorWindow editorWin;

        public AEditorModule (EditorWindow editorWin)
        {
            this.editorWin = editorWin;
        }

        /// <summary>
        /// 显示一个Tip信息
        /// </summary>
        /// <param name="content"></param>
        public void ShowTip (string content)
        {
            editorWin.ShowNotification (new UnityEngine.GUIContent (content));
        }
    }
}