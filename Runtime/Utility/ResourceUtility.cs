using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NBC.ActionEditor
{
    internal class ResourceUtility
    {
#if UNITY_EDITOR
        private static readonly Dictionary<string, UnityEngine.Object> CacheObject =
            new Dictionary<string, UnityEngine.Object>();
#endif


        internal static T FindAssetsWithPath<T>(string resPath) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(resPath))
            {
                return null;
            }
#if UNITY_EDITOR
            if (CacheObject.TryGetValue(resPath, out var result))
            {
                return result as T;
            }
            else
            {
                result = AssetDatabase.LoadAssetAtPath<T>(resPath);
                CacheObject.Add(resPath, result);
                return result as T;
            }
#endif
            return null;
        }
    }
}