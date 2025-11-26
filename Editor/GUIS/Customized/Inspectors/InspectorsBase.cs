using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NBC.ActionEditor
{
    public class InspectorsBase
    {
        protected object target;

        private Dictionary<int, bool> _unfoldDictionary = new Dictionary<int, bool>();
        private Dictionary<MethodInfo, object[]> methodParamCache = new Dictionary<MethodInfo, object[]>();


        public void SetTarget(object t)
        {
            target = t;
            _unfoldDictionary.Clear();
        }

        public virtual void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

        public void DrawDefaultInspector()
        {
            DrawDefaultInspector(target);
            DrawDefaultInspectorMethod(target);
        }

        private void DrawDefaultInspectorMethod(object obj)
        {
            var t = obj.GetType();
            var methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes();
                foreach (var attribute in attributes)
                {
                    if (attribute is ButtonAttribute buttonAttribute)
                    {
                        string buttonText = string.IsNullOrEmpty(buttonAttribute.ButtonName)
                            ? method.Name
                            : buttonAttribute.ButtonName;

                        ParameterInfo[] parameters = method.GetParameters();

                        // 初始化参数缓存
                        if (!methodParamCache.ContainsKey(method))
                        {
                            object[] paramValues = new object[parameters.Length];
                            for (int i = 0; i < parameters.Length; i++)
                            {
                                Type paramType = parameters[i].ParameterType;
                                if (paramType.IsValueType)
                                {
                                    paramValues[i] = Activator.CreateInstance(paramType);
                                }
                            }

                            methodParamCache[method] = paramValues;
                        }

                        // 如果有参数，显示参数输入框
                        if (parameters.Length > 0)
                        {
                            EditorGUILayout.BeginVertical("box");
                            EditorGUILayout.LabelField(buttonText, EditorStyles.boldLabel);

                            for (int i = 0; i < parameters.Length; i++)
                            {
                                ParameterInfo param = parameters[i];
                                object currentValue = methodParamCache[method][i];

                                // 根据参数类型显示不同的输入控件
                                if (param.ParameterType == typeof(int))
                                {
                                    methodParamCache[method][i] =
                                        EditorGUILayout.IntField(param.Name, (int)currentValue);
                                }
                                else if (param.ParameterType == typeof(float))
                                {
                                    methodParamCache[method][i] =
                                        EditorGUILayout.FloatField(param.Name, (float)currentValue);
                                }
                                else if (param.ParameterType == typeof(string))
                                {
                                    methodParamCache[method][i] =
                                        EditorGUILayout.TextField(param.Name, (string)currentValue);
                                }
                                else if (param.ParameterType == typeof(bool))
                                {
                                    methodParamCache[method][i] =
                                        EditorGUILayout.Toggle(param.Name, (bool)currentValue);
                                }
                                else if (param.ParameterType == typeof(Vector3))
                                {
                                    methodParamCache[method][i] =
                                        EditorGUILayout.Vector3Field(param.Name, (Vector3)currentValue);
                                }
                                else if (typeof(UnityEngine.Object).IsAssignableFrom(param.ParameterType))
                                {
                                    methodParamCache[method][i] = EditorGUILayout.ObjectField(
                                        param.Name,
                                        (UnityEngine.Object)currentValue,
                                        param.ParameterType,
                                        true
                                    );
                                }
                            }
                        }

                        if (GUILayout.Button(parameters.Length > 0 ? "执行" : buttonText))
                        {
                            try
                            {
                                var paramValues = parameters.Length > 0 ? methodParamCache[method] : null;
                                method.Invoke(obj, paramValues);
                                if (obj is UnityEngine.Object unityObj)
                                {
                                    EditorUtility.SetDirty(unityObj);
                                }

                                Debug.Log($"成功调用方法: {method.Name}");
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"调用方法 {method.Name} 时发生错误: {ex.Message}\n{ex.StackTrace}");
                            }
                        }

                        if (parameters.Length > 0)
                        {
                            EditorGUILayout.EndVertical();
                        }
                    }
                }
            }
        }

        public void DrawDefaultInspector(object obj)
        {
            var objectType = obj.GetType();
            FieldInfo[] fieldInfos = objectType.GetFields();
            Array.Sort(fieldInfos, FieldsSprtBy);

            List<FieldInfo> visibleFields = FilterVisibleFields(fieldInfos, obj);

            foreach (var field in visibleFields)
            {
                FieldDefaultInspector(field, obj);
            }
        }

        private List<FieldInfo> FilterVisibleFields(FieldInfo[] fieldInfos, object obj)
        {
            List<FieldInfo> visibleFields = new List<FieldInfo>();
            
            foreach (var field in fieldInfos)
            {
                if (ShouldShowField(field, fieldInfos, obj))
                {
                    visibleFields.Add(field);
                }
            }

            return visibleFields;
        }

        private bool ShouldShowField(FieldInfo field, FieldInfo[] allFields, object obj)
        {
            var attributes = field.GetCustomAttributes();
            
            foreach (var attribute in attributes)
            {
                if (attribute is HideInInspector)
                {
                    return false;
                }

                if (attribute is OptionRelateParamAttribute optionRelate)
                {
                    if (!CheckOptionRelateParam(optionRelate, allFields, obj))
                    {
                        return false;
                    }
                }

                if (attribute is OptionRelateBoolAttribute boolRelate)
                {
                    if (!CheckOptionRelateBool(boolRelate, allFields, obj))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CheckOptionRelateParam(OptionRelateParamAttribute option, FieldInfo[] allFields, object obj)
        {
            var relatedField = Array.Find(allFields, f => f.Name == option.argsName);
            if (relatedField == null)
            {
                return true;
            }

            var fieldValue = relatedField.GetValue(obj);
            var matchIndex = Array.FindIndex(option.argsValue, v => v.Equals(fieldValue));
            return matchIndex >= 0;
        }

        private bool CheckOptionRelateBool(OptionRelateBoolAttribute boolOption, FieldInfo[] allFields, object obj)
        {
            var relatedField = Array.Find(allFields, f => f.Name == boolOption.boolFieldName);
            if (relatedField == null)
            {
                return true;
            }

            var fieldValue = relatedField.GetValue(obj);
            if (fieldValue is bool boolValue)
            {
                var matchIndex = Array.FindIndex(boolOption.boolValues, v => v == boolValue);
                return matchIndex >= 0;
            }

            return true;
        }

        protected void FieldDefaultInspector(FieldInfo field, object obj)
        {
            var fieldType = field.FieldType;
            var showType = field.FieldType;
            var value = field.GetValue(obj);
            var newValue = value;
            bool forceSave = false;

            var name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(field.Name);

            List<object> args = new List<object>();

            var attributes = field.GetCustomAttributes();
            foreach (var attribute in attributes)
            {
                var t = attribute.GetType();
                if (attribute is MenuNameAttribute menuNameAttribute)
                {
                    name = menuNameAttribute.showName;
                }
                else if (attribute is RangeAttribute rangeAttribute && fieldType == typeof(float))
                {
                    showType = t;
                    args = new List<object> { rangeAttribute.min, rangeAttribute.max };
                }
                else if (attribute is OptionParamAttribute option)
                {
                    showType = t;
                    args = new List<object> { option.classType };
                }
                else if (attribute is SelectObjectPathAttribute selectObjectPathAttribute)
                {
                    showType = t;
                    args = new List<object> { selectObjectPathAttribute.type };
                }
            }


            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elementType = fieldType.GetGenericArguments()[0];
                IList list = (IList)value;
                
                if (list == null)
                {
                    list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                    field.SetValue(obj, list);
                    newValue = list;
                }
                
                var foldout = EditorGUILayout.Foldout(GetFoldout(list), field.GetShowName());
                SetFoldout(list, foldout);
                
                if (foldout)
                {
                    GUILayout.Space(6);
                    GUILayout.BeginVertical(GUI.skin.box);
                    
                    int indexToRemove = -1;
                    for (int i = 0; i < list.Count; i++)
                    {
                        object listItem = list[i];
                        
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"Element {i}");
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            indexToRemove = i;
                        }
                        EditorGUILayout.EndHorizontal();
                        
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(12);
                        EditorGUILayout.BeginVertical();
                        
                        if (ShouldShowTypeSelector(elementType, field))
                        {
                            object newInstance = DrawTypeSelectorForElement(elementType, listItem, field);
                            if (newInstance != listItem)
                            {
                                list[i] = newInstance;
                                listItem = newInstance;
                            }
                        }
                        
                        if (listItem != null)
                        {
                            object newElementValue = DrawElementValue(elementType, listItem, i);
                            if (newElementValue != listItem && !newElementValue.Equals(listItem))
                            {
                                list[i] = newElementValue;
                            }
                        }
                        
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        DrawDivider();
                    }
                    
                    if (indexToRemove >= 0)
                    {
                        list.RemoveAt(indexToRemove);
                    }

                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("+", GUILayout.Width(30)))
                    {
                        list.Add(CreateDefaultInstance(elementType));
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
            }
            else if (fieldType.IsArray)
            {
                Type elementType = fieldType.GetElementType();
                Array array = (Array)value;
                
                if (array == null)
                {
                    array = Array.CreateInstance(elementType, 0);
                    field.SetValue(obj, array);
                    newValue = array;
                }

                var foldout = EditorGUILayout.Foldout(GetFoldout(array), field.GetShowName());
                SetFoldout(array, foldout);
                
                if (foldout)
                {
                    GUILayout.Space(6);
                    GUILayout.BeginVertical(GUI.skin.box);
                    
                    int indexToRemove = -1;
                    for (int i = 0; i < array.Length; i++)
                    {
                        object arrayItem = array.GetValue(i);
                        
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"Element {i}");
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            indexToRemove = i;
                        }
                        EditorGUILayout.EndHorizontal();
                        
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(12);
                        EditorGUILayout.BeginVertical();
                        
                        if (ShouldShowTypeSelector(elementType, field))
                        {
                            object newInstance = DrawTypeSelectorForElement(elementType, arrayItem, field);
                            if (newInstance != arrayItem)
                            {
                                array.SetValue(newInstance, i);
                                arrayItem = newInstance;
                            }
                        }
                        
                        if (arrayItem != null)
                        {
                            object newElementValue = DrawElementValue(elementType, arrayItem, i);
                            if (newElementValue != arrayItem && !newElementValue.Equals(arrayItem))
                            {
                                array.SetValue(newElementValue, i);
                            }
                        }
                        
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        DrawDivider();
                    }
                    
                    if (indexToRemove >= 0)
                    {
                        Array newArray = RemoveArrayElement(array, elementType, indexToRemove);
                        RemoveFoldout(array);
                        field.SetValue(obj, newArray);
                        SetFoldout(newArray, true);
                        newValue = newArray;
                    }

                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("+", GUILayout.Width(30)))
                    {
                        Array newArray = AddArrayElement(array, elementType);
                        RemoveFoldout(array);
                        field.SetValue(obj, newArray);
                        SetFoldout(newArray, true);
                        newValue = newArray;
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
            }
            else if (showType == typeof(int))
            {
                newValue = EditorGUILayout.IntField(name, (int)value);
            }
            else if (showType == typeof(float))
            {
                newValue = EditorGUILayout.FloatField(name, (float)value);
            }
            else if (showType == typeof(bool))
            {
                newValue = EditorGUILayout.Toggle(name, (bool)value);
            }
            else if (showType == typeof(string))
            {
                newValue = EditorGUILayout.TextField(name, (string)value);
            }
            else if (showType == typeof(Color))
            {
                newValue = EditorGUILayout.ColorField(name, (Color)value);
            }
            else if (showType.IsSubclassOf(typeof(Object)))
            {
                newValue = EditorGUILayout.ObjectField(name, (Object)value, fieldType, false);
            }
            else if (showType.IsEnum)
            {
                newValue = EditorGUILayout.EnumPopup(name, (Enum)value);
            }
            else if (showType == typeof(AnimationCurve))
            {
                AnimationCurve curve = field.GetValue(obj) as AnimationCurve;
                if (curve == null)
                {
                    curve = new AnimationCurve();
                }

                newValue = EditorGUILayout.CurveField(name, curve);
            }
            else if (showType == typeof(Vector2))
            {
                newValue = EditorGUILayout.Vector2Field(name, (Vector2)value);
            }
            else if (showType == typeof(Vector3))
            {
                newValue = EditorGUILayout.Vector3Field(name, (Vector3)value);
            }
            else if (showType == typeof(Quaternion))
            {
                Vector3 euler = EditorGUILayout.Vector3Field(name, ((Quaternion)value).eulerAngles);
                newValue = Quaternion.Euler(euler);
            }
            else if (showType == typeof(Vector4))
            {
                newValue = EditorGUILayout.Vector4Field(name, (Vector4)value);
            }
            else if (showType == typeof(Vector2Int))
            {
                newValue = EditorGUILayout.Vector2IntField(name, (Vector2Int)value);
            }
            else if (showType == typeof(Vector3Int))
            {
                newValue = EditorGUILayout.Vector3IntField(name, (Vector3Int)value);
            }
            else if (showType == typeof(Rect))
            {
                newValue = EditorGUILayout.RectField(name, (Rect)value);
            }
            else if (showType == typeof(RectInt))
            {
                newValue = EditorGUILayout.RectIntField(name, (RectInt)value);
            }
            else if (showType == typeof(Bounds))
            {
                newValue = EditorGUILayout.BoundsField(name, (Bounds)value);
            }
            else if (showType == typeof(RangeAttribute))
            {
                if (fieldType == typeof(float))
                {
                    newValue = EditorGUILayout.Slider(name, (float)value, (float)args[0], (float)args[1]);
                }
            }
            else if (showType == typeof(OptionParamAttribute))
            {
                // LookAtType.None
                var t = (Type)args[0];
                var fields = t.GetFields();
                //先对fields排序
                Array.Sort(fields, FieldsSort);

                var title = new List<string>();
                var mask = 0;
                foreach (var f in fields)
                {
                    var menuNameAttr = f.GetCustomAttribute<MenuNameAttribute>();
                    title.Add(menuNameAttr != null ? menuNameAttr.showName : f.Name);
                }

                mask = (int)value;

                newValue = EditorGUILayout.Popup(name, mask, title.ToArray());
            }
            else if (showType == typeof(SelectObjectPathAttribute))
            {
                if (args[0] is Type type)
                {
                    Object o = null;
                    var path = value.ToString();
                    if (!string.IsNullOrEmpty(path))
                    {
                        o = AssetDatabase.LoadAssetAtPath(path, type);
                    }

                    GUILayout.Label(path);
                    var newObj = EditorGUILayout.ObjectField(name, o, type, false);
                    if (newObj != o)
                    {
                        newValue = AssetDatabase.GetAssetPath(newObj);
                    }
                }
            }
            else if (showType.GetCustomAttribute<DrawableAttribute>() != null)
            {
                //已经处理过了
                EditorGUILayout.BeginVertical("box");
                if (value == null)
                {
                    value = Activator.CreateInstance(fieldType);
                }

                //递归渲染里面的
                DrawDefaultInspector(value);
                newValue = value;
                forceSave = true;
                EditorGUILayout.EndVertical();
            }
            else if (DrawCustomFieldInspector(fieldType, showType, value, out newValue, name))
            {
            }
            else
            {
                //提示不支持
                EditorGUILayout.LabelField(name, $"Not Support Type: {fieldType.Name}");
            }


            if (value != newValue || forceSave)
            {
                if (!field.IsLiteral && !field.IsInitOnly)
                {
                    field.SetValue(obj, newValue);
                }
            }
        }

        /// <summary>
        /// 子类继承的时候可以单独绘制
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="showType"></param>
        /// <param name="value"></param>
        /// <param name="newValue"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual bool DrawCustomFieldInspector(Type fieldType, Type showType, object value,
            out object newValue,
            string name)
        {
            newValue = value;
            return false;
        }

        private int FieldsSprtBy(FieldInfo f1, FieldInfo f2)
        {
            if (f1 == null || f2 == null) return 0;
            var e1 = f1.DeclaringType == f1.ReflectedType;
            var e2 = f2.DeclaringType == f2.ReflectedType;
            if (e1 != e2)
            {
                if (e1)
                {
                    return 1;
                }

                return -1;
            }

            return 0;
        }

        private int FieldsSort(FieldInfo f1, FieldInfo f2)
        {
            var sort1 = f1.GetCustomAttribute<OptionSortAttribute>();
            var sort2 = f2.GetCustomAttribute<OptionSortAttribute>();
            var i1 = 99;
            var i2 = 99;
            if (sort1 != null)
            {
                i1 = sort1.sort;
            }

            if (sort2 != null)
            {
                i2 = sort2.sort;
            }

            return i1 - i2;
        }

        private bool GetFoldout(object obj)
        {
            if (obj == null) return false;
            if (!_unfoldDictionary.TryGetValue(obj.GetHashCode(), out var value))
            {
                _unfoldDictionary[obj.GetHashCode()] = false;
            }

            return value;
        }

        private void SetFoldout(object obj, bool unfold)
        {
            if (obj == null) return;
            _unfoldDictionary[obj.GetHashCode()] = unfold;
        }

        public void RemoveFoldout(object obj)
        {
            if (obj == null) return;
            _unfoldDictionary.Remove(obj.GetHashCode());
        }

        private void DrawDivider()
        {
            GUILayout.Space(2);
            Color color = Color.black.WithAlpha(0.1f);
            Rect rect = EditorGUILayout.GetControlRect(false, 2);
            EditorGUI.DrawRect(rect, color);
            GUILayout.Space(2);
        }

        private bool ShouldShowTypeSelector(Type elementType, FieldInfo field)
        {
            if (elementType.IsInterface || elementType.IsAbstract)
            {
                return true;
            }

            var objectTypesAttr = field.GetCustomAttribute<ObjectTypesAttribute>();
            if (objectTypesAttr != null && objectTypesAttr.baseType != null)
            {
                return true;
            }

            return false;
        }

        private object DrawTypeSelectorForElement(Type elementType, object currentValue, FieldInfo field)
        {
            Type[] availableTypes = GetAvailableTypes(elementType, field);
            
            if (availableTypes == null || availableTypes.Length == 0)
            {
                EditorGUILayout.HelpBox($"No implementations found for {elementType.Name}", MessageType.Warning);
                return currentValue;
            }

            Type currentType = currentValue?.GetType();
            int selectedIndex = -1;
            
            string[] typeNames = new string[availableTypes.Length + 1];
            typeNames[0] = "(None)";
            
            for (int i = 0; i < availableTypes.Length; i++)
            {
                typeNames[i + 1] = availableTypes[i].Name;
                if (currentType == availableTypes[i])
                {
                    selectedIndex = i + 1;
                }
            }

            if (selectedIndex == -1)
            {
                selectedIndex = 0;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Type", GUILayout.Width(100));
            int newIndex = EditorGUILayout.Popup(selectedIndex, typeNames);
            EditorGUILayout.EndHorizontal();

            if (newIndex != selectedIndex)
            {
                if (newIndex == 0)
                {
                    return null;
                }
                else
                {
                    Type newType = availableTypes[newIndex - 1];
                    return CreateDefaultInstance(newType);
                }
            }

            return currentValue;
        }

        private Type[] GetAvailableTypes(Type baseType, FieldInfo field)
        {
            var objectTypesAttr = field.GetCustomAttribute<ObjectTypesAttribute>();
            
            if (objectTypesAttr != null)
            {
                return objectTypesAttr.types;
            }

            if (baseType.IsInterface || baseType.IsAbstract)
            {
                var derivedTypes = TypeCache.GetTypesDerivedFrom(baseType)
                    .Where(t => !t.IsAbstract && !t.IsInterface)
                    .ToArray();
                return derivedTypes;
            }

            return new Type[0];
        }

        private object DrawElementValue(Type elementType, object elementValue, int index)
        {
            if (elementType == typeof(int))
            {
                return EditorGUILayout.IntField((int)elementValue);
            }
            else if (elementType == typeof(float))
            {
                return EditorGUILayout.FloatField((float)elementValue);
            }
            else if (elementType == typeof(bool))
            {
                return EditorGUILayout.Toggle((bool)elementValue);
            }
            else if (elementType == typeof(string))
            {
                return EditorGUILayout.TextField((string)elementValue ?? string.Empty);
            }
            else if (elementType == typeof(Vector2))
            {
                return EditorGUILayout.Vector2Field(string.Empty, (Vector2)elementValue);
            }
            else if (elementType == typeof(Vector3))
            {
                return EditorGUILayout.Vector3Field(string.Empty, (Vector3)elementValue);
            }
            else if (elementType == typeof(Vector4))
            {
                return EditorGUILayout.Vector4Field(string.Empty, (Vector4)elementValue);
            }
            else if (elementType == typeof(Vector2Int))
            {
                return EditorGUILayout.Vector2IntField(string.Empty, (Vector2Int)elementValue);
            }
            else if (elementType == typeof(Vector3Int))
            {
                return EditorGUILayout.Vector3IntField(string.Empty, (Vector3Int)elementValue);
            }
            else if (elementType == typeof(Color))
            {
                return EditorGUILayout.ColorField((Color)elementValue);
            }
            else if (elementType == typeof(Quaternion))
            {
                Vector3 euler = EditorGUILayout.Vector3Field(string.Empty, ((Quaternion)elementValue).eulerAngles);
                return Quaternion.Euler(euler);
            }
            else if (elementType == typeof(Rect))
            {
                return EditorGUILayout.RectField((Rect)elementValue);
            }
            else if (elementType == typeof(RectInt))
            {
                return EditorGUILayout.RectIntField((RectInt)elementValue);
            }
            else if (elementType == typeof(Bounds))
            {
                return EditorGUILayout.BoundsField((Bounds)elementValue);
            }
            else if (elementType == typeof(AnimationCurve))
            {
                AnimationCurve curve = elementValue as AnimationCurve;
                if (curve == null)
                {
                    curve = new AnimationCurve();
                }
                return EditorGUILayout.CurveField(curve);
            }
            else if (elementType.IsSubclassOf(typeof(Object)))
            {
                return EditorGUILayout.ObjectField((Object)elementValue, elementType, false);
            }
            else if (elementType.IsEnum)
            {
                return EditorGUILayout.EnumPopup((Enum)elementValue);
            }
            else
            {
                DrawDefaultInspector(elementValue);
                return elementValue;
            }
        }

        private object CreateDefaultInstance(Type type)
        {
            if (type == typeof(string))
            {
                return string.Empty;
            }
            else if (type == typeof(AnimationCurve))
            {
                return new AnimationCurve();
            }
            else if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else if (type.IsArray)
            {
                return Array.CreateInstance(type.GetElementType(), 0);
            }
            else
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch
                {
                    return null;
                }
            }
        }

        private Array AddArrayElement(Array originalArray, Type elementType)
        {
            int newLength = originalArray.Length + 1;
            Array newArray = Array.CreateInstance(elementType, newLength);
            originalArray.CopyTo(newArray, 0);
            newArray.SetValue(CreateDefaultInstance(elementType), newLength - 1);
            return newArray;
        }

        private Array RemoveArrayElement(Array originalArray, Type elementType, int indexToRemove)
        {
            int newLength = originalArray.Length - 1;
            Array newArray = Array.CreateInstance(elementType, newLength);
            
            for (int i = 0, j = 0; i < originalArray.Length; i++)
            {
                if (i != indexToRemove)
                {
                    newArray.SetValue(originalArray.GetValue(i), j);
                    j++;
                }
            }
            
            return newArray;
        }
    }
}