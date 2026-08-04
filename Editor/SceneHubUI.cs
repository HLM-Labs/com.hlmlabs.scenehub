using UnityEditor;
using UnityEngine;

namespace HLMLabs.SceneHub.Editor
{
    internal static class SceneHubUI
    {
        private static readonly Color Accent = new Color(0.26f, 0.80f, 0.50f);
        private static readonly Color CardTitleColor = new Color(0.55f, 0.85f, 0.66f);
        private static readonly Color TextPro = new Color(0.72f, 0.73f, 0.76f);
        private static readonly Color TextLite = new Color(0.36f, 0.37f, 0.40f);
        private static readonly Color BodyTextPro = new Color(0.62f, 0.63f, 0.66f);
        private static readonly Color BodyTextLite = new Color(0.36f, 0.37f, 0.40f);
        private static readonly Color WarningText = new Color(1f, 0.42f, 0.40f);
        private static readonly Color SeparatorPro = new Color(1f, 1f, 1f, 0.08f);
        private static readonly Color SeparatorLite = new Color(0f, 0f, 0f, 0.12f);
        private static readonly Color CurrentRowBackgroundPro = new Color(0.26f, 0.80f, 0.50f, 0.18f);
        private static readonly Color CurrentRowBackgroundLite = new Color(0.26f, 0.80f, 0.50f, 0.22f);
        private static readonly Color CurrentRowHighlightPro = new Color(0.26f, 0.80f, 0.50f, 0.38f);
        private static readonly Color CurrentRowHighlightLite = new Color(0.26f, 0.80f, 0.50f, 0.42f);
        private static readonly Color DefaultRowBackgroundPro = new Color(0.95f, 0.72f, 0.18f, 0.20f);
        private static readonly Color DefaultRowBackgroundLite = new Color(0.95f, 0.72f, 0.18f, 0.26f);
        private static readonly Color DefaultStrip = new Color(0.95f, 0.72f, 0.18f, 1f);
        private static readonly Color DefaultBadgeBgPro = new Color(0.95f, 0.72f, 0.18f, 0.28f);
        private static readonly Color DefaultBadgeBgLite = new Color(0.95f, 0.72f, 0.18f, 0.35f);
        private static readonly Color DefaultText = new Color(0.95f, 0.78f, 0.28f);
        private static readonly Color EvenRowBackgroundPro = new Color(1f, 1f, 1f, 0.03f);
        private static readonly Color EvenRowBackgroundLite = new Color(0f, 0f, 0f, 0.03f);
        private static readonly Color OddRowBackground = new Color(0f, 0f, 0f, 0f);
        private static readonly Color DefaultBarEmptyPro = new Color(1f, 1f, 1f, 0.04f);
        private static readonly Color DefaultBarEmptyLite = new Color(0f, 0f, 0f, 0.04f);
        private static readonly Color DefaultBarSetPro = new Color(0.95f, 0.72f, 0.18f, 0.16f);
        private static readonly Color DefaultBarSetLite = new Color(0.95f, 0.72f, 0.18f, 0.22f);

        private const float ContentPadding = 8f;
        private const float DefaultBarHeight = 22f;
        private const float DefaultStripWidth = 3f;

        private static bool _initialized;
        private static GUIStyle _card;
        private static GUIStyle _cardTitle;
        private static GUIStyle _body;
        private static GUIStyle _sectionCount;
        private static GUIStyle _currentIndicatorLabel;
        private static GUIStyle _dirtyIndicatorLabel;
        private static GUIStyle _buildIndexLabel;
        private static GUIStyle _sceneNameNormal;
        private static GUIStyle _sceneNameBold;
        private static GUIStyle _sceneNameDefault;
        private static GUIStyle _defaultStarLabel;
        private static GUIStyle _defaultBarLabel;
        private static GUIStyle _defaultBadgeLabel;

        public static Color Separator => EditorGUIUtility.isProSkin ? SeparatorPro : SeparatorLite;
        public static Color AccentColor => Accent;
        public static Color WarningColor => WarningText;

        public static void EnsureInitialized()
        {
            if (_initialized && _card != null)
                return;

            if (!AreEditorStylesReady())
                return;

            _initialized = true;
            InitStyles();
        }

        private static void InitStyles()
        {
            bool pro = EditorGUIUtility.isProSkin;
            Color textColor = pro ? TextPro : TextLite;
            Color bodyColor = pro ? BodyTextPro : BodyTextLite;

            _card = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 6, 6),
                margin = new RectOffset(0, 0, 2, 2)
            };

            _cardTitle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12,
                normal = { textColor = CardTitleColor }
            };

            _body = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 12,
                normal = { textColor = bodyColor }
            };

            _sectionCount = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = bodyColor }
            };

            _currentIndicatorLabel = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = Accent },
                fontStyle = FontStyle.Bold
            };

            _dirtyIndicatorLabel = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = WarningText },
                fontStyle = FontStyle.Bold
            };

            _buildIndexLabel = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = bodyColor }
            };

            _sceneNameNormal = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                normal = { textColor = textColor }
            };

            _sceneNameBold = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Accent }
            };

            _sceneNameDefault = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = DefaultText }
            };

            _defaultStarLabel = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = DefaultText },
                fontStyle = FontStyle.Bold,
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            _defaultBarLabel = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = bodyColor }
            };

            _defaultBadgeLabel = new GUIStyle(EditorStyles.miniLabel)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 9,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(5, 5, 1, 1),
                normal = { textColor = DefaultText }
            };
        }

        public static void BeginContentArea()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(ContentPadding);
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        }

        public static void EndContentArea()
        {
            EditorGUILayout.EndVertical();
            GUILayout.Space(ContentPadding);
            EditorGUILayout.EndHorizontal();
        }

        public static void BeginPanel()
        {
            EditorGUILayout.BeginVertical(_card);
        }

        public static void EndPanel()
        {
            EditorGUILayout.EndVertical();
        }

        public static void PanelHeading(string text)
        {
            EditorGUILayout.LabelField(text, _cardTitle);
            Rect rect = EditorGUILayout.GetControlRect(false, 1f);
            EditorGUI.DrawRect(rect, Separator);
            EditorGUILayout.Space(3f);
        }

        public static void BodyText(string text)
        {
            EditorGUILayout.LabelField(text, _body);
        }

        public static void DrawDefaultSceneBar(string defaultScene)
        {
            EnsureInitialized();

            bool hasDefault = !string.IsNullOrEmpty(defaultScene);
            bool pro = EditorGUIUtility.isProSkin;
            Color bgColor = hasDefault
                ? (pro ? DefaultBarSetPro : DefaultBarSetLite)
                : (pro ? DefaultBarEmptyPro : DefaultBarEmptyLite);

            var rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(DefaultBarHeight));
            EditorGUI.DrawRect(rect, bgColor);

            GUILayout.Space(8);

            if (!hasDefault)
            {
                EditorGUILayout.LabelField("No default scene set", _defaultBarLabel ?? EditorStyles.miniLabel);
            }
            else
            {
                GUILayout.Label("★", _defaultStarLabel ?? EditorStyles.label, GUILayout.Width(16));
                EditorGUILayout.LabelField(
                    "Default scene: " + SceneHubUtil.GetDisplayName(defaultScene, isDirty: false),
                    _defaultBarLabel ?? EditorStyles.miniLabel);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        public static bool DrawSectionHeader(
            SceneHubPreferences preferences,
            string title,
            Texture2D icon,
            int count,
            string sectionId)
        {
            EnsureInitialized();

            var isCollapsed = preferences.IsSectionCollapsed(sectionId);

            EditorGUILayout.BeginHorizontal();

            var newCollapsed = !EditorGUILayout.Foldout(!isCollapsed, GUIContent.none, true, EditorStyles.foldout);

            GUILayout.Space(-15);

            if (icon != null)
                GUILayout.Label(new GUIContent(icon), GUILayout.Width(18), GUILayout.Height(18));

            GUILayout.Label(title, _cardTitle ?? EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"({count})", _sectionCount ?? EditorStyles.miniLabel);

            EditorGUILayout.EndHorizontal();

            if (newCollapsed != isCollapsed)
                preferences.ToggleSectionCollapsed(sectionId, newCollapsed);

            if (!newCollapsed)
            {
                var lineRect = EditorGUILayout.GetControlRect(false, 1);
                EditorGUI.DrawRect(lineRect, Separator);
            }

            return !newCollapsed;
        }

        public static GUIStyle GetSceneNameStyle(bool isCurrent, bool isDefault)
        {
            EnsureInitialized();

            if (isDefault)
                return _sceneNameDefault ?? EditorStyles.boldLabel;

            return isCurrent
                ? _sceneNameBold ?? EditorStyles.label
                : _sceneNameNormal ?? EditorStyles.label;
        }

        public static GUIStyle CurrentIndicatorLabel
        {
            get
            {
                EnsureInitialized();
                return _currentIndicatorLabel ?? EditorStyles.boldLabel;
            }
        }

        public static GUIStyle DirtyIndicatorLabel
        {
            get
            {
                EnsureInitialized();
                return _dirtyIndicatorLabel ?? EditorStyles.boldLabel;
            }
        }

        public static GUIStyle DefaultStarLabel
        {
            get
            {
                EnsureInitialized();
                return _defaultStarLabel ?? EditorStyles.boldLabel;
            }
        }

        public static GUIStyle BuildIndexLabel
        {
            get
            {
                EnsureInitialized();
                return _buildIndexLabel ?? EditorStyles.miniLabel;
            }
        }

        public static void DrawDefaultRowChrome(Rect rowRect)
        {
            bool pro = EditorGUIUtility.isProSkin;
            EditorGUI.DrawRect(rowRect, pro ? DefaultRowBackgroundPro : DefaultRowBackgroundLite);
            EditorGUI.DrawRect(new Rect(rowRect.x, rowRect.y, DefaultStripWidth, rowRect.height), DefaultStrip);
        }

        public static void DrawDefaultBadge()
        {
            EnsureInitialized();

            var content = new GUIContent("DEFAULT");
            var style = _defaultBadgeLabel ?? EditorStyles.miniLabel;
            var size = style.CalcSize(content);
            var rect = GUILayoutUtility.GetRect(size.x + 2f, size.y, style, GUILayout.Width(size.x + 2f));

            bool pro = EditorGUIUtility.isProSkin;
            EditorGUI.DrawRect(rect, pro ? DefaultBadgeBgPro : DefaultBadgeBgLite);
            GUI.Label(rect, content, style);
        }

        public static Color GetRowColor(bool isCurrentScene, bool isDefault, bool isEvenRow, bool isHighlighted = false)
        {
            bool pro = EditorGUIUtility.isProSkin;

            if (isHighlighted && isCurrentScene)
                return pro ? CurrentRowHighlightPro : CurrentRowHighlightLite;

            if (isDefault)
                return pro ? DefaultRowBackgroundPro : DefaultRowBackgroundLite;

            if (isCurrentScene)
                return pro ? CurrentRowBackgroundPro : CurrentRowBackgroundLite;

            if (isEvenRow)
                return pro ? EvenRowBackgroundPro : EvenRowBackgroundLite;

            return OddRowBackground;
        }

        public static void DrawCurrentRowHighlight(Rect rowRect)
        {
            bool pro = EditorGUIUtility.isProSkin;
            EditorGUI.DrawRect(rowRect, pro ? CurrentRowHighlightPro : CurrentRowHighlightLite);
            EditorGUI.DrawRect(new Rect(rowRect.x, rowRect.y, DefaultStripWidth, rowRect.height), Accent);
        }

        private static bool AreEditorStylesReady()
        {
            try
            {
                return EditorStyles.label != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
