using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AID
{
    public class NudgeEditorWindow : EditorWindow
    {
        protected Vector2 wholePanelScrollPos, sceneScrollPos, assetScrollPos;
        protected IComparer<CommentBeh> behComparer;
        protected IComparer<CommentSO> soComparer;

        protected CommentBeh[] allCommentBeh;
        protected List<CommentBeh> sortedCommentBeh;
        protected List<CommentSO> allCommentSO;
        protected List<CommentSO> sortedCommentSO;

        protected NudgeSettings nudgeSettings;

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
            behComparer = new CommentBehDateCreatedSort();
            soComparer = new CommentSODateCreatedSort();
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
            sortedCommentBeh = allCommentBeh.ToList();
            sortedCommentBeh.Sort(behComparer);

            sortedCommentSO = new List<CommentSO>(allCommentSO);
            sortedCommentSO.Sort(soComparer);
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
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
                    behComparer = new CommentBehDateCreatedSort();
                    soComparer = new CommentSODateCreatedSort();
                    break;

                case NudgeSettings.SortMode.Body:
                    behComparer = new CommentBehBodySort();
                    soComparer = new CommentSOBodySort();
                    break;

                case NudgeSettings.SortMode.ParentObjectName:
                    behComparer = new CommentBehGameObjectnameAlphaNumericSort();
                    soComparer = new CommentSONameAlphaNumericSort();
                    break;

                case NudgeSettings.SortMode.Priority:
                    behComparer = new CommentBehPrioritySort();
                    soComparer = new CommentSOPrioritySort();
                    break;

                default:
                    break;
                }

                RunFilters();
            }

            bool foundNull = false;
            var origCol = GUI.color;
            const int maxItems = 10;

            EditorGUILayout.Space();
            wholePanelScrollPos = EditorGUILayout.BeginScrollView(wholePanelScrollPos);
            {
                EditorGUILayout.PrefixLabel("Scene Comments");
                if (sortedCommentBeh != null && sortedCommentBeh.Count > 0)
                {
                    sceneScrollPos = EditorGUILayout.BeginScrollView(sceneScrollPos, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight * maxItems));
                    foreach (var item in sortedCommentBeh)
                    {
                        if (item == null)
                        {
                            foundNull = true;
                            continue;
                        }

                        if (item.comment.hidden)
                        {
                            if (!nudgeSettings.showHidden)
                                continue;

                            GUI.color *= nudgeSettings.hiddenTint;
                        }

                        if (!item.comment.isTask && nudgeSettings.onlyShowTasks)
                            continue;

                        if (item.comment.isTask)
                            GUI.color *= nudgeSettings.isTaskTint;

                        EditorGUILayout.ObjectField(item, typeof(CommentBeh), true);
                        GUI.color = origCol;
                    }
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    EditorGUILayout.LabelField("None Found.");
                }

                EditorGUILayout.Space();
                EditorGUILayout.PrefixLabel("Project Comments");
                if (sortedCommentSO != null && sortedCommentSO.Count > 0)
                {
                    assetScrollPos = EditorGUILayout.BeginScrollView(assetScrollPos, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight * maxItems));
                    foreach (var item in sortedCommentSO)
                    {
                        if (item == null)
                        {
                            foundNull = true;
                            continue;
                        }

                        if (item.comment.hidden)
                        {
                            if (!nudgeSettings.showHidden)
                                continue;

                            GUI.color = nudgeSettings.hiddenTint;
                        }

                        if (!item.comment.isTask && nudgeSettings.onlyShowTasks)
                            continue;

                        if (item.comment.isTask)
                            GUI.color *= nudgeSettings.isTaskTint;

                        EditorGUILayout.ObjectField(item, typeof(CommentBeh), true);
                        GUI.color = origCol;
                    }
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    EditorGUILayout.LabelField("None Found.");
                }

                //var scriptGuids = AssetDatabase.FindAssets("t:script");
                //var scriptFilePaths = scriptGuids.Select(x => AssetDatabase.GUIDToAssetPath(x));
                //var filesOfInterest = new List<string>();
                //find all todos, hacks, fixme,

                //foreach (var item in scriptFilePaths)
                //{
                //}

                //UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(classPath, line);
            }
            EditorGUILayout.EndScrollView();

            if (foundNull || nudgeSettings.constantRecache)
            {
                Recache();
            }
        }
    }

    #region Comparers

    internal class CommentBehGameObjectnameAlphaNumericSort : IComparer<CommentBeh>
    {
        public int Compare(CommentBeh lhs, CommentBeh rhs)
        {
            if (lhs == rhs) return 0;
            if (lhs == null) return -1;
            if (rhs == null) return 1;

            return EditorUtility.NaturalCompare(lhs.gameObject.name, rhs.gameObject.name);
        }
    }

    internal class CommentSONameAlphaNumericSort : IComparer<CommentSO>
    {
        public int Compare(CommentSO lhs, CommentSO rhs)
        {
            if (lhs == rhs) return 0;
            if (lhs == null) return -1;
            if (rhs == null) return 1;

            return EditorUtility.NaturalCompare(lhs.name, rhs.name);
        }
    }

    internal class CommentBehDateCreatedSort : IComparer<CommentBeh>
    {
        public int Compare(CommentBeh lhs, CommentBeh rhs)
        {
            if (lhs == rhs) return 0;
            if (lhs == null) return -1;
            if (rhs == null) return 1;

            var lhsDT = DateTime.Parse(lhs.comment.dateCreated);
            var rhsDT = DateTime.Parse(rhs.comment.dateCreated);

            return -DateTime.Compare(lhsDT, rhsDT);
        }
    }

    internal class CommentSODateCreatedSort : IComparer<CommentSO>
    {
        public int Compare(CommentSO lhs, CommentSO rhs)
        {
            if (lhs == rhs) return 0;
            if (lhs == null) return -1;
            if (rhs == null) return 1;

            var lhsDT = DateTime.Parse(lhs.comment.dateCreated);
            var rhsDT = DateTime.Parse(rhs.comment.dateCreated);

            return -DateTime.Compare(lhsDT, rhsDT);
        }
    }

    internal class CommentBehBodySort : IComparer<CommentBeh>
    {
        public int Compare(CommentBeh lhs, CommentBeh rhs)
        {
            if (lhs == rhs) return 0;
            if (lhs == null) return -1;
            if (rhs == null) return 1;

            return string.Compare(lhs.comment.body, rhs.comment.body);
        }
    }

    internal class CommentSOBodySort : IComparer<CommentSO>
    {
        public int Compare(CommentSO lhs, CommentSO rhs)
        {
            if (lhs == rhs) return 0;
            if (lhs == null) return -1;
            if (rhs == null) return 1;

            return string.Compare(lhs.comment.body, rhs.comment.body);
        }
    }

    internal class CommentBehPrioritySort : IComparer<CommentBeh>
    {
        public int Compare(CommentBeh lhs, CommentBeh rhs)
        {
            if (lhs == rhs) return 0;
            if (lhs == null) return -1;
            if (rhs == null) return 1;

            return -lhs.comment.priority.CompareTo(rhs.comment.priority);
        }
    }

    internal class CommentSOPrioritySort : IComparer<CommentSO>
    {
        public int Compare(CommentSO lhs, CommentSO rhs)
        {
            if (lhs == rhs) return 0;
            if (lhs == null) return -1;
            if (rhs == null) return 1;

            return -lhs.comment.priority.CompareTo(rhs.comment.priority);
        }
    }

    #endregion Comparers
}