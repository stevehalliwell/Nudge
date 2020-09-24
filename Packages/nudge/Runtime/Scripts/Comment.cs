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

        [SerializeField] private bool isHidden = false;

        [SerializeField] private bool isTask = false;
#pragma warning disable CS0649
        [SerializeField] private int priority;

        [TextArea(5, 25)]
        [SerializeField] private string body = StartingBody;
        [SerializeField] private UnityEngine.Object primaryLinkedObject;
        [SerializeField] private UnityEngine.Object[] additionalLinkedObjects = System.Array.Empty<UnityEngine.Object>();
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

        public void SetSelectedItems(UnityEngine.Object[] objs)
        {
            primaryLinkedObject = objs[0];
            additionalLinkedObjects = new Object[objs.Length - 1];
            for (int i = 1; i < objs.Length; i++)
            {
                additionalLinkedObjects[i - 1] = objs[i];
            }
        }
		
        public UnityEngine.Object PrimaryLinkedObject => primaryLinkedObject;
        public System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Object> AdditionalLinkedObjects => 
            System.Array.AsReadOnly(additionalLinkedObjects);
        public bool Hidden => isHidden;
        public int Priority => priority;
        public bool IsTask => isTask;
        public string Body => body;
        public string DateCreated => dateCreated;
        public string GUIDString => guidString;
    }
}
