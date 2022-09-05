using UnityEngine;

namespace AID.Nudge
{
    //This is what we want but more, so we use custom menu items in NudgeMenuItems
    //[CreateAssetMenu( menuName = "Comment %$&c", fileName = "New Comment")]
    public class CommentScriptableObject : ScriptableObject, ICommentHolder
    {
        public Comment comment = new();

        public Comment Comment => comment;

        public string Name => name;

        public Object UnityObject => this;

        public virtual void OnValidate()
        {
            if (comment == null) comment = new();
            comment.ValidateInternalData();
        }

#if UNITY_EDITOR

        [ContextMenu("Copy GUID")]
        public void CopyGUID()
        {
            UnityEditor.EditorGUIUtility.systemCopyBuffer = comment.guidString;
        }

#endif
    }
}
