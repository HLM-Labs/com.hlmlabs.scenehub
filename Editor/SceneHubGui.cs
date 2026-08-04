using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HLMLabs.SceneHub.Editor
{
    internal static class SceneHubGui
    {
        private const float RowHeight = 26f;
        private const float RowButtonSize = 22f;
        private const float RowButtonWidth = 28f;
        private const double CurrentSceneHighlightSeconds = 1.25d;

        public static void HandleKeyboardShortcuts(Action refreshCatalog, SceneHubViewState viewState)
        {
            var e = Event.current;
            if (e.type != EventType.KeyDown)
                return;

            if (e.keyCode == KeyCode.F5)
            {
                refreshCatalog();
                e.Use();
            }
            else if (e.keyCode == KeyCode.Escape)
            {
                viewState.SearchFilter = string.Empty;
                GUI.FocusControl(null);
                e.Use();
            }
        }

        public static void DrawToolbar(
            SceneHubController controller,
            SceneHubViewState viewState,
            Action refreshCatalog,
            string currentScenePath)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            viewState.SearchFilter = EditorGUILayout.TextField(
                viewState.SearchFilter,
                EditorStyles.toolbarSearchField,
                GUILayout.MinWidth(100));

            if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.Width(18)))
            {
                viewState.SearchFilter = string.Empty;
                GUI.FocusControl(null);
            }

            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(currentScenePath));
            if (GUILayout.Button(
                    new GUIContent(SceneHubResources.SceneIcon, "Locate Current Scene"),
                    EditorStyles.toolbarButton,
                    GUILayout.Width(28)))
            {
                RequestFocusCurrentScene(viewState);
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button(new GUIContent(SceneHubResources.RefreshIcon, "Refresh (F5)"), EditorStyles.toolbarButton, GUILayout.Width(28)))
                refreshCatalog();

            if (GUILayout.Button(new GUIContent(SceneHubResources.CreateIcon, "Create New Scene"), EditorStyles.toolbarButton, GUILayout.Width(28)))
                controller.Operations.CreateNewScene();

            EditorGUILayout.EndHorizontal();
        }

        public static void RequestFocusCurrentScene(SceneHubViewState viewState)
        {
            viewState.SearchFilter = string.Empty;
            viewState.FocusCurrentSceneRequested = true;
            viewState.FocusCurrentSceneSectionId = null;
            viewState.HighlightCurrentUntil = EditorApplication.timeSinceStartup + CurrentSceneHighlightSeconds;
            GUI.FocusControl(null);
        }

        public static void PrepareFocusCurrentScene(
            SceneHubController controller,
            SceneHubViewModel viewModel,
            SceneHubViewState viewState)
        {
            if (!viewModel.HasCurrentScene)
                return;

            var section = FindPreferredCurrentSceneSection(viewModel);
            if (section == null)
                return;

            viewState.FocusCurrentSceneSectionId = section.Id;

            if (controller.Preferences.IsSectionCollapsed(section.Id))
                controller.Preferences.ToggleSectionCollapsed(section.Id, false);
        }

        public static void DrawSections(
            SceneHubController controller,
            SceneHubViewModel viewModel,
            SceneHubViewState viewState)
        {
            var highlightCurrent = EditorApplication.timeSinceStartup < viewState.HighlightCurrentUntil;

            for (var i = 0; i < viewModel.Sections.Count; i++)
            {
                var section = viewModel.Sections[i];

                SceneHubUI.BeginPanel();

                if (SceneHubUI.DrawSectionHeader(
                        controller.Preferences,
                        section.Title,
                        section.Icon,
                        section.Rows.Count,
                        section.Id))
                {
                    if (section.Rows.Count == 0 && !string.IsNullOrEmpty(section.EmptyMessage))
                        SceneHubUI.BodyText(section.EmptyMessage);
                    else
                        DrawSceneRows(controller, section, viewState, highlightCurrent);
                }

                SceneHubUI.EndPanel();

                if (i < viewModel.Sections.Count - 1)
                    EditorGUILayout.Space(4);
            }
        }

        private static SceneHubSectionViewModel FindPreferredCurrentSceneSection(SceneHubViewModel viewModel)
        {
            SceneHubSectionViewModel buildSection = null;
            SceneHubSectionViewModel otherSection = null;
            SceneHubSectionViewModel fallbackSection = null;

            for (var i = 0; i < viewModel.Sections.Count; i++)
            {
                var section = viewModel.Sections[i];
                if (!SectionContainsCurrentScene(section))
                    continue;

                if (section.Id == SceneHubConstants.SectionBuild)
                    buildSection = section;
                else if (section.Id == SceneHubConstants.SectionOther)
                    otherSection = section;
                else if (fallbackSection == null)
                    fallbackSection = section;
            }

            return buildSection ?? otherSection ?? fallbackSection;
        }

        private static bool SectionContainsCurrentScene(SceneHubSectionViewModel section)
        {
            for (var i = 0; i < section.Rows.Count; i++)
            {
                if (section.Rows[i].IsCurrent)
                    return true;
            }

            return false;
        }

        private static void DrawSceneRows(
            SceneHubController controller,
            SceneHubSectionViewModel section,
            SceneHubViewState viewState,
            bool highlightCurrent)
        {
            for (var i = 0; i < section.Rows.Count; i++)
                DrawSceneRow(controller, section.Rows[i], i % 2 == 0, viewState, highlightCurrent, section.Id);
        }

        private static void DrawSceneRow(
            SceneHubController controller,
            SceneHubSceneRow row,
            bool isEvenRow,
            SceneHubViewState viewState,
            bool highlightCurrent,
            string sectionId)
        {
            SceneHubUI.EnsureInitialized();

            var rowRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(RowHeight));
            var shouldHighlight = row.IsCurrent && highlightCurrent;

            if (shouldHighlight)
                SceneHubUI.DrawCurrentRowHighlight(rowRect);
            else if (row.IsDefault)
                SceneHubUI.DrawDefaultRowChrome(rowRect);
            else
                EditorGUI.DrawRect(rowRect, SceneHubUI.GetRowColor(row.IsCurrent, isDefault: false, isEvenRow));

            var isFocusTargetSection = string.IsNullOrEmpty(viewState.FocusCurrentSceneSectionId)
                || viewState.FocusCurrentSceneSectionId == sectionId;

            if (row.IsCurrent &&
                viewState.FocusCurrentSceneRequested &&
                isFocusTargetSection &&
                Event.current.type == EventType.Repaint)
            {
                GUI.ScrollTo(rowRect);
                viewState.FocusCurrentSceneRequested = false;
                viewState.FocusCurrentSceneSectionId = null;
            }

            GUILayout.Space(row.IsDefault ? 6 : 4);

            DrawRowIndicator(row);

            if (row.IsInBuildSettings && row.BuildIndex >= 0)
            {
                GUILayout.Label($"[{row.BuildIndex}]", SceneHubUI.BuildIndexLabel, GUILayout.Width(24));
            }
            else if (row.IsInBuildSettings)
            {
                GUILayout.Space(24);
            }

            var nameStyle = SceneHubUI.GetSceneNameStyle(row.IsCurrent, row.IsDefault);
            var labelRect = GUILayoutUtility.GetRect(new GUIContent(row.DisplayName), nameStyle, GUILayout.MinWidth(80));

            HandleSceneNameInput(controller, row, labelRect);

            EditorGUI.LabelField(labelRect, new GUIContent(row.DisplayName, row.Path), nameStyle);

            if (row.IsDefault)
            {
                GUILayout.Space(4);
                SceneHubUI.DrawDefaultBadge();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent(SceneHubResources.SceneIcon, "Open Scene"), GUILayout.Width(RowButtonWidth), GUILayout.Height(RowButtonSize)))
                controller.Operations.OpenScene(row.Path);

            if (GUILayout.Button(new GUIContent(SceneHubResources.PlayIcon, "Play Scene"), GUILayout.Width(RowButtonWidth), GUILayout.Height(RowButtonSize)))
                controller.Operations.RunScene(row.Path);

            if (GUILayout.Button(new GUIContent("⋮", "More Options"), GUILayout.Width(RowButtonWidth), GUILayout.Height(RowButtonSize)))
                ShowSceneContextMenu(controller, row);

            GUILayout.Space(4);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawRowIndicator(SceneHubSceneRow row)
        {
            if (row.IsDefault)
            {
                GUILayout.Label("★", SceneHubUI.DefaultStarLabel, GUILayout.Width(16));
                return;
            }

            if (row.IsCurrent)
            {
                var indicatorStyle = row.IsDirty
                    ? SceneHubUI.DirtyIndicatorLabel
                    : SceneHubUI.CurrentIndicatorLabel;
                var indicator = row.IsDirty ? "●" : "►";
                GUILayout.Label(indicator, indicatorStyle, GUILayout.Width(14));
            }
            else
            {
                GUILayout.Space(14);
            }
        }

        private static void HandleSceneNameInput(SceneHubController controller, SceneHubSceneRow row, Rect labelRect)
        {
            var e = Event.current;
            if (!labelRect.Contains(e.mousePosition))
                return;

            if (e.type == EventType.ContextClick ||
                (e.type == EventType.MouseDown && e.button == 1))
            {
                ShowSceneContextMenu(controller, row);
                e.Use();
                return;
            }

            if (e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 2)
            {
                controller.Operations.OpenScene(row.Path);
                e.Use();
            }
        }

        private static void ShowSceneContextMenu(SceneHubController controller, SceneHubSceneRow row)
        {
            var menu = new GenericMenu();
            var preferences = controller.Preferences;
            var operations = controller.Operations;

            if (row.IsInBuildSettings)
            {
                var defaultLabel = row.IsDefault
                    ? "★ Default Scene (click to unset)"
                    : "☆ Set as Default Scene";

                menu.AddItem(new GUIContent(defaultLabel), row.IsDefault, () =>
                {
                    preferences.DefaultScene = row.IsDefault ? string.Empty : row.Path;
                });

                menu.AddSeparator(string.Empty);
            }

            var favoriteLabel = row.IsFavorite ? "♥ Remove from Favorites" : "♡ Add to Favorites";
            menu.AddItem(new GUIContent(favoriteLabel), row.IsFavorite, () => preferences.ToggleFavorite(row.Path));

            var buildLabel = row.IsInBuildSettings ? "Remove from Build Settings" : "Add to Build Settings";
            menu.AddItem(new GUIContent(buildLabel), row.IsInBuildSettings, () => operations.ToggleBuildSettings(row.Path));

            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Open as Additive"), false, () => operations.OpenSceneAdditive(row.Path));
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Locate in Project"), false, () => operations.LocateScene(row.Path));
            menu.AddItem(new GUIContent("Duplicate Scene"), false, () => operations.DuplicateScene(row.Path));
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Delete Scene"), false, () =>
                operations.DeleteScene(row.Path, row.IsCurrent));

            menu.ShowAsContext();
        }
    }
}
