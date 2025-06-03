using System;
using System.Collections.Generic;
namespace DialogueControl
{
    [Serializable]
    public class DialogueLines
    {
        public string tableName;
        public string tableKey;
        public ActorName actorName;
        public ActorExpression actorExpression;
        public bool isOption;
        public List<DialogueOptions> options;
        public ActorName otherActorInScene;
        public int jumpTo = -1;
        public bool isMainCharacterTalking;
        public DialogueLines()
        {
            tableName = string.Empty;
            tableKey = string.Empty;
            actorName = ActorName.None;
            actorExpression = ActorExpression.None;
            isOption = false;
            options = new List<DialogueOptions>();
            otherActorInScene = ActorName.None;
            isMainCharacterTalking = true;
        }
        // Copy Constructor
        public DialogueLines(DialogueLines other)
        {
            tableKey = other.tableKey;
            tableName = other.tableName;
            actorName = other.actorName;
            actorExpression = other.actorExpression;
            isOption = other.isOption;
            options = new List<DialogueOptions>(other.options);
            otherActorInScene = other.otherActorInScene;
        }
    }

}
