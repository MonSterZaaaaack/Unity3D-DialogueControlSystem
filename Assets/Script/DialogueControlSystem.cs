using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
namespace DialogueControl
{
    public class DialogueControlSystem : MonoBehaviour
    {
        public List<DialogueOptionController> dialogueOptions;
        public LocalizeStringEvent actorName;
        public LocalizeStringEvent dialogueContent;
        public Image mainActorPortrait;
        public Image otherActorPortrait;
        public DialogueData currentDialogue;
        public ActorDatabase actorDatabase;
        public Dictionary<ActorName, ActorData> actorDictionary;
        public int dialogueIndex;
        public int dialogueCounter;
        public Sprite defaultUnknownSprite;
        private bool hasOption;
        public GameObject dialogueArea;

        public static DialogueControlSystem instance
        {
            get
            {
                return i_instance;
            }
        }
        private static DialogueControlSystem i_instance;
        private void Awake()
        {
            if (i_instance != null && i_instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                i_instance = this;
            }
        }
        public void SetUpDialogue(DialogueData data, int index)
        {
            currentDialogue = data;
            dialogueCounter = 0;
            dialogueIndex = index;
            actorDictionary = new Dictionary<ActorName, ActorData>();
            mainActorPortrait.sprite = null;
            otherActorPortrait.sprite = null;
            hasOption = false;
            foreach (ActorData actorData in actorDatabase.datas)
            {
                actorDictionary[actorData.actorName] = actorData;
            }
            dialogueArea.SetActive(true);
            ContinueDialogue();
        }
        public void OnClick()
        {
            if (!hasOption)
            {
                ContinueDialogue(-1);
            }
        }
        public void DialogueComplete(int index)
        {
            //switch (index)
            //{
            //    case 0:
            //        if (PlayerProcessManager.instance)
            //        {
            //            MainMenuManager.instance.startSelectController.gameObject.SetActive(true);
            //            MainMenuManager.instance.startSelectController.SurvivorSelectSetup(PlayerProcessManager.instance.setupData._startSurvivors, true, true);
            //        }
            //        break;
            //    case 1:
            //        if (PrevGameManager.instance && Time.timeScale == 0) PrevGameManager.instance.TimeScaleResetForced();
            //        break;
            //    case 2:
            //        if (PrevGameManager.instance) PrevGameManager.instance.FinGaTutorialPopUp();
            //        break;
            //    case 3:
            //        if (PrevGameManager.instance)
            //        {
            //            PrevGameManager.instance.TempCameraRetarget();
            //            PrevGameManager.instance._gameStartTrigger += PrevPlayerManager.instance.autoController.GameStart;
            //        }
            //        break;
            //    case 4:
            //        break;
            //}
        }
        public void ContinueDialogue(int nextDialogue = -1)
        {
            try
            {
                if (nextDialogue >= 0)
                {
                    dialogueCounter = nextDialogue;
                }
                // Clean Up Options;
                foreach (var option in dialogueOptions)
                {
                    if (option.gameObject.activeInHierarchy)
                    {
                        option.CleanUp();
                        option.gameObject.SetActive(false);
                    }
                }
                // Setup Current Dialogue Line and Actors 
                if (dialogueCounter >= currentDialogue.dialogueLines.Count)
                {
                    dialogueArea.SetActive(false);
                    DialogueComplete(dialogueIndex);
                    return;
                }
                DialogueLines curLine = currentDialogue.dialogueLines[dialogueCounter];
                ActorData mainActor = actorDictionary.ContainsKey(curLine.actorName) ? actorDictionary[curLine.actorName] : null;
                ActorData otherActor = actorDictionary.ContainsKey(curLine.otherActorInScene) ? actorDictionary[curLine.otherActorInScene] : null;
                // Setup Dialogue line text
                dialogueContent.StringReference.SetReference(curLine.tableName, curLine.tableKey);
                dialogueContent.RefreshString();
                // Setup Main Actor Portrait and Name text
                if (mainActor != null)
                {
                    actorName.StringReference.SetReference(mainActor.tableName, mainActor.tableEntry);
                    actorName.RefreshString();
                    switch (curLine.actorExpression)
                    {
                        case ActorExpression.None:
                            mainActorPortrait.sprite = mainActor.normal;
                            break;
                        case ActorExpression.Happy:
                            mainActorPortrait.sprite = mainActor.happy;
                            break;
                        case ActorExpression.Angry:
                            mainActorPortrait.sprite = mainActor.angry;
                            break;
                        default:
                            mainActorPortrait.sprite = defaultUnknownSprite;
                            break;
                    }
                    if (mainActorPortrait.sprite == null)
                    {
                        mainActorPortrait.sprite = defaultUnknownSprite;
                    }
                }
                else
                {
                    actorName.StringReference.SetReference("ActorName", "Error");
                    actorName.RefreshString();
                    mainActorPortrait.sprite = defaultUnknownSprite;
                }
                // Setup Other Actor Portrait
                if (otherActor != null)
                {
                    otherActorPortrait.sprite = otherActor.normal == null ? defaultUnknownSprite : otherActor.normal;
                }
                else
                {
                    otherActorPortrait.sprite = defaultUnknownSprite;
                }
                //otherActorPortrait.flipX = true;
                // Setup Options
                if (curLine.options.Count > 0)
                {
                    for (int i = 0; i < curLine.options.Count; i++)
                    {
                        if (dialogueOptions[i].gameObject.activeInHierarchy)
                        {
                            dialogueOptions[i].CleanUp();
                        }
                        dialogueOptions[i].gameObject.SetActive(true);
                        dialogueOptions[i].SetUpOption(curLine.options[i]);
                    }
                    hasOption = true;
                }
                else
                {
                    hasOption = false;
                }
                // Setup Counter
                if (curLine.jumpTo >= 0)
                {
                    dialogueCounter = curLine.jumpTo;
                }
                else
                {
                    dialogueCounter++;
                }
                // Setup GreyOut
                if (curLine.isMainCharacterTalking)
                {
                    mainActorPortrait.color = new Color(mainActorPortrait.color.r, mainActorPortrait.color.g, mainActorPortrait.color.b, 1);
                    otherActorPortrait.color = new Color(otherActorPortrait.color.r, otherActorPortrait.color.g, otherActorPortrait.color.b, 0.1f);
                }
                else
                {
                    mainActorPortrait.color = new Color(mainActorPortrait.color.r, mainActorPortrait.color.g, mainActorPortrait.color.b, 0.1f);
                    otherActorPortrait.color = new Color(otherActorPortrait.color.r, otherActorPortrait.color.g, otherActorPortrait.color.b, 1);
                }
            }
            catch (Exception e)
            {
                dialogueContent.StringReference.SetReference("ActorName", "Error");
                actorName.StringReference.SetReference("ActorName", "Error");
                dialogueContent.RefreshString();
                actorName.RefreshString();
                mainActorPortrait.sprite = defaultUnknownSprite;
                otherActorPortrait.sprite = defaultUnknownSprite;
                dialogueCounter++;
                Debug.LogError(e.Message + e.StackTrace);
            }
        }

    }

}
