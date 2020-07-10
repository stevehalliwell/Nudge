using UnityEngine;
using UnityEngine.Serialization;

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

        [FormerlySerializedAs("hidden")]
        [SerializeField] private bool isHidden = false;
        [SerializeField] private bool isTask = false;
#pragma warning disable CS0649
        [SerializeField] private int priority;
        [TextArea(5, 25)]
        [SerializeField] private string body = StartingBody;
        [SerializeField] private UnityEngine.Object linkedObject;
#pragma warning restore CS0649


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
        public bool Hidden => isHidden;
        public int Priority => priority;
        public bool IsTask => isTask;
        public string Body => body;
        public string DateCreated => dateCreated;
        public string GUIDString => guidString;
    }
}