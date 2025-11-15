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

        
        protected GameObject Target { get; private set; }
        
        public void SetTarget(IDirectable t)
        {
            directable = t;
        }

        public void SetSelectGameObject(GameObject selectGameObject)
        {
            Target = selectGameObject;
            if (Target != null)
            {
                OnSetSelectGameObject(Target);
            }
            else
            {
                OnClearSelectGameObject();
            }
        }

        /// <summary>
        /// 初始化Preview
        /// </summary>
        public virtual void Initialize()
        {
          
        }
        /// <summary>
        /// 从前面进入
        /// </summary>
        public virtual void Enter()
        {
            
        }

        /// <summary>
        /// 从后面出去
        /// </summary>
        public virtual void Exit()
        {
           
        }

        /// <summary>
        /// 从后面进入
        /// </summary>
        public virtual void ReverseEnter()
        {
           
        }

        /// <summary>
        /// 从左面出去
        /// </summary>
        public virtual void Reverse()
        {
            
        }
        
        /// <summary>
        /// Assets 被释放调用
        /// </summary>
        public virtual void OnDestroy()
        {
            
        }

        protected virtual void OnSetSelectGameObject(GameObject target)
        {
        
        }

        protected virtual void OnClearSelectGameObject()
        {
           
        }

        public abstract void Update(float time, float previousTime);
    }
}