using DialogueControl;
using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    public DialogueData testingData;
    public void OnClick()
    {
        if(DialogueControlSystem.instance != null)
        {
            DialogueControlSystem.instance.SetUpDialogue(testingData, 0);
        }
    }
}
