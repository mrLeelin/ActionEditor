using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public static class ScriptLocator
    {
        private static Dictionary<string, string> _cache = new Dictionary<string, string>();
        private static bool _cacheInitialized = false;


        public static void TryOpenScriptPath(Type targetType, bool useCache = true)
        {
            var path = FindScriptPath(targetType.FullName, true);
            if (!string.IsNullOrEmpty(path))
            {
                //打开脚本
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                AssetDatabase.OpenAsset(script);
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "未找到脚本.已复制到剪贴板", "确定");
            }
        }

        public static string FindScriptPath(string typeFullName, bool useCache = true)
        {
            if (string.IsNullOrEmpty(typeFullName))
            {
                Debug.LogWarning("Type full name is null or empty.");
                return null;
            }

            if (useCache && _cache.TryGetValue(typeFullName, out string cachedPath))
            {
                return cachedPath;
            }

            Type targetType = FindType(typeFullName);
            if (targetType == null)
            {
                Debug.LogWarning($"Type '{typeFullName}' not found in any loaded assemblies.");
                return null;
            }

            string scriptPath = FindScriptByType(targetType);

            if (string.IsNullOrEmpty(scriptPath))
            {
                scriptPath = FindScriptBySearching(typeFullName);
            }

            if (!string.IsNullOrEmpty(scriptPath) && useCache)
            {
                _cache[typeFullName] = scriptPath;
            }

            if (string.IsNullOrEmpty(scriptPath))
            {
                Debug.LogWarning($"Script file not found for type '{typeFullName}'.");
            }

            return scriptPath;
        }

        public static void RebuildCache()
        {
            var startTime = EditorApplication.timeSinceStartup;
            _cache.Clear();

            MonoScript[] allScripts = MonoImporter.GetAllRuntimeMonoScripts();

            foreach (var script in allScripts)
            {
                Type scriptType = script.GetClass();
                if (scriptType != null)
                {
                    string path = AssetDatabase.GetAssetPath(script);
                    _cache[scriptType.FullName] = path;
                }
            }

            _cacheInitialized = true;
            var elapsed = (EditorApplication.timeSinceStartup - startTime) * 1000;
            Debug.Log($"Script cache rebuilt: {_cache.Count} types cached in {elapsed:F2}ms");
        }

        public static void ClearCache()
        {
            _cache.Clear();
            _cacheInitialized = false;
        }

        private static string FindScriptByType(Type targetType)
        {
            MonoScript[] allScripts = MonoImporter.GetAllRuntimeMonoScripts();
            MonoScript foundScript = allScripts.FirstOrDefault(script => script.GetClass() == targetType);

            if (foundScript != null)
            {
                return AssetDatabase.GetAssetPath(foundScript);
            }

            return null;
        }

        private static string FindScriptBySearching(string typeFullName)
        {
            string className = typeFullName.Contains(".")
                ? typeFullName.Split('.').Last()
                : typeFullName;

            string[] guids = AssetDatabase.FindAssets($"t:MonoScript {className}");

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!assetPath.EndsWith(".cs")) continue;

                string fileContent = File.ReadAllText(assetPath);

                if (ContainsClassDefinition(fileContent, typeFullName, className))
                {
                    return assetPath;
                }
            }

            return null;
        }

        private static bool ContainsClassDefinition(string content, string fullName, string className)
        {
            string classPattern = $@"\bclass\s+{Regex.Escape(className)}\b";
            string structPattern = $@"\bstruct\s+{Regex.Escape(className)}\b";

            if (Regex.IsMatch(content, classPattern) || Regex.IsMatch(content, structPattern))
            {
                if (fullName.Contains("."))
                {
                    string namespaceName = fullName.Substring(0, fullName.LastIndexOf('.'));
                    string namespacePattern = $@"\bnamespace\s+{Regex.Escape(namespaceName)}\b";
                    return Regex.IsMatch(content, namespacePattern);
                }

                return true;
            }

            return false;
        }

        private static Type FindType(string fullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}