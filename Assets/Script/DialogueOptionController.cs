
using UnityEngine;
using UnityEngine.Localization.Components;
namespace DialogueControl
{
    public class DialogueOptionController : MonoBehaviour
    {
        public LocalizeStringEvent optionText;
        public int jumpTo;
        public void SetUpOption(DialogueOptions options)
        {
            optionText.StringReference.SetReference(options.tableName, options.tableKey);
            jumpTo = options.skipTo;
            optionText.RefreshString();
        }
        public void CleanUp()
        {
            optionText.StringReference.SetReference("ActorName", "Error");
            optionText.RefreshString();
            jumpTo = -1;
        }
        public void OnClick()
        {
            DialogueControlSystem.instance.ContinueDialogue(jumpTo);
        }
    }
}

