using System;
using UnityEngine;

namespace AID.Nudge
{
    public interface ICommentHolder
    {
        Comment Comment { get; }
        string Name { get; }
        UnityEngine.Object UnityObject { get; }
    }

    [Serializable]
    public class Comment
    {
        public bool hidden;
        public bool isTask;
        public int priority;

        [TextArea(5, 25)]
        public string body = "...\n...";
        [Space]
        public UnityEngine.Object[] linkedObjects = Array.Empty<UnityEngine.Object>();

        [HideInInspector] [SerializeField] public string dateCreated;
        [HideInInspector] [SerializeField] public string guidString = System.Guid.NewGuid().ToString("N");

        public void ValidateInternalData()
        {
            if (string.IsNullOrEmpty(dateCreated)) dateCreated = System.DateTime.Now.ToString("O");
            if (string.IsNullOrEmpty(guidString)) guidString = System.Guid.NewGuid().ToString("N");
        }

        public void SetSelectedItems(UnityEngine.Object[] objs)
        {
            linkedObjects = new UnityEngine.Object[objs.Length];
            for (int i = 0; i < objs.Length; i++) linkedObjects[i] = objs[i];
        }
    }
}
