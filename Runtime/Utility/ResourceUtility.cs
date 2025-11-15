#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NBC.ActionEditor
{
    internal class ResourceUtility
    {
        internal static T FindAssetsWithPath<T>(string resPath, ref T result) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(resPath))
            {
                result = null;
                return null;
            }

            if (result == null)
            {
#if UNITY_EDITOR
                result = AssetDatabase.LoadAssetAtPath<T>(resPath);
#endif
            }

            return result;
        }
    }
}