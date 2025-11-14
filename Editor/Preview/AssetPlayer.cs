using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NBC.ActionEditor
{
    public enum EditorPlaybackState
    {
        Stoped,
        PlayingForwards,
        PlayingBackwards
    }

    public enum PointerDragType
    {
        None,
        Play,
        StartRange,
        EndRange
    }

    public class AssetPlayer
    {
        private static AssetPlayer _inst;

        public static AssetPlayer Inst
        {
            get
            {
                if (_inst == null)
                {
                    _inst = new AssetPlayer();
                }

                return _inst;
            }
        }

        private List<IDirectableTimePointer> timePointers;

        /// <summary>
        /// 预览器
        /// </summary>
        private List<IDirectableTimePointer> unsortedStartTimePointers;

        private Dictionary<Type, Type> previewTypeDic = new Dictionary<Type, Type>();

        private float playTimeMin;
        private float playTimeMax;
        private float currentTime;

        public float previousTime { get; private set; }

        private bool preInitialized;

        public Asset Asset => App.AssetData;

        public PointerDragType PointerDragType { get; set; } = PointerDragType.None;

        /// <summary>
        /// 选中的预制体
        /// </summary>
        public GameObject SelectSceneGameObject { get; set; }


        public AssetPlayer()
        {
            App.OnOpenAsset += OnOpenAsset;
        }

        ~AssetPlayer()
        {
            App.OnOpenAsset -= OnOpenAsset;
        }

        private void OnOpenAsset(Asset asset)
        {
            InitializePreviewPointers();
        }

        /// <summary>
        /// 当前时间
        /// </summary>
        public float CurrentTime
        {
            get => currentTime;
            set => currentTime = Mathf.Clamp(value, 0, Length);
        }

        public int CurrentFrame => Mathf.FloorToInt(currentTime * Prefs.FrameRate);


        public int LengthInFrames => Mathf.FloorToInt(Length * Prefs.FrameRate);

        public float Length
        {
            get
            {
                if (Asset != null)
                {
                    return Asset.Length;
                }

                return 0;
            }
        }

        public bool ExitSelectGameObjectInPreview()
        {
            return SelectSceneGameObject != null;
        }

        public void Sample()
        {
            Sample(currentTime);
        }

        public Type FindPreviewType(Type type)
        {
            return previewTypeDic.ContainsKey(type) ? previewTypeDic[type] : null;
        }

        public void Sample(float time)
        {
            CurrentTime = time;
            if ((currentTime == 0 || currentTime == Length) && previousTime == currentTime)
            {
                return;
            }

            if (!preInitialized && currentTime > 0 && previousTime == 0)
            {
                InitializePreviewPointers();
            }


            if (timePointers != null)
            {
                InternalSamplePointers(currentTime, previousTime);
            }

            previousTime = currentTime;
        }

        void InternalSamplePointers(float currentTime, float previousTime)
        {
            if (!Application.isPlaying || currentTime > previousTime)
            {
                foreach (var t in timePointers)
                {
                    try
                    {
                        t.TriggerForward(currentTime, previousTime);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }


            if (!Application.isPlaying || currentTime < previousTime)
            {
                for (var i = timePointers.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        timePointers[i].TriggerBackward(currentTime, previousTime);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            if (unsortedStartTimePointers != null)
            {
                foreach (var t in unsortedStartTimePointers)
                {
                    try
                    {
                        t.Update(currentTime, previousTime);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        /// <summary>
        /// 初始化时间指针预览器
        /// </summary>
        public void InitializePreviewPointers()
        {
            timePointers = new List<IDirectableTimePointer>();
            unsortedStartTimePointers = new List<IDirectableTimePointer>();

            previewTypeDic.Clear();
            var childs = EditorTools.GetTypeMetaDerivedFrom(typeof(PreviewBase));
            foreach (var t in childs)
            {
                var arrs = t.type.GetCustomAttributes(typeof(CustomPreviewAttribute), true);
                foreach (var arr in arrs)
                {
                    if (arr is CustomPreviewAttribute c)
                    {
                        var bindT = c.PreviewType;
                        var iT = t.type;
                        if (!previewTypeDic.ContainsKey(bindT))
                        {
                            if (!iT.IsAbstract) previewTypeDic[bindT] = iT;
                        }
                        else
                        {
                            var old = previewTypeDic[bindT];
                            //如果不是抽象类，且是子类就更新
                            if (!iT.IsAbstract && iT.IsSubclassOf(old))
                            {
                                previewTypeDic[bindT] = iT;
                            }
                        }
                    }
                }
            }

            foreach (var group in Asset.groups.AsEnumerable().Reverse())
            {
                if (!group.IsActive) continue;
                foreach (var track in group.Tracks.AsEnumerable().Reverse())
                {
                    if (!track.IsActive) continue;
                    var tType = track.GetType();
                    if (previewTypeDic.TryGetValue(tType, out var t1))
                    {
                        if (Activator.CreateInstance(t1) is PreviewBase preview)
                        {
                            preview.SetTarget(track);
                            var p3 = new StartTimePointer(preview);
                            timePointers.Add(p3);

                            unsortedStartTimePointers.Add(p3);
                            timePointers.Add(new EndTimePointer(preview));
                        }
                    }

                    foreach (var clip in track.Clips)
                    {
                        var cType = clip.GetType();
                        if (previewTypeDic.TryGetValue(cType, out var t))
                        {
                            if (Activator.CreateInstance(t) is PreviewBase preview)
                            {
                                preview.SetTarget(clip);
                                var p3 = new StartTimePointer(preview);
                                timePointers.Add(p3);

                                unsortedStartTimePointers.Add(p3);
                                timePointers.Add(new EndTimePointer(preview));
                            }
                        }
                    }
                }
            }

            preInitialized = true;
        }
    }
}