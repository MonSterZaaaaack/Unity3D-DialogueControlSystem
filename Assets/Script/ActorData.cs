using UnityEngine;

namespace DialogueControl
{
    [CreateAssetMenu(fileName = "New Actor", menuName = "DialogueSystem/New Actor")]
    public class ActorData : ScriptableObject
    {
        public ActorName actorName;
        public string tableName;
        public string tableEntry;
        public int actorId;
        public Sprite normal;
        public Sprite happy;
        public Sprite angry;
    }

}
