using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AID.Nudge
{
    public class EditorWindow : UnityEditor.EditorWindow
    {
        protected Vector2 wholePanelScrollPos, sceneScrollPos, projectScrollPos;
        protected IComparer<ICommentHolder> sortingComparer;

        protected CommentGameObject[] allCommentGO;
        protected List<ICommentHolder> sortedCommentGO;
        protected CommentScriptableObject[] allCommentSO;
        protected List<ICommentHolder> sortedCommentSO;
        protected string searchString;

        protected Settings settings;

        protected enum WindowTabs
        {
            Scene,
            Project,
        }

        protected WindowTabs windowTabs;
        private string[] windowTabNames = System.Enum.GetNames(typeof(WindowTabs));
        private bool foundNull;
        protected int sceneCommentsThatPassFilters = -1, projectCommentsThatPassFilters = -1;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Tasks and Comments")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = UnityEditor.EditorWindow.GetWindow(typeof(EditorWindow)) as EditorWindow;
            window.Show();
        }

        private void OnEnable()
        {
            settings = Settings.instance;
            titleContent = new GUIContent("Tasks & Comments");
            sortingComparer = new CommentHolderDateCreatedSort();
            Recache();
        }

        protected void Recache()
        {
            allCommentGO = FindObjectsOfType<CommentGameObject>();

            var commentSOGuids = AssetDatabase.FindAssets("t:" + nameof(CommentScriptableObject));
            allCommentSO = commentSOGuids.Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath(x, typeof(CommentScriptableObject)) as CommentScriptableObject).ToArray();

            RunFilters();
        }

        protected void RunFilters()
        {
            sortedCommentGO = allCommentGO.ToList<ICommentHolder>();
            sortedCommentGO.Sort(sortingComparer);
            sceneCommentsThatPassFilters = sortedCommentGO.Count(x => PassesFilter(x));

            sortedCommentSO = new List<ICommentHolder>(allCommentSO);
            sortedCommentSO.Sort(sortingComparer);
            projectCommentsThatPassFilters = sortedCommentSO.Count(x => PassesFilter(x));

            windowTabNames = new string[]
            {
                string.Format("Scene - {0} [{1}]", sceneCommentsThatPassFilters, sortedCommentGO.Count),
                string.Format("Project - {0} [{1}]", projectCommentsThatPassFilters, sortedCommentSO.Count)
            };
        }

        private bool PassesFilter(ICommentHolder item)
        {
            if (item == null) return false;
            if (item.Comment.hidden && !settings.showHidden) return false;
            if (!item.Comment.isTask && settings.onlyShowTasks) return false;
            if (!string.IsNullOrEmpty(searchString) &&
                !item.Comment.guidString.Contains(searchString) &&
                !item.Comment.body.Contains(searchString))
                return false;

            return true;
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            searchString = EditorGUILayout.TextField("Search:", searchString);
            settings.sortMode = (Settings.SortMode)EditorGUILayout.EnumPopup(new GUIContent("Sort by;"), settings.sortMode);
            settings.showHidden = EditorGUILayout.Toggle(new GUIContent("Show Hidden?"), settings.showHidden);
            settings.onlyShowTasks = EditorGUILayout.Toggle(new GUIContent("Only Show Tasks?"), settings.onlyShowTasks);
            EditorGUILayout.BeginHorizontal();
            settings.constantRecache = EditorGUILayout.Toggle(new GUIContent("Always Refresh Cache"), settings.constantRecache);
            if (GUILayout.Button("Refresh Now"))
            {
                Recache();
            }
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                switch (settings.sortMode)
                {
                    case Settings.SortMode.DateCreated:
                        sortingComparer = new CommentHolderDateCreatedSort();
                        break;

                    case Settings.SortMode.Body:
                        sortingComparer = new CommentHolderBodySort();
                        break;

                    case Settings.SortMode.ParentObjectName:
                        sortingComparer = new CommentHolderNameAlphaNumericSort();
                        break;

                    case Settings.SortMode.Priority:
                        sortingComparer = new CommentHolderPrioritySort();
                        break;

                    default:
                    break;
                }

                RunFilters();
            }

            windowTabs = (WindowTabs)GUILayout.Toolbar((int)windowTabs, windowTabNames);

            foundNull = false;

            EditorGUILayout.Space();

            switch (windowTabs)
            {
                case WindowTabs.Scene:
                    DoCommentListScrollView(sortedCommentGO, typeof(CommentGameObject), ref sceneScrollPos);
                    break;

                case WindowTabs.Project:
                    DoCommentListScrollView(sortedCommentSO, typeof(CommentScriptableObject), ref projectScrollPos);
                    break;

                default:
                break;
            }

            if (foundNull || settings.constantRecache)
            {
                Recache();
            }
        }

        private void DoCommentListScrollView(List<ICommentHolder> list, System.Type type, ref Vector2 scrollPos)
        {
            var origCol = GUI.color;
            if (list != null && list.Count > 0)
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                foreach (var item in list)
                {
                    if (item == null)
                    {
                        foundNull = true;
                        continue;
                    }

                    if (!string.IsNullOrEmpty(searchString) &&
                        !item.Comment.guidString.Contains(searchString) &&
                        !item.Comment.body.Contains(searchString))
                        continue;

                    if (item.Comment.hidden)
                    {
                        if (!settings.showHidden)
                            continue;

                        GUI.color *= settings.hiddenTint;
                    }

                    if (!item.Comment.isTask && settings.onlyShowTasks)
                        continue;

                    if (item.Comment.isTask)
                        GUI.color *= settings.isTaskTint;

                    EditorGUILayout.ObjectField(item.UnityObject, type, true);
                    GUI.color = origCol;
                }
                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.LabelField("None Found.");
            }
        }
    }

    #region Comparers

    internal class CommentHolderDateCreatedSort : IComparer<ICommentHolder>
    {
        public int Compare(ICommentHolder lhs, ICommentHolder rhs)
        {
            if (lhs == rhs) return 0;
            if (lhs == null) return -1;
            if (rhs == null) return 1;

            var lhsDT = System.DateTime.Parse(lhs.Comment.dateCreated);
            var rhsDT = System.DateTime.Parse(rhs.Comment.dateCreated);

            return -System.DateTime.Compare(lhsDT, rhsDT);
        }
    }

    internal class CommentHolderNameAlphaNumericSort : IComparer<ICommentHolder>
    {
        public int Compare(ICommentHolder lhs, ICommentHolder rhs)
        {
            if (lhs == rhs) return 0;
            if (lhs == null) return -1;
            if (rhs == null) return 1;

            return EditorUtility.NaturalCompare(lhs.Name, rhs.Name);
        }
    }

    internal class CommentHolderBodySort : IComparer<ICommentHolder>
    {
        public int Compare(ICommentHolder lhs, ICommentHolder rhs)
        {
            if (lhs == rhs) return 0;
            if (lhs == null) return -1;
            if (rhs == null) return 1;

            return string.Compare(lhs.Comment.body, rhs.Comment.body);
        }
    }

    internal class CommentHolderPrioritySort : IComparer<ICommentHolder>
    {
        public int Compare(ICommentHolder lhs, ICommentHolder rhs)
        {
            if (lhs == rhs) return 0;
            if (lhs == null) return -1;
            if (rhs == null) return 1;

            return -lhs.Comment.priority.CompareTo(rhs.Comment.priority);
        }
    }

    #endregion Comparers
}
