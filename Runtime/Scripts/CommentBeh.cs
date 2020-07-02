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
    }
}