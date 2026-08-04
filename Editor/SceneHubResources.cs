using UnityEditor;
using UnityEngine;

namespace HLMLabs.SceneHub.Editor
{
    internal static class SceneHubResources
    {
        private static bool iconsLoaded;

        public static Texture2D SceneIcon { get; private set; }
        public static Texture2D PlayIcon { get; private set; }
        public static Texture2D RefreshIcon { get; private set; }
        public static Texture2D BuildIcon { get; private set; }
        public static Texture2D FolderIcon { get; private set; }
        public static Texture2D FavoriteIcon { get; private set; }
        public static Texture2D RecentIcon { get; private set; }
        public static Texture2D CreateIcon { get; private set; }

        public static void EnsureIconsLoaded()
        {
            if (iconsLoaded)
                return;

            SceneIcon = LoadIcon("d_SceneAsset Icon");
            PlayIcon = LoadIcon("d_PlayButton");
            RefreshIcon = LoadIcon("d_Refresh");
            BuildIcon = LoadIcon("d_BuildSettings.SelectedIcon");
            FolderIcon = LoadIcon("d_Folder Icon");
            FavoriteIcon = LoadIcon("d_Favorite Icon");
            RecentIcon = LoadIcon("d_UnityEditor.AnimationWindow");
            CreateIcon = LoadIcon("d_CreateAddNew");
            iconsLoaded = true;
        }

        private static Texture2D LoadIcon(string iconName)
        {
            return EditorGUIUtility.IconContent(iconName).image as Texture2D;
        }
    }
}
