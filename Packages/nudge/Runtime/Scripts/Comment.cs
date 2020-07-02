using UnityEngine;

namespace AID
{
    public interface ICommentHolder
    {
        Comment Comment { get; }
        string Name { get; }
        UnityEngine.Object UnityObject { get; }
    }

    [System.Serializable]
    public class Comment
    {
        private const string StartingBody = "...\n...";

        public UnityEngine.Object linkedObject;
        static public bool DrawLinkedObjectConnection;
        public bool hidden = false;
        public int priority;
        public bool isTask = false;
        public string body = StartingBody;
        [HideInInspector] public string dateCreated;

        public void ValidateCreationDate()
        {
            if (string.IsNullOrEmpty(dateCreated))
                dateCreated = System.DateTime.Now.ToString("O");
        }
    }
}