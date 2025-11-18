using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public static class SafeObjectField
    {
        /// <summary>
        /// 安全的 ObjectField，不会因为按下 Delete 键而清空
        /// </summary>
        public static T Draw<T>(string label, T current, bool allowSceneObjects) where T : Object
        {
            // 在绘制前拦截 Delete
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
            {
                Event.current.Use(); // 阻止传递到 ObjectField 内部
            }
            
            EditorGUILayout.BeginHorizontal();
            var labelSize = GUI.skin.label.CalcSize(new GUIContent(label));
            GUILayout.Label(label, GUILayout.Width(labelSize.x));
            var myObj = (T)EditorGUILayout.ObjectField(current, typeof(T), allowSceneObjects,GUILayout.MaxWidth(200));
            EditorGUILayout.EndHorizontal();
            return myObj;
        }
    }

}

