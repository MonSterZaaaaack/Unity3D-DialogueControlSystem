using System;
using System.Collections.Generic;
namespace DialogueControl
{
    [Serializable]
    public class DialogueLines
    {
        public string tableName;
        public string tableKey;
        public bool hasOption;
        public ActorInScene mainActor;
        public ActorInScene talkingToActor;
        public List<DialogueOptions> options;
        public bool reAssignPosition;
        public ActorInScene leftSidePosition2Actor;
        public ActorInScene rightSidePosition2Actor;
        public int jumpTo = -1;
        public DialogueLines()
        {
            tableName = string.Empty;
            tableKey = string.Empty;
            hasOption = false;
            mainActor = new ActorInScene();
            talkingToActor = new ActorInScene();
            options = new List<DialogueOptions>();
            reAssignPosition = false;
            leftSidePosition2Actor = null;
            rightSidePosition2Actor = null;
            jumpTo = -1;
            
        }
        // Copy Constructor
        public DialogueLines(DialogueLines other)
        {
            tableKey = other.tableKey;
            tableName = other.tableName;
            hasOption = other.hasOption;
            mainActor = new ActorInScene(other.mainActor);
            options = other.options;
            reAssignPosition = other.reAssignPosition;
            if(other.leftSidePosition2Actor != null)
            {
                leftSidePosition2Actor = new ActorInScene(other.leftSidePosition2Actor);
            }
            else
            {
                leftSidePosition2Actor = null;
            }
            if (other.rightSidePosition2Actor != null)
            {
                rightSidePosition2Actor = new ActorInScene(other.rightSidePosition2Actor);
            }
            else
            {
                rightSidePosition2Actor = null;
            }

            jumpTo = other.jumpTo;
        }
    }
    [Serializable]
    public class ActorInScene
    {
        public int actorId;
        public ActorExpression actorExpression;
        public ActorInScene()
        {
            actorId = 0;
            actorExpression = ActorExpression.Neutral;
        }
        public ActorInScene(ActorInScene other)
        {
            actorId = other.actorId;
            actorExpression = other.actorExpression;
        }
        public void SetId(int actorId)
        {
            this.actorId = actorId;
        }
        public void SetExpression(ActorExpression actorExpression)
        {
            this.actorExpression = actorExpression;
        }
    }

}
