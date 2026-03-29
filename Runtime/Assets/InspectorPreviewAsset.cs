using UnityEngine;

namespace NBC.ActionEditor
{
    public class InspectorPreviewAsset : ScriptableObject
    {
        [HideInInspector] [SerializeField] private string serializedState;

        public string SerializedState
        {
            get => serializedState;
            set => serializedState = value;
        }
    }
}
