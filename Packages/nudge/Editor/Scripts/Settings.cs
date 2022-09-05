using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

//https://docs.unity3d.com/2018.3/Documentation/ScriptReference/SettingsProvider.html

namespace AID.Nudge
{
    [FilePath("ProjectSettings/NudgeSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class Settings : ScriptableSingleton<Settings>
    {
        public enum SortMode
        {
            DateCreated,
            Body,
            Priority,
            ParentObjectName,
        }

        public bool showHidden = false;
        public bool onlyShowTasks = false;
        public bool constantRecache = true;
        public SortMode sortMode = SortMode.DateCreated;
        public Color hiddenTint = new Color(0.8f, 0.8f, 0.8f, 1);
        public Color isTaskTint = new Color(0.8f, 1, 0.8f, 1);
        public Color linkedTint = new Color(0.8f, 0.8f, 1, 1);
        public Color defaultNormalTextColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public Color defaultHoverTextColor = Color.white;
        public string defaultCommentName = "New Comment";
        public string defaultTargetedCommentFormat = "Comment On {0}";

        [Tooltip("Draws, in the scene, an icon and line between a Scene Comment and the object to which it is refering.")]
        public bool drawLinkedConnection = true;

        public string sceneCommentGizmoPath = "Packages/net.sam-tak.nudge/Gizmos/CommentIcon.png";
        public string commentLinkedGizmoPath = "Packages/net.sam-tak.nudge/Gizmos/CommentLinkIcon.png";
        public string commentTaskGizmoPath = "Packages/net.sam-tak.nudge/Gizmos/CommentTaskIcon.png";
        public string commentTaskLinkedGizmoPath = "Packages/net.sam-tak.nudge/Gizmos/CommentTaskLinkIcon.png";

        public void Save() => Save(true);
    }

    public class NudgeSettingsProvider : SettingsProvider
    {
        public NudgeSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) { }
        
        private Editor _editor;

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // This function is called when the user clicks on the MyCustom element in the Settings window.
            var settings = Settings.instance;
            // let scriptable object be editable
            settings.hideFlags = HideFlags.HideAndDontSave & ~HideFlags.NotEditable;
            Editor.CreateCachedEditor(settings, null, ref _editor);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUI.BeginChangeCheck();
            // show default inspector
            _editor.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                // save if needed
                Settings.instance.Save();
            }
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateNudgeSettingsProvider()
        {
            var provider = new NudgeSettingsProvider("Project/Nudge Settings", SettingsScope.Project);
            provider.keywords = GetSearchKeywordsFromSerializedObject(new SerializedObject(Settings.instance));
            return provider;
        }
    }
}
