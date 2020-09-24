using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AID.Editor
{
    public class NudgeEditorWindow : EditorWindow
    {
        protected Vector2 wholePanelScrollPos, sceneScrollPos, projectScrollPos;
        protected IComparer<ICommentHolder> sortingComparer;

        protected CommentBeh[] allCommentBeh;
        protected List<ICommentHolder> sortedCommentBeh;
        protected List<CommentSO> allCommentSO;
        protected List<ICommentHolder> sortedCommentSO;
        protected string searchString;

        protected NudgeSettings nudgeSettings;

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
            NudgeEditorWindow window = (NudgeEditorWindow)EditorWindow.GetWindow(typeof(NudgeEditorWindow));
            window.Show();
        }

        private void OnEnable()
        {
            nudgeSettings = NudgeSettings.GetOrCreateSettings();
            titleContent = new GUIContent("Tasks & Comments");
            sortingComparer = new CommentHolderDateCreatedSort();
            Recache();
        }

        protected void Recache()
        {
            allCommentBeh = FindObjectsOfType<CommentBeh>();

            var commentSOGuids = AssetDatabase.FindAssets("t:CommentSO");
            allCommentSO = commentSOGuids.Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath(x, typeof(CommentSO)) as CommentSO).ToList();

            RunFilters();
        }

        protected void RunFilters()
        {
            sortedCommentBeh = allCommentBeh.ToList<ICommentHolder>();
            sortedCommentBeh.Sort(sortingComparer);
            sceneCommentsThatPassFilters = sortedCommentBeh.Count(x => PassesFilter(x));

            sortedCommentSO = new List<ICommentHolder>(allCommentSO);
            sortedCommentSO.Sort(sortingComparer);
            projectCommentsThatPassFilters = sortedCommentSO.Count(x => PassesFilter(x));

            windowTabNames = new string[]
            {
                string.Format("Scene - {0} [{1}]", sceneCommentsThatPassFilters, sortedCommentBeh.Count),
                string.Format("Project - {0} [{1}]", projectCommentsThatPassFilters, sortedCommentSO.Count)
            };
        }

        private bool PassesFilter(ICommentHolder item)
        {
            if (item == null) return false;
            if (item.Comment.Hidden && !nudgeSettings.showHidden) return false;
            if (!item.Comment.IsTask && nudgeSettings.onlyShowTasks) return false;
            if (!string.IsNullOrEmpty(searchString) &&
                !item.Comment.GUIDString.Contains(searchString) &&
                !item.Comment.Body.Contains(searchString))
                return false;

            return true;
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            searchString = EditorGUILayout.TextField("Search:", searchString);
            nudgeSettings.sortMode = (NudgeSettings.SortMode)EditorGUILayout.EnumPopup(new GUIContent("Sort by;"), nudgeSettings.sortMode);
            nudgeSettings.showHidden = EditorGUILayout.Toggle(new GUIContent("Show Hidden?"), nudgeSettings.showHidden);
            nudgeSettings.onlyShowTasks = EditorGUILayout.Toggle(new GUIContent("Only Show Tasks?"), nudgeSettings.onlyShowTasks);
            EditorGUILayout.BeginHorizontal();
            nudgeSettings.constantRecache = EditorGUILayout.Toggle(new GUIContent("Always Refresh Cache"), nudgeSettings.constantRecache);
            if (GUILayout.Button("Refresh Now"))
            {
                Recache();
            }
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                switch (nudgeSettings.sortMode)
                {
                    case NudgeSettings.SortMode.DateCreated:
                        sortingComparer = new CommentHolderDateCreatedSort();
                        break;

                    case NudgeSettings.SortMode.Body:
                        sortingComparer = new CommentHolderBodySort();
                        break;

                    case NudgeSettings.SortMode.ParentObjectName:
                        sortingComparer = new CommentHolderNameAlphaNumericSort();
                        break;

                    case NudgeSettings.SortMode.Priority:
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
                    DoCommentListScrollView(sortedCommentBeh, typeof(CommentBeh), ref sceneScrollPos);
                    break;

                case WindowTabs.Project:
                    DoCommentListScrollView(sortedCommentSO, typeof(CommentSO), ref projectScrollPos);
                    break;

                default:
                break;
            }

            if (foundNull || nudgeSettings.constantRecache)
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
                        !item.Comment.GUIDString.Contains(searchString) &&
                        !item.Comment.Body.Contains(searchString))
                        continue;

                    if (item.Comment.Hidden)
                    {
                        if (!nudgeSettings.showHidden)
                            continue;

                        GUI.color *= nudgeSettings.hiddenTint;
                    }

                    if (!item.Comment.IsTask && nudgeSettings.onlyShowTasks)
                        continue;

                    if (item.Comment.IsTask)
                        GUI.color *= nudgeSettings.isTaskTint;

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

            var lhsDT = System.DateTime.Parse(lhs.Comment.DateCreated);
            var rhsDT = System.DateTime.Parse(rhs.Comment.DateCreated);

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

            return string.Compare(lhs.Comment.Body, rhs.Comment.Body);
        }
    }

    internal class CommentHolderPrioritySort : IComparer<ICommentHolder>
    {
        public int Compare(ICommentHolder lhs, ICommentHolder rhs)
        {
            if (lhs == rhs) return 0;
            if (lhs == null) return -1;
            if (rhs == null) return 1;

            return -lhs.Comment.Priority.CompareTo(rhs.Comment.Priority);
        }
    }

    #endregion Comparers
}
