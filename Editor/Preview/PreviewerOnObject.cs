using NBC.ActionEditor;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class PreviewerOnObject : MonoBehaviour
    {

        private PreviewBase _previewBase;
        private IActionController _actionController;
        
        public virtual void SelfDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        public virtual void Init()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public void SetPreview(PreviewBase previewBase)
        {
            _previewBase = previewBase;
        }

        public void SetActionController(IActionController actionController)
        {
            _actionController = actionController;
        }

        /// <summary>
        /// 1.UpdateHandle
        /// </summary>
        /// <param name="sceneView"></param>
        public virtual void OnSceneGUI(SceneView sceneView)
        {
            if (_previewBase == null || _actionController == null || this == null)
            {
                SceneView.duringSceneGui -= OnSceneGUI;
                return;
            }

            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                return;
            }
            
            if(!_previewBase.IsInPreview)
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
    }
}