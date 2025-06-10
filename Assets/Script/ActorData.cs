using UnityEngine;

namespace DialogueControl
{
    [CreateAssetMenu(fileName = "New Actor", menuName = "DialogueSystem/New Actor")]
    public class ActorData : ScriptableObject
    {
        public string actorName;
        public string tableName;
        public string tableEntry;
        public int actorId;
        public Sprite neutralPortrait;
        public Sprite neutralProfileIcon;
        public Sprite happyPortrait;
        public Sprite happyProfileIcon;
        public Sprite angryPortrait;
        public Sprite angryProfileIcon;
        public Sprite sadPortrait;
        public Sprite sadProfileIcon;
    }

}
