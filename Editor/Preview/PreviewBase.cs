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

        public void SetTarget(IDirectable t)
        {
            directable = t;
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void ReverseEnter()
        {
        }

        public virtual void Reverse()
        {
        }


        public abstract void Update(float time, float previousTime);

        /// <summary>
        /// 获取编辑器选中的游戏对象
        /// </summary>
        /// <returns></returns>
        protected virtual GameObject GetSelectGameObject()
        {
            return AssetPlayer.Inst.SelectSceneGameObject;
        }
    }
}