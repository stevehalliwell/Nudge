using UnityEngine;

namespace AID
{
    [CreateAssetMenu()]
    public class CommentSO : ScriptableObject
    {
        public Comment comment;

#if UNITY_2020_1_OR_NEWER
        public virtual void OnValidate()
#else

        public virtual void OnEnable()
#endif
        {
            if (comment == null)
                comment = new Comment();

            comment.ValidateCreationDate();
        }
    }
}