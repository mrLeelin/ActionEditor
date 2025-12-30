using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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

    public class AssetPlayer : ScriptableObject
    {
        private static AssetPlayer _inst;

        public static AssetPlayer Inst
        {
            get
            {
                if (_inst == null)
                {
                    // 运行时创建 ScriptableObject，不保存到磁盘
                    _inst = CreateInstance<AssetPlayer>();
                    //_inst.hideFlags = HideFlags.HideAndDontSave;
                    _inst.InitializeFields();
                }

                return _inst;
            }
            set { _inst = value; }
        }

        [SerializeField] private float _currentTime;  // 可序列化，支持 Undo
        public float previousTime { get; private set; }

        // 非序列化字段（运行时初始化）
        private List<IDirectableTimePointer> timePointers;
        private List<PreviewBase> _allPreview;
        private INBCActionController _currentSelectGo;

        /// <summary>
        /// 预览器
        /// </summary>
        private List<IDirectableTimePointer> unsortedStartTimePointers;

        private Dictionary<Type, Type> previewTypeDic = new Dictionary<Type, Type>();
        private Dictionary<IDirectable, PreviewBase> _linkPreview;
        private List<PreviewerSamplerBase> _previewHandles;

        private float playTimeMin;
        private float playTimeMax;

        private bool preInitialized;

        public Asset Asset => App.AssetData;

        public PointerDragType PointerDragType { get; set; } = PointerDragType.None;


        /// <summary>
        /// 预览器根节点
        /// </summary>
        public GameObject PreviewGroupRoot { get; private set; }

        /// <summary>
        /// 所有使用的Previews
        /// </summary>
        public List<PreviewBase> AllPreview => _allPreview;

        /// <summary>
        /// 选中的预制体
        /// </summary>
        public INBCActionController SelectSceneGameObject
        {
            get => _currentSelectGo;
            set
            {
                if (_currentSelectGo != value)
                {
                    if (_allPreview != null)
                    {
                        foreach (var previewBase in _allPreview)
                        {
                            previewBase.SetSelectGameObject(value);
                        }
                    }
                }

                _currentSelectGo = value;
            }
        }


        private void InitializeFields()
        {
            // 初始化运行时字段
            _previewHandles = new List<PreviewerSamplerBase>();
            previewTypeDic = new Dictionary<Type, Type>();

            // 注册事件
            App.OnOpenAsset += OnOpenAsset;
            App.OnCloseAssets += OnCloseAssets;
            App.OnStop += OnStop;
            Track.OnAddClip += OnAddClipCallBack;
            Track.OnDeleteClip += OnDeleteClipCallBack;
        }

        private void OnDestroy()
        {
            // 反注册事件（当 ScriptableObject 被销毁时）
            App.OnOpenAsset -= OnOpenAsset;
            App.OnCloseAssets -= OnCloseAssets;
            Track.OnAddClip -= OnAddClipCallBack;
            Track.OnDeleteClip -= OnDeleteClipCallBack;
            App.OnStop -= OnStop;
        }

        private void OnStop()
        {
            if (_allPreview == null)
            {
                return;
            }

            foreach (var previewBase in _allPreview)
            {
                if (previewBase == null)
                {
                    continue;
                }

                previewBase.Exit(false);
            }
        }


        public void OnCloseAssets()
        {
            timePointers = null;
            unsortedStartTimePointers = null;
            if (_allPreview != null)
            {
                foreach (var preview in _allPreview)
                {
                    preview.OnDestroy();
                }
            }

            previewTypeDic.Clear();
            _linkPreview = null;
            ClearAllPreviewHandles();
            DestroyPreviewAssetsRootInScene();
        }

        private void OnOpenAsset(Asset asset)
        {
            CreatePreviewAssetsRootInScene();
            InitializePreviewPointers();
        }

        /// <summary>
        /// 当前时间（支持 Undo 撤销）
        /// </summary>
        public float CurrentTime
        {
            get => _currentTime;
            set
            {
                if (_currentTime != value)
                {
                    // 记录修改以支持 Undo
                    Undo.RecordObject(this, "改变播放时间");
                    _currentTime = Mathf.Clamp(value, 0, Length);
                    EditorUtility.SetDirty(this);
                }
            }
        }

        public int CurrentFrame => Mathf.FloorToInt(_currentTime * Prefs.FrameRate);


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
            Sample(_currentTime);
        }

        public Type FindPreviewType(Type type)
        {
            return previewTypeDic.ContainsKey(type) ? previewTypeDic[type] : null;
        }

        public void Sample(float time)
        {
            if (!App.IsPlay)
            {
                return;
            }

            CurrentTime = time;
            if (!TimeConverter.IsFrameMode)
            {
                if ((_currentTime == 0 || _currentTime == Length) && previousTime == _currentTime)
                {
                    return;
                }
            }
            else
            {
                if (CurrentFrame < 0 || CurrentFrame > LengthInFrames)
                {
                    return;
                }
            }


            if (!preInitialized && _currentTime > 0 && previousTime == 0)
            {
                InitializePreviewPointers();
            }


            if (timePointers != null)
            {
                InternalSamplePointers(_currentTime, previousTime);
            }

            previousTime = _currentTime;
        }

        public THandle CreateSampler<THandle>(PreviewBase previewBase) where THandle : PreviewerSamplerBase, new()
        {
            var result = new THandle();
            result.Init(SelectSceneGameObject, previewBase, PreviewGroupRoot);
            _previewHandles.Add(result);
            return result;
        }

        public void DestroySampler(PreviewerSamplerBase handle)
        {
            if (handle == null)
            {
                return;
            }

            handle.SelfDestroy();
            _previewHandles.Remove(handle);
        }

        private void InternalSamplePointers(float currentTime, float previousTime)
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

        private void CreatePreviewAssetsRootInScene()
        {
            if (PreviewGroupRoot != null)
            {
                return;
            }

            var findScenePreviewRoot = GameObject.Find(Lan.PreviewGroupRootName);
            if (findScenePreviewRoot != null)
            {
                PreviewGroupRoot = findScenePreviewRoot;
                return;
            }

            PreviewGroupRoot = new GameObject(Lan.PreviewGroupRootName);
        }

        private void DestroyPreviewAssetsRootInScene()
        {
            if (PreviewGroupRoot != null)
            {
                Object.DestroyImmediate(PreviewGroupRoot);
                PreviewGroupRoot = null;
            }
        }


        private void ClearAllPreviewHandles()
        {
            foreach (var previewerOnObject in _previewHandles)
            {
                previewerOnObject.SelfDestroy();
            }

            _previewHandles.Clear();
        }

        private void OnDeleteClipCallBack(Clip clip)
        {
            if (!_linkPreview.Remove(clip, out var previewBase))
            {
                return;
            }

            previewBase.OnDestroy();
            timePointers.RemoveAll(x => x.target == previewBase);
            unsortedStartTimePointers.RemoveAll(x => x.target == previewBase);
            _allPreview.Remove(previewBase);
        }

        private void OnAddClipCallBack(Clip clip)
        {
            var cType = clip.GetType();
            if (!previewTypeDic.TryGetValue(cType, out var t))
            {
                return;
            }

            if (Activator.CreateInstance(t) is PreviewBase preview)
            {
                preview.SetTarget(clip);
                preview.SetGroupBaseTransform(PreviewGroupRoot.transform);
                preview.Initialize();
                if (SelectSceneGameObject != null)
                {
                    preview.SetSelectGameObject(SelectSceneGameObject);
                }

                var p3 = new StartTimePointer(preview);
                timePointers.Add(p3);
                unsortedStartTimePointers.Add(p3);
                timePointers.Add(new EndTimePointer(preview));
                _allPreview.Add(preview);
                _linkPreview.Add(clip, preview);
            }
        }

        private void OnAddTrackCallBack(Track track)
        {
            var tType = track.GetType();
            if (!previewTypeDic.TryGetValue(tType, out var t1))
            {
                return;
            }

            if (Activator.CreateInstance(t1) is PreviewBase preview)
            {
                preview.SetTarget(track);
                preview.SetGroupBaseTransform(PreviewGroupRoot.transform);
                preview.Initialize();
                if (SelectSceneGameObject != null)
                {
                    preview.SetSelectGameObject(SelectSceneGameObject);
                }

                var p3 = new StartTimePointer(preview);
                timePointers.Add(p3);
                unsortedStartTimePointers.Add(p3);
                timePointers.Add(new EndTimePointer(preview));
                _allPreview.Add(preview);
                _linkPreview.Add(track, preview);
            }
        }


        /// <summary>
        /// 初始化时间指针预览器
        /// </summary>
        private void InitializePreviewPointers()
        {
            timePointers = new List<IDirectableTimePointer>();
            unsortedStartTimePointers = new List<IDirectableTimePointer>();
            _allPreview = new List<PreviewBase>();
            _linkPreview = new Dictionary<IDirectable, PreviewBase>();
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
                    OnAddTrackCallBack(track);
                    foreach (var clip in track.Clips)
                    {
                        OnAddClipCallBack(clip);
                    }
                }
            }

            preInitialized = true;
        }
    }
}