using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

//https://docs.unity3d.com/2018.3/Documentation/ScriptReference/SettingsProvider.html

namespace AID.Editor
{
    public class NudgeSettings : ScriptableObject
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
        [Tooltip("Draws, in the scene, an icon and line between a Scene Comment and the object to which it is refering.")]
        public bool drawLinkedConnection = true;
        public string sceneCommentGizmoPath = "Packages/com.aid.nudge/Gizmos/CommentIcon.png";
        public string commentLinkedGizmoPath = "Packages/com.aid.nudge/Gizmos/CommentLinkIcon.png";
        public string commentTaskGizmoPath = "Packages/com.aid.nudge/Gizmos/CommentTaskIcon.png";
        public string commentTaskLinkedGizmoPath = "Packages/com.aid.nudge/Gizmos/CommentTaskLinkIcon.png";

        public const string DefaultNudgeSettingsPath = "Assets/Editor/NudgeSettings.asset";

        private static NudgeSettings inst;


        internal static NudgeSettings GetOrCreateSettings()
        {
            if (inst == null)
            {
                var found = AssetDatabase.FindAssets("t:NudgeSettings");
                if (found != null && found.Length > 0)
                {
                    if (found.Length > 1)
                    {
                        Debug.LogError("More than 1 NudgeSettings found in project, first found will be used.");
                    }

                    inst = AssetDatabase.LoadAssetAtPath<NudgeSettings>(AssetDatabase.GUIDToAssetPath(found[0]));
                }
                else
                {
                    var settings = ScriptableObject.CreateInstance<NudgeSettings>();
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(DefaultNudgeSettingsPath));
                    AssetDatabase.CreateAsset(settings, DefaultNudgeSettingsPath);
                    AssetDatabase.SaveAssets();
                    inst = settings;
                }
            }

            return inst;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    public class NudgeSettingsProvider : SettingsProvider
    {
        private SerializedObject nudgeSettingsSerializedObject;

        public NudgeSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // This function is called when the user clicks on the MyCustom element in the Settings window.
            nudgeSettingsSerializedObject = NudgeSettings.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            nudgeSettingsSerializedObject.Update();

            var p = nudgeSettingsSerializedObject.GetIterator();
            while (p.NextVisible(true))
            {
                //we don't want them changing the type by accident
                if (p.name == "m_Script")
                    continue;

                EditorGUILayout.PropertyField(p);
            }

            nudgeSettingsSerializedObject.ApplyModifiedProperties();
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateNudgeSettingsProvider()
        {
            var provider = new NudgeSettingsProvider("Project/Nudge Settings", SettingsScope.Project);
            provider.keywords = GetSearchKeywordsFromSerializedObject(NudgeSettings.GetSerializedSettings());
            return provider;
        }
    }
}