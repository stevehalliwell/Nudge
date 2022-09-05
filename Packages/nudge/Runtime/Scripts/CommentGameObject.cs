using UnityEngine;

namespace AID.Nudge
{
    //prevent it from showing up in add component menu it's not that kind of thing
    [AddComponentMenu("")]
    /// <summary>
    /// Comment object as monobehaviour to allow for directly attaching as components or within the scene
    /// making direct reference to other elements in the scene itself via the Unity.Object reference.
    /// </summary>
    public class CommentGameObject : MonoBehaviour, ICommentHolder
    {
        public Comment comment;
        public Color normalTextColor = Color.black;
        public Color hoverTextColor = Color.white;
        public Vector2 anchor = Vector2.one;
        [Range(1, 100)]
        public float textSize = 14f;
        public bool hidesTextInSceneViewport;

        public Comment Comment => comment;
        public string Name => gameObject.name;
        public Object UnityObject => this;

        public virtual void OnValidate()
        {
            if (comment == null) comment = new Comment();
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
