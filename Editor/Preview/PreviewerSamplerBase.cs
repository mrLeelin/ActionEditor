using NBC.ActionEditor;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class PreviewerSamplerBase
    {
        private PreviewBase _previewBase;
        private INBCActionController _inbcActionController;
        private GameObject _previewGroupRoot;

        public virtual void SelfDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        public virtual void Init(INBCActionController target, PreviewBase previewBase, GameObject previewGroupRoot)
        {
            SceneView.duringSceneGui += OnSceneGUI;
            this._inbcActionController = target;
            this._previewBase = previewBase;
            _previewGroupRoot = previewGroupRoot;
        }

        /// <summary>
        /// 1.UpdateHandle
        /// </summary>
        /// <param name="sceneView"></param> 
        public virtual void OnSceneGUI(SceneView sceneView)
        {
            if (_previewBase == null || _inbcActionController == null || this == null)
            {
                SceneView.duringSceneGui -= OnSceneGUI;
                return;
            }

            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                return;
            }

            if (!_previewBase.IsInPreview)
            {
                UpdateHiddenHandle();
                return;
            }

            PaintHandle();
        }

        public virtual void PaintHandle()
        {
        }

        public virtual void UpdateHiddenHandle()
        {
        }

        protected PreviewBase GetPreviewBase() => _previewBase;

        protected TClip GetClip<TClip>()
            where TClip : IDirectable
            => (TClip)_previewBase.Directable;

        protected T GetActionController<T>()
            where T : INBCActionController
            => (T)_inbcActionController;

        protected Transform GetPreviewGroupRoot() => _previewGroupRoot.transform;
    }
}