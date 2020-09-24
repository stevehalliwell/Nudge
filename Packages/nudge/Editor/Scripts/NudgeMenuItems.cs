using UnityEditor;
using UnityEngine;

namespace AID.Editor
{
    public static class NudgeMenuItems
    {
        [MenuItem("GameObject/Comment %#c", false, 20)]
        public static void CreateSceneComment()
        {
            var nudgeSettings = NudgeSettings.GetOrCreateSettings();
            //as per doco https://docs.unity3d.com/ScriptReference/Selection-transforms.html this gives only scene objects
            var selectedTrans = Selection.transforms;

            //should determine if this is in the scene or project?
            var newComment = new GameObject(nudgeSettings.defaultCommentName, typeof(CommentBeh)).GetComponent<CommentBeh>();

            if (selectedTrans != null && selectedTrans.Length > 0)
            {
                newComment.comment.SetSelectedItems(selectedTrans);
                newComment.gameObject.name = string.Format(nudgeSettings.defaultTargetedCommentFormat, selectedTrans[0].gameObject.name);
            }

            Undo.RegisterCreatedObjectUndo(newComment.gameObject, $"Created: {newComment.gameObject.name}");

            Selection.activeObject = newComment;
        }

        [MenuItem("Assets/Create/CommentSO %#&c")]
        public static void CreateAssetComment()
        {
            var nudgeSettings = NudgeSettings.GetOrCreateSettings();

            var newComment = ScriptableObject.CreateInstance<CommentSO>();
            newComment.name = nudgeSettings.defaultCommentName;

            var selectedAssets = Selection.GetFiltered<Object>(SelectionMode.Assets);

            if (selectedAssets != null && selectedAssets.Length > 0)
            {
                newComment.comment.SetSelectedItems(selectedAssets);
                newComment.name = string.Format(nudgeSettings.defaultTargetedCommentFormat, selectedAssets[0].name);
            }

            ProjectWindowUtil.CreateAsset(newComment, newComment.name + ".asset");
            ProjectWindowUtil.ShowCreatedAsset(newComment);

            //no undo for creating assets
        }
    }
}
