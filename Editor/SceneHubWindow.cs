using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HLMLabs.SceneHub.Editor
{
    public sealed class SceneHubWindow : EditorWindow
    {
        private SceneHubController controller;
        private readonly SceneHubViewState viewState = new();

        [MenuItem(SceneHubConstants.MenuPath)]
        public static void ShowWindow()
        {
            var window = GetWindow<SceneHubWindow>(SceneHubConstants.WindowTitle);
            window.minSize = new Vector2(320, 250);
        }

        private void OnEnable()
        {
            EnsureController();
            EditorApplication.projectChanged += OnProjectChanged;
            EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;
        }

        private void OnDisable()
        {
            EditorApplication.projectChanged -= OnProjectChanged;
            EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChanged;
        }

        private void EnsureController()
        {
            SceneHubResources.EnsureIconsLoaded();

            if (controller != null)
                return;

            controller = new SceneHubController();
            titleContent = new GUIContent(SceneHubConstants.WindowTitle, SceneHubResources.SceneIcon);
            controller.RefreshCatalog();
        }

        private void OnProjectChanged()
        {
            if (controller == null)
                return;

            controller.RefreshCatalog();
            Repaint();
        }

        private void OnActiveSceneChanged(Scene previousScene, Scene newScene)
        {
            Repaint();
        }

        private void OnGUI()
        {
            EnsureController();
            SceneHubUI.EnsureInitialized();

            if (viewState.FocusCurrentSceneRequested)
                viewState.SearchFilter = string.Empty;

            var activeScene = EditorSceneManager.GetActiveScene();
            var viewModel = controller.BuildViewModel(viewState, activeScene.path, activeScene.isDirty);

            if (viewState.FocusCurrentSceneRequested)
            {
                SceneHubGui.PrepareFocusCurrentScene(controller, viewModel, viewState);
                if (!viewModel.HasCurrentScene)
                {
                    viewState.FocusCurrentSceneRequested = false;
                    viewState.FocusCurrentSceneSectionId = null;
                }
            }

            SceneHubGui.HandleKeyboardShortcuts(RefreshCatalog, viewState);

            EditorGUILayout.Space(4f);
            SceneHubUI.BeginContentArea();

            SceneHubUI.DrawDefaultSceneBar(viewModel.DefaultScene);

            EditorGUILayout.Space(2);
            SceneHubGui.DrawToolbar(controller, viewState, RefreshCatalog, activeScene.path);
            EditorGUILayout.Space(4);

            viewState.ScrollPosition = EditorGUILayout.BeginScrollView(viewState.ScrollPosition);
            SceneHubGui.DrawSections(controller, viewModel, viewState);
            EditorGUILayout.EndScrollView();

            SceneHubUI.EndContentArea();

            if (viewState.HighlightCurrentUntil > EditorApplication.timeSinceStartup)
                Repaint();
        }

        private void RefreshCatalog()
        {
            EnsureController();
            controller.RefreshCatalog();
            Repaint();
        }
    }
}
