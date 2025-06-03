
using System.Collections.Generic;
using UnityEngine;
namespace DialogueControl
{
    [CreateAssetMenu(fileName = "New ActorDatabase", menuName = "DialogueSystem/New ActorDatabase")]
    public class ActorDatabase : ScriptableObject
    {
        public List<ActorData> datas = new List<ActorData>();
    }
}

