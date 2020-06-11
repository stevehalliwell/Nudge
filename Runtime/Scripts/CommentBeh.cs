using UnityEngine;

namespace AID
{
    public class CommentBeh : MonoBehaviour
    {
        public Comment comment;

        public virtual void OnValidate()
        {
            if (comment == null)
                comment = new Comment();

            comment.ValidateCreationDate();
        }
    }
}