using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class RenamePopup : EditorWindow
    {
        private string _currentName;
        private string _newName;
        private System.Action<string> _onConfirm;

        public void Initialize(string currentName, System.Action<string> onConfirm)
        {
            _currentName = currentName;
            _newName = currentName;
            _onConfirm = onConfirm;
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("输入新名称:", EditorStyles.boldLabel);
            GUILayout.Space(5);

            GUI.SetNextControlName("RenameField");
            _newName = EditorGUILayout.TextField(_newName);

            if (Event.current.type == EventType.Repaint && string.IsNullOrEmpty(GUI.GetNameOfFocusedControl()))
            {
                EditorGUI.FocusTextInControl("RenameField");
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("确定", GUILayout.Width(80)) ||
                (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return))
            {
                _onConfirm?.Invoke(_newName);
                Close();
                Event.current.Use();
            }

            if (GUILayout.Button("取消", GUILayout.Width(80)) ||
                (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape))
            {
                Close();
                Event.current.Use();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
