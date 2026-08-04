using System.Collections.Generic;
using UnityEngine;

namespace HLMLabs.SceneHub.Editor
{
    internal sealed class SceneHubViewState
    {
        public string SearchFilter = string.Empty;
        public Vector2 ScrollPosition;
        public bool FocusCurrentSceneRequested;
        public string FocusCurrentSceneSectionId;
        public double HighlightCurrentUntil;
    }

    internal readonly struct SceneHubSceneRow
    {
        public string Path { get; }
        public string DisplayName { get; }
        public int BuildIndex { get; }
        public bool IsInBuildSettings { get; }
        public bool IsCurrent { get; }
        public bool IsDefault { get; }
        public bool IsFavorite { get; }
        public bool IsDirty { get; }

        public SceneHubSceneRow(
            string path,
            string displayName,
            int buildIndex,
            bool isInBuildSettings,
            bool isCurrent,
            bool isDefault,
            bool isFavorite,
            bool isDirty)
        {
            Path = path;
            DisplayName = displayName;
            BuildIndex = buildIndex;
            IsInBuildSettings = isInBuildSettings;
            IsCurrent = isCurrent;
            IsDefault = isDefault;
            IsFavorite = isFavorite;
            IsDirty = isDirty;
        }
    }

    internal sealed class SceneHubSectionViewModel
    {
        public string Id { get; }
        public string Title { get; }
        public Texture2D Icon { get; }
        public IReadOnlyList<SceneHubSceneRow> Rows { get; }
        public string EmptyMessage { get; }

        public SceneHubSectionViewModel(
            string id,
            string title,
            Texture2D icon,
            IReadOnlyList<SceneHubSceneRow> rows,
            string emptyMessage)
        {
            Id = id;
            Title = title;
            Icon = icon;
            Rows = rows;
            EmptyMessage = emptyMessage;
        }
    }

    internal sealed class SceneHubViewModel
    {
        public string DefaultScene { get; }
        public IReadOnlyList<SceneHubSectionViewModel> Sections { get; }
        public bool HasCurrentScene { get; }

        public SceneHubViewModel(string defaultScene, IReadOnlyList<SceneHubSectionViewModel> sections)
        {
            DefaultScene = defaultScene;
            Sections = sections;
            HasCurrentScene = ContainsCurrentScene(sections);
        }

        private static bool ContainsCurrentScene(IReadOnlyList<SceneHubSectionViewModel> sections)
        {
            for (var i = 0; i < sections.Count; i++)
            {
                var rows = sections[i].Rows;
                for (var j = 0; j < rows.Count; j++)
                {
                    if (rows[j].IsCurrent)
                        return true;
                }
            }

            return false;
        }
    }
}
