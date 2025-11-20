using System;
using System.Collections.Generic;
using FullSerializer;
using UnityEngine;

namespace NBC.ActionEditor
{
    [Serializable]
    public abstract class Clip : IClip
    {
        [SerializeField] private bool enableLoop = true;
        [SerializeField] private float startTime;
        [SerializeField] [HideInInspector] private float length = 1f;
        [SerializeField] private string name;


        //====================Real Value==========================
        private float _realStartTime;
        private float _realLength = 1f;
        private bool _isBeCreateFlag;
        //======================================================

        [MenuName("是否循环")]
        [fsIgnore]
        public bool EnableLoop
        {
            get => enableLoop;
            set => enableLoop = value;
        }

        [MenuName("片段长度")]
        [fsIgnore]
        public virtual float Length
        {
            get => _realLength;
            set => _realLength = value;
        }

        public virtual string Info => null;
        /*
        {
            get
            {
                var nameAtt = GetType().RTGetAttribute<NameAttribute>(true);
                if (nameAtt != null) return nameAtt.name;

                return GetType().Name.SplitCamelCase();
            }
        }
        */

        public virtual bool IsValid => false;

        [fsIgnore] public IDirector Root => Parent?.Root;

        [fsIgnore] public IDirectable Parent { get; private set; }

        IEnumerable<IDirectable> IDirectable.Children => null;

        public GameObject Actor => Parent?.Actor;

        public string Name
        {
            get => name;
            set => name = value;
        }


        [MenuName("开始时间")]
        public float StartTime
        {
            get => _realStartTime;
            set
            {
                if (Math.Abs(_realStartTime - value) > 0.0001f)
                {
                    _realStartTime = Mathf.Max(value, 0);
                    // BlendIn = Mathf.Clamp(BlendIn, 0, Length - BlendOut);
                    // BlendOut = Mathf.Clamp(BlendOut, 0, Length - BlendIn);
                }
            }
        }

        [MenuName("结束时间")]
        public float EndTime
        {
            get => StartTime + Length;
            set
            {
                if (Math.Abs(StartTime + Length - value) > 0.0001f)
                {
                    Length = Mathf.Max(value - StartTime, 0);
                    BlendOut = Mathf.Clamp(BlendOut, 0, Length - BlendIn);
                    BlendIn = Mathf.Clamp(BlendIn, 0, Length - BlendOut);
                }
            }
        }

        public bool IsActive
        {
            get => Parent?.IsActive ?? false;
            set { }
        }

        public bool IsCollapsed
        {
            get { return Parent != null && Parent.IsCollapsed; }
            set { }
        }

        public bool IsLocked
        {
            get { return Parent != null && Parent.IsLocked; }
            set { }
        }

        public virtual float BlendIn
        {
            get => 0;
            set { }
        }

        public virtual float BlendOut
        {
            get => 0;
            set { }
        }

        public virtual bool CanCrossBlend { get; }

        [Button("长度铺满")]
        public void ResetLengthToAssets()
        {
            StartTime = 0;
            Length = Root.Length;
        }

        public virtual void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            RealValueToCache();
#endif
        }

        public virtual void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            CacheToRealValue();
#endif
        }

        public bool Initialize()
        {
            return true;
        }

        public void Validate(IDirector root, IDirectable parent)
        {
            Parent = parent;
            // hideFlags = HideFlags.HideInHierarchy;
            // ValidateAnimParams();
            // OnAfterValidate();
        }


        public object AnimatedParametersTarget { get; }

        public Clip GetNextClip()
        {
            return this.GetNextSibling<Clip>();
        }

        public float GetClipWeight(float time)
        {
            return GetClipWeight(time, BlendIn, BlendOut);
        }

        public float GetClipWeight(float time, float blendInOut)
        {
            return GetClipWeight(time, blendInOut, blendInOut);
        }

        public float GetClipWeight(float time, float blendIn, float blendOut)
        {
            return this.GetWeight(time, blendIn, blendOut);
        }

        #region 长度匹配

        public void TryMatchSubClipLength()
        {
            if (this is ISubClipContainable)
                Length = ((ISubClipContainable)this).SubClipLength / ((ISubClipContainable)this).SubClipSpeed;
        }

        public void TryMatchPreviousSubClipLoop()
        {
            if (this is ISubClipContainable) Length = (this as ISubClipContainable).GetPreviousLoopLocalTime();
        }

        public void TryMatchNexSubClipLoop()
        {
            if (this is ISubClipContainable)
            {
                var targetLength = (this as ISubClipContainable).GetNextLoopLocalTime();
                var nextClip = GetNextClip();
                if (nextClip == null || StartTime + targetLength <= nextClip.StartTime) Length = targetLength;
            }
        }

        #endregion

        #region 混合切片

        public virtual void SetCrossBlendIn(float value)
        {
        }

        public virtual void SetCrossBlendOut(float value)
        {
        }

        #endregion

        public void PostCreate(IDirectable parent)
        {
            Parent = parent;
            // CreateAnimationDataCollection();
            // OnCreate();
        }

        internal void SetCreateFlag() => _isBeCreateFlag = true;
        
        /// <summary>
        /// 主要用于Editor中查找资源
        /// Runtime也不会报错会返回Null
        /// </summary>
        /// <param name="refPath"></param>
        /// <param name="result"></param>
        /// <typeparam name="TObj"></typeparam>
        /// <returns></returns>
        protected virtual TObj FindAssetsWithPath<TObj>(string refPath) 
            where TObj : UnityEngine.Object 
            => ResourceUtility.FindAssetsWithPath<TObj>(refPath);

#if UNITY_EDITOR
        private void RealValueToCache()
        {
            if (Prefs.timeStepMode == Prefs.TimeStepMode.Frames)
            {
                startTime = Mathf.FloorToInt(_realStartTime * Prefs.FrameRate);
                length = Mathf.FloorToInt(_realLength * Prefs.FrameRate);
            }
            else
            {
                startTime = _realStartTime;
                length = _realLength;
            }
        }

        private void CacheToRealValue()
        {
            //如果是新建的Clip 在新建的时候已经给与了 time 和length的正确值所以不需要从缓存读取值
            if (_isBeCreateFlag)
            {
                return;
            }

            if (Prefs.timeStepMode == Prefs.TimeStepMode.Frames)
            {
                _realStartTime = (startTime / Prefs.FrameRate);
                _realLength = (length / Prefs.FrameRate);
            }
            else
            {
                _realStartTime = startTime;
                _realLength = length;
            }
        }
#endif
        public virtual void CopyFrom(Clip originClip)
        {
        }
    }


    [Serializable]
    public abstract class ClipSignal : Clip
    {
        public override float Length
        {
            get => 0;
            //set => TimeCache();
        }
    }

    [Serializable]
    public abstract class ClipCrossBlend : Clip
    {
        [SerializeField] [HideInInspector] protected float blendIn = 0f;
        [SerializeField] [HideInInspector] protected float blendOut = 0f;

        [SerializeField] [HideInInspector] private float CrossBlendIn = 0f;
        [SerializeField] [HideInInspector] private float CrossBlendOut = 0f;


        public override bool CanCrossBlend => true;

        [MenuName("渐入时间")]
        public override float BlendIn
        {
            get
            {
                if (CrossBlendIn > 0)
                {
                    return CrossBlendIn;
                }

                return blendIn;
            }
            set
            {
                blendIn = value;
                if (blendIn < 0)
                {
                    blendIn = 0;
                }
                else if (blendIn > Length - BlendOut)
                {
                    blendIn = Length - BlendOut;
                }
            }
        }

        [MenuName("渐出时间")]
        public override float BlendOut
        {
            get
            {
                if (CrossBlendOut > 0)
                {
                    return CrossBlendOut;
                }

                return blendOut;
            }
            set
            {
                blendOut = value;
                if (blendOut < 0)
                {
                    blendOut = 0;
                }
                else if (blendOut > Length - BlendIn)
                {
                    blendOut = Length - BlendIn;
                }
            }
        }

        public override void SetCrossBlendIn(float value)
        {
            CrossBlendIn = value;
        }

        public override void SetCrossBlendOut(float value)
        {
            CrossBlendOut = value;
        }
    }
}