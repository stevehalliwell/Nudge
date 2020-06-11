using UnityEngine;

namespace AID
{
    [System.Serializable]
    public class Comment
    {
        private const string StartingBody = "...\n...";

        public UnityEngine.Object linkedObject;
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