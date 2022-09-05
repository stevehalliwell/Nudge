using UnityEditor;
using UnityEngine;

namespace AID.Nudge
{
    public static class NudgeMenuItems
    {
        [MenuItem("GameObject/Comment", false, 20)]
        public static void CreateSceneComment()
        {
            var settings = Settings.instance;
            //as per doco https://docs.unity3d.com/ScriptReference/Selection-transforms.html this gives only scene objects
            var selectedTrans = Selection.transforms;

            //should determine if this is in the scene or project?
            var newComment = new GameObject(settings.defaultCommentName, typeof(CommentGameObject)).GetComponent<CommentGameObject>();
            newComment.tag = "EditorOnly";
            newComment.normalTextColor = settings.defaultNormalTextColor;
            newComment.hoverTextColor = settings.defaultHoverTextColor;

            if (selectedTrans != null && selectedTrans.Length > 0)
            {
                newComment.comment.SetSelectedItems(selectedTrans);
                newComment.gameObject.name = string.Format(settings.defaultTargetedCommentFormat, selectedTrans[0].gameObject.name);
            }

            Undo.RegisterCreatedObjectUndo(newComment.gameObject, $"Created: {newComment.gameObject.name}");

            Selection.activeObject = newComment;
        }

        [MenuItem("Assets/Create/Comment")]
        public static void CreateAssetComment()
        {
            var settings = Settings.instance;

            var newComment = ScriptableObject.CreateInstance<CommentScriptableObject>();
            newComment.name = settings.defaultCommentName;

            var selectedAssets = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets);

            if (selectedAssets != null && selectedAssets.Length > 0)
            {
                Debug.Log($"{newComment.comment} {selectedAssets}");
                newComment.comment.SetSelectedItems(selectedAssets);
                newComment.name = string.Format(settings.defaultTargetedCommentFormat, selectedAssets[0].name);
            }

            ProjectWindowUtil.CreateAsset(newComment, newComment.name + ".asset");
            ProjectWindowUtil.ShowCreatedAsset(newComment);

            //no undo for creating assets
        }
    }
}
