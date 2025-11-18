using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public static class HierarchyTools
    {
        /// <summary>
        /// 在Hierarchy中展开GameObject
        /// </summary>
        /// <param name="go"></param>
        /// <param name="expand"></param>
        public static void SetExpandedRecursive(GameObject go, bool expand)
        {
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            var methodInfo = type.GetMethod("SetExpandedRecursive");
	
            // This differs in unity versions.
            // Old version should be "Window/Hierarchy."
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
	
            var window = EditorWindow.focusedWindow;
	
            methodInfo.Invoke(window, new object[] { go.GetInstanceID(), expand });
        }
    }
}