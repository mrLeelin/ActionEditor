using UnityEngine;

namespace NBC.ActionEditor
{
    public abstract class PreviewBase<T> : PreviewBase where T : IDirectable
    {
        public T clip => (T)directable;
    }

    public abstract class PreviewBase
    {
        public IDirectable directable;


        protected IActionController Target { get; private set; }

        public void SetTarget(IDirectable t)
        {
            directable = t;
        }

        public void SetSelectGameObject(IActionController selectGameObject)
        {
            Target = selectGameObject;
            if (Target != null)
            {
                OnSetSelectTarget(Target);
            }
            else
            {
                OnClearSelecTarget();
            }
        }

        /// <summary>
        /// 初始化Preview
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// 进入
        /// isReverse 是否从右边进入
        /// </summary>
        public virtual void Enter(bool isReverse)
        {
        }

        /// <summary>
        /// 退出
        /// isReverse 是否从左面推出
        /// </summary>
        public virtual void Exit(bool isReverse)
        {
        }

        /// <summary>
        /// Assets 被释放调用
        /// </summary>
        public virtual void OnDestroy()
        {
        }

        protected virtual void OnSetSelectTarget(IActionController target)
        {
        }

        protected virtual void OnClearSelecTarget()
        {
        }

        public abstract void Update(float time, float previousTime);
    }
}