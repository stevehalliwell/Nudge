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

#pragma warning disable CS0649
        [SerializeField] private UnityEngine.Object linkedObject;
        [SerializeField] private int priority;
#pragma warning restore CS0649
        [SerializeField] private bool hidden = false;
        [SerializeField] private bool isTask = false;
        [SerializeField] private string body = StartingBody;
        [HideInInspector] [SerializeField] private string dateCreated;
        [HideInInspector] [SerializeField] private string guidString = System.Guid.NewGuid().ToString("N");

        public void ValidateInternalData()
        {
            if (string.IsNullOrEmpty(dateCreated))
                dateCreated = System.DateTime.Now.ToString("O");

            if (string.IsNullOrEmpty(guidString))
                guidString = System.Guid.NewGuid().ToString("N");
        }

        public UnityEngine.Object LinkedObject => linkedObject;
        public bool Hidden => hidden;
        public int Priority => priority;
        public bool IsTask => isTask;
        public string Body => body;
        public string DateCreated => dateCreated;
        public string GUIDString => guidString;
    }
}