
using System.Collections.Generic;
using UnityEngine;
namespace DialogueControl
{
    [CreateAssetMenu(fileName = "New DialogueData", menuName = "DialogueSystem/New DialogueData")]
    public class DialogueData : ScriptableObject
    {
        public List<DialogueLines> dialogueLines = new List<DialogueLines>();
    }
}

