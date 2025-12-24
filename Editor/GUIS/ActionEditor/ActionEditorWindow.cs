using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class ActionEditorWindow : EditorWindow
    {
        [MenuItem("NBC/Action Editor/Open Action Editor", false, 0)]
        public static void OpenDirectorWindow()
        {
            if (Instance != null)
            {
                return;
            }

            var window = GetWindow(typeof(ActionEditorWindow)) as ActionEditorWindow;
            if (window == null) return;
            window.Show();
        }

        private WelcomeView _welcomeView;
        private TimelineView _timelineView;
        private string _lastEditorTargetPath;
        private string _lastTextAssetPath;


        public static ActionEditorWindow Instance;


        #region Init

        private void InitializeAll()
        {
            Lan.Load();
            Styles.Load();
            Prefs.InitializeAssetTypes();

            App.OnInitialize?.Invoke();
            //停止播放
            if (App.AssetData != null)
            {
                if (!Application.isPlaying)
                {
                    // App.Stop(true);
                }
            }

            _welcomeView = this.CreateView<WelcomeView>();
            _timelineView = this.CreateView<TimelineView>();
            // WillRepaint = true;
        }

        #endregion

        #region Lifecycle

        void OnEnable()
        {
            Instance = this;
            App.Window = this;
            EditorSceneManager.sceneSaving -= OnWillSaveScene;
            EditorSceneManager.sceneSaving += OnWillSaveScene;
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
            titleContent = new GUIContent(Lan.Title);
            minSize = new Vector2(500, 250);
            InitializeAll();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }


        void OnDisable()
        {
            Instance = null;
            App.Window = null;
            EditorSceneManager.sceneSaving -= OnWillSaveScene;
            EditorApplication.update -= OnEditorUpdate;
            App.OnDisable?.Invoke();
            App.Stop();
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            App.TextAsset = null;
        }


        private void TryDelHandler()
        {
            if (App.AssetData == null)
            {
                return;
            }

            //检测 del 按钮
            var e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.Delete)
                {
                    App.DeleteDirectable();
                }
            }
        }

        void OnEditorUpdate()
        {
            this.UpdateViews();
            if (App.NeedForceRefresh)
            {
                this.Repaint();
            }

            App.OnUpdate();
        }

        void OnGUI()
        {
            if (Application.isPlaying)
            {
                //给我一句提示关闭引擎在运行
                GUIStyle centeredBoldLabel = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                    fontSize = 28
                };
                EditorGUILayout.LabelField(Lan.RunningTitle, centeredBoldLabel, GUILayout.Height(30));

                return;
            }

            if (App.AssetData == null)
            {
                // Test();
                _welcomeView.OnGUI(this.position);
                return;
            }

            if (Event.current.type == EventType.MouseMove)
            {
                Debug.Log("MouseMove===11");
                Repaint();
            }

            this.TryDelHandler();
            _timelineView.OnGUI(this.position);
            App.OnGUIEnd();
        }

        void OnWillSaveScene(UnityEngine.SceneManagement.Scene scene, string path)
        {
        }


        private void OnBeforeAssemblyReload()
        {
            SerializeSaveData();
        }


        private void OnAfterAssemblyReload()
        {
            UnSerializeSaveData();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                {
                    UnSerializeSaveData();
                }
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    SerializeSaveData();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                {
                    AssetPlayer.Inst.OnCloseAssets();
                    AssetPlayer.Inst = null;
                    App.TextAsset = null;
                }
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
            }
        }


        private void UnSerializeSaveData()
        {
            if (!string.IsNullOrEmpty(_lastEditorTargetPath))
            {
                var go = GameObject.Find(_lastEditorTargetPath);
                if (go) AssetPlayer.Inst.SelectSceneGameObject = go.GetComponent<INBCActionController>();
            }

            if (!string.IsNullOrEmpty(_lastTextAssetPath))
            {
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(_lastTextAssetPath);
                if (asset) App.TextAsset = asset;
            }
        }

        private void SerializeSaveData()
        {
            if (AssetPlayer.Inst.SelectSceneGameObject != null)
            {
                var go = ((MonoBehaviour)AssetPlayer.Inst.SelectSceneGameObject).gameObject;
                //记录最后的选择
                _lastEditorTargetPath = go.GetScenePath();
            }

            if (App.TextAsset != null)
            {
                _lastTextAssetPath = AssetDatabase.GetAssetPath(App.TextAsset);
            }
        }

        #endregion
    }
}