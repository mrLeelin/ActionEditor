using System;
using System.Collections.Generic;
using UnityEngine;

namespace NBC.ActionEditor
{
    /// <summary>
    /// 安全事件类，提供异常隔离和自动清理功能
    /// </summary>
    /// <remarks>
    /// 相比原生 Action 委托的优势：
    /// 1. 异常隔离 - 一个订阅者出错不影响其他订阅者
    /// 2. 自动清理 - 支持域重载时批量清理
    /// 3. 调试友好 - 可查看订阅者数量
    /// </remarks>
    public class SafeEvent
    {
        private readonly List<Action> _callbacks = new List<Action>();

        /// <summary>
        /// 当前订阅者数量
        /// </summary>
        public int SubscriberCount => _callbacks.Count;

        /// <summary>
        /// 添加订阅者
        /// </summary>
        public void Add(Action callback)
        {
            if (callback != null && !_callbacks.Contains(callback))
            {
                _callbacks.Add(callback);
            }
        }

        /// <summary>
        /// 移除订阅者
        /// </summary>
        public void Remove(Action callback)
        {
            if (callback != null)
            {
                _callbacks.Remove(callback);
            }
        }

        /// <summary>
        /// 清理所有订阅者
        /// </summary>
        public void Clear()
        {
            _callbacks.Clear();
        }

        /// <summary>
        /// 安全调用所有订阅者，异常隔离
        /// </summary>
        public void Invoke()
        {
            // 使用 ToArray 避免在遍历时修改集合
            foreach (var callback in _callbacks.ToArray())
            {
                try
                {
                    callback?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// 支持 += 语法添加订阅
        /// </summary>
        public static SafeEvent operator +(SafeEvent e, Action callback)
        {
            e?.Add(callback);
            return e;
        }

        /// <summary>
        /// 支持 -= 语法移除订阅
        /// </summary>
        public static SafeEvent operator -(SafeEvent e, Action callback)
        {
            e?.Remove(callback);
            return e;
        }
    }

    /// <summary>
    /// 带参数的安全事件类
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    public class SafeEvent<T>
    {
        private readonly List<Action<T>> _callbacks = new List<Action<T>>();

        /// <summary>
        /// 当前订阅者数量
        /// </summary>
        public int SubscriberCount => _callbacks.Count;

        /// <summary>
        /// 添加订阅者
        /// </summary>
        public void Add(Action<T> callback)
        {
            if (callback != null && !_callbacks.Contains(callback))
            {
                _callbacks.Add(callback);
            }
        }

        /// <summary>
        /// 移除订阅者
        /// </summary>
        public void Remove(Action<T> callback)
        {
            if (callback != null)
            {
                _callbacks.Remove(callback);
            }
        }

        /// <summary>
        /// 清理所有订阅者
        /// </summary>
        public void Clear()
        {
            _callbacks.Clear();
        }

        /// <summary>
        /// 安全调用所有订阅者，异常隔离
        /// </summary>
        public void Invoke(T arg)
        {
            foreach (var callback in _callbacks.ToArray())
            {
                try
                {
                    callback?.Invoke(arg);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// 支持 += 语法添加订阅
        /// </summary>
        public static SafeEvent<T> operator +(SafeEvent<T> e, Action<T> callback)
        {
            e?.Add(callback);
            return e;
        }

        /// <summary>
        /// 支持 -= 语法移除订阅
        /// </summary>
        public static SafeEvent<T> operator -(SafeEvent<T> e, Action<T> callback)
        {
            e?.Remove(callback);
            return e;
        }
    }
}
