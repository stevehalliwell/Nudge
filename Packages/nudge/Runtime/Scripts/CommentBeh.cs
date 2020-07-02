using UnityEngine;

namespace AID
{
    /// <summary>
    /// Comment object as monobehaviour to allow for directly attaching as components or within the scene
    /// making direct reference to other elements in the scene itself via the Unity.Object reference.
    /// </summary>
    public class CommentBeh : MonoBehaviour, ICommentHolder
    {
        public Comment comment;

        public Comment Comment => comment;

        public string Name => gameObject.name;
        public Object UnityObject => this;

        public virtual void OnValidate()
        {
            if (comment == null)
                comment = new Comment();

            comment.ValidateCreationDate();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Packages/Nudge/Gizmos/CommentBeh Icon.png", true);
            if(comment.linkedObject != null)
            {
                Transform linkedTransform = null;
                if(comment.linkedObject is GameObject)
                {
                    linkedTransform = (comment.linkedObject as GameObject).transform;
                }
                else if(comment.linkedObject is Component)
                {
                    linkedTransform = (comment.linkedObject as Component).transform;
                }

                if (linkedTransform != null && Comment.DrawLinkedObjectConnection)
                {
                    Gizmos.DrawIcon(linkedTransform.position, "Packages/Nudge/Gizmos/CommentBehLink Icon.png", true);
                    var prevCol = Gizmos.color;
                    Gizmos.color = Color.grey;
                    Gizmos.DrawLine(transform.position, linkedTransform.position);
                    Gizmos.color = prevCol;
                }
            }
        }
    }
}