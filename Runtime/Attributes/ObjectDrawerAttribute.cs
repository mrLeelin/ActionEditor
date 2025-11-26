using System;
using System.Diagnostics;

namespace NBC.ActionEditor
{
    /// <summary>
    /// ObjectDrawer
    /// 
    /// 静态函数 object Func(GUIContent title, object obj, Type type, object[] attrs);
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ObjectDrawerAttribute : Attribute
    {
        public Type type { get; private set; }
        public ObjectDrawerAttribute(Type type)
        {
            this.type = type;
        }
    }
}