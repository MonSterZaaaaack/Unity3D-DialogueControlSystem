
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
namespace DialogueControl
{
    namespace Options
    {
        public class DialogueOptionController : MonoBehaviour
        {
            public LocalizedString optionTextString;
            public TMP_Text optionText;
            public int jumpTo;
            public void SetUpOption(DialogueOptions options)
            {
                if(optionTextString != null)
                {
                    optionTextString.StringChanged -= SetUpOptionText;
                }
                optionTextString = new LocalizedString(options.tableName, options.tableKey);
                optionTextString.StringChanged += SetUpOptionText;
                jumpTo = options.skipTo;
                
            }
            public void CleanUp()
            {
                if (optionTextString != null)
                {
                    optionTextString.StringChanged -= SetUpOptionText;
                }
                optionTextString = new LocalizedString("ActorName", "Error");
                optionTextString.StringChanged += SetUpOptionText;
                jumpTo = -1;
            }
            private void SetUpOptionText(string s)
            {
                optionText.text = s;
            }
            public void OnClick()
            {
                DialogueControlSystem.instance.ContinueDialogue(jumpTo);
            }
        }
    }
}

