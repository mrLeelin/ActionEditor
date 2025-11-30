using UnityEngine;

namespace NBC.ActionEditor
{
    public abstract class PreviewBase<T> : PreviewBase where T : IDirectable
    {
        public T clip => (T)Directable;
    }

    public abstract class PreviewBase
    {
        public IDirectable Directable;
        private bool _isInPreview;

        /// <summary>
        /// 用于预览的GameObject
        /// 地下可以挂载实例化的GameObject
        /// </summary>
        private Transform _groupBaseTransform;

        private bool _lastIsBeSelect;
        protected IActionController Target { get; private set; }

        protected Transform GroupBaseTransform => _groupBaseTransform;


        /// <summary>
        /// 是否在预览中
        /// </summary>
        public bool IsInPreview => _isInPreview;

        public void SetTarget(IDirectable t)
        {
            Directable = t;
            _lastIsBeSelect = false;
        }

        public void SetGroupBaseTransform(Transform t) => _groupBaseTransform = t;

        public void SetSelectGameObject(IActionController selectGameObject)
        {
            Target = selectGameObject;
            if (Target != null)
            {
                OnSetSelectTarget(Target);
            }
            else
            {
                OnClearSelectTarget();
            }
        }

        /// <summary>
        /// 初始化Preview
        /// </summary>
        public virtual void Initialize()
        {
            _isInPreview = false;
        }

        /// <summary>
        /// 进入
        /// isReverse 是否从右边进入
        /// </summary>
        public virtual void Enter(bool isReverse)
        {
            _isInPreview = true;
        }

        /// <summary>
        /// 退出
        /// isReverse 是否从左面推出
        /// </summary>
        public virtual void Exit(bool isReverse)
        {
            _isInPreview = false;
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

        protected virtual void OnClearSelectTarget()
        {
        }

        public void Update(float time, float previousTime)
        {
            _ = IsBeSelect();
            OnUpdate(time, previousTime);
        }

        protected abstract void OnUpdate(float time, float previousTime);

        public bool IsBeSelect()
        {
            if (Directable == null) return false;
            var isBeSelect = App.IsSelect(Directable);
            if (_lastIsBeSelect != isBeSelect)
            {
                OnSelectChange(isBeSelect);
            }

            _lastIsBeSelect = isBeSelect;
            return isBeSelect;
        }

        protected virtual void OnSelectChange(bool isSelect)
        {
        }
    }
}