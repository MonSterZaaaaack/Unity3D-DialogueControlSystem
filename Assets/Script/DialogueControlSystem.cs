using DialogueControl.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
namespace DialogueControl
{
    public class DialogueControlSystem : MonoBehaviour
    {
        private LocalizedString _actorNameString;
        private LocalizedString _dialogueContentString;
        [Header("UI - Portrait Images")]
        public Image LeftSideMainPortrait;
        public Image LeftSideSubPortrait;
        public Image RightSideMainPortrait;
        public Image RightSideSubPortrait;
        [Header("UI - Dialogue Contents")]
        public Image speakerProfileIcon;
        public TMP_Text speakerName;
        public TMP_Text dialogueContent;
        public List<DialogueOptionController> dialogueOptions;
        [Header("Current Dialogue Data")]
        public DialogueData currentDialogue;
        [Header("Global Control variables")]
        public ActorDatabase actorDatabase;
        public Dictionary<int, ActorData> actorDictionary;
        private Dictionary<int, ActivePortrait> actorPortraitMap = new();
        private Dictionary<PortraitSlot, ActivePortrait> slotMap = new();
        private static readonly PortraitSlot[] LeftSlots = { PortraitSlot.Left1, PortraitSlot.Left2 };
        private static readonly PortraitSlot[] RightSlots = { PortraitSlot.Right1, PortraitSlot.Right2 };
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
            actorDictionary = new Dictionary<int, ActorData>();
            hasOption = false;
            actorPortraitMap = new Dictionary<int, ActivePortrait>();
            slotMap = new Dictionary<PortraitSlot, ActivePortrait>();
            foreach (ActorData actorData in actorDatabase.datas)
            {
                actorDictionary[actorData.actorId] = actorData;
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
                ActorData mainActor = actorDictionary.ContainsKey(curLine.mainActor.actorId) ? actorDictionary[curLine.mainActor.actorId] : null;
                // Setup Dialogue line text
                if (_dialogueContentString != null)
                {
                    _dialogueContentString.StringChanged -= SwitchDialogueContentString;

                }
                _dialogueContentString = new LocalizedString(curLine.tableName, curLine.tableKey);
                _dialogueContentString.StringChanged += SwitchDialogueContentString;
                // Setup Main Actor Name text
                if (_actorNameString != null)
                {
                    _actorNameString.StringChanged -= SwitchSpeakerNameString;
                }
                if (mainActor != null)
                { 
                        _actorNameString = new LocalizedString(mainActor.tableName, mainActor.tableEntry);
                }
                else
                {
                    _actorNameString = new LocalizedString("ActorName", "Error");
                    //mainActorPortrait.sprite = defaultUnknownSprite;
                }
                _actorNameString.StringChanged += SwitchSpeakerNameString;
                // Setup Portrait, flip portrait sprite if it is in the right side.
                if (curLine.reAssignPosition)
                {
                    actorPortraitMap = new Dictionary<int, ActivePortrait>();
                    slotMap = new Dictionary<PortraitSlot, ActivePortrait>();
                    ActivePortrait mainActorPortrait = new ActivePortrait { actorId = curLine.mainActor.actorId, slot = PortraitSlot.Left1, expression = curLine.mainActor.actorExpression};
                    actorPortraitMap[curLine.mainActor.actorId] = mainActorPortrait;
                    slotMap[mainActorPortrait.slot] = mainActorPortrait;
                    ActivePortrait talkingToActorPortrait = new ActivePortrait { actorId = curLine.talkingToActor.actorId, slot = PortraitSlot.Right1, expression = curLine.talkingToActor.actorExpression };
                    actorPortraitMap[curLine.talkingToActor.actorId] = talkingToActorPortrait;
                    slotMap[talkingToActorPortrait.slot] = talkingToActorPortrait;
                    // The slot Map is Initialized when Re AssignPoisition is Detected, so if no Actor assigned for Left 2 and Right 2 position, those two portrait will become empty.
                    if (curLine.leftSidePosition2Actor != null)
                    {
                        
                        if (curLine.leftSidePosition2Actor.actorId != -1)
                        {

                            ActivePortrait leftSidePosition2 = new ActivePortrait { actorId = curLine.leftSidePosition2Actor.actorId, slot = PortraitSlot.Left2, expression = curLine.leftSidePosition2Actor.actorExpression };
                            actorPortraitMap[curLine.leftSidePosition2Actor.actorId] = leftSidePosition2;
                            slotMap[leftSidePosition2.slot] = leftSidePosition2;
                        }

                    }
                    if(curLine.rightSidePosition2Actor != null)
                    {
                        if(curLine.rightSidePosition2Actor.actorId != -1)
                        {
                            ActivePortrait rightSidePosition2 = new ActivePortrait { actorId = curLine.rightSidePosition2Actor.actorId, slot = PortraitSlot.Right2, expression = curLine.rightSidePosition2Actor.actorExpression };
                            actorPortraitMap[curLine.rightSidePosition2Actor.actorId] = rightSidePosition2;
                            slotMap[rightSidePosition2.slot] = rightSidePosition2;
                        }

                    }
                }
                else
                {
                    UpdatePortraits(curLine);
                    
                }
                UpdateActorPortraitSprite();
                // Update Main Actor Icon
                switch (curLine.mainActor.actorExpression)
                {
                    case ActorExpression.Neutral:
                        speakerProfileIcon.sprite = mainActor.neutralProfileIcon;
                        break;
                    case ActorExpression.Happy:
                        speakerProfileIcon.sprite = mainActor.happyProfileIcon;
                        break;
                    case ActorExpression.Angry:
                        speakerProfileIcon.sprite = mainActor.angryProfileIcon;
                        break;
                    case ActorExpression.Sad:
                        speakerProfileIcon.sprite = mainActor.sadProfileIcon;
                        break;

                }

                // Setup Options
                if (curLine.hasOption)
                {
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
                }
                else
                {
                    hasOption = false;
                    // Setup Counter
                    if (curLine.jumpTo >= 0)
                    {
                        dialogueCounter = curLine.jumpTo;
                    }
                    else
                    {
                        dialogueCounter++;
                    }
                }
                //TODO: Setup GreyOut
            }
            catch (Exception e)
            {
                dialogueCounter++;
                Debug.LogError(e.Message + e.StackTrace);
            }
        }

        private void SwitchSpeakerNameString(string s)
        {
            speakerName.text = s;
        }
        private void SwitchDialogueContentString(string s)
        {
            dialogueContent.text = s;
        }
        public void UpdateActorPortraitSprite()
        {
            LeftSideMainPortrait.sprite = null;
            RightSideMainPortrait.sprite = null;
            LeftSideSubPortrait.sprite = null;
            RightSideSubPortrait.sprite = null;
            for(int i = 0; i < 4; i++)
            {
                PortraitSlot curSlot = (PortraitSlot)i;
                if (slotMap.ContainsKey(curSlot))
                {
                    ActivePortrait curPortrait = slotMap[curSlot];
                    ActorData curActor = actorDictionary[curPortrait.actorId];
                    Sprite actorSprite = null;
                    switch (curPortrait.expression)
                    {
                        case ActorExpression.Neutral:
                            actorSprite = curActor.neutralPortrait;
                            break;
                        case ActorExpression.Happy:
                            actorSprite = curActor.happyPortrait;
                            break;
                        case ActorExpression.Angry:
                            actorSprite = curActor.angryPortrait;
                            break;
                        case ActorExpression.Sad:
                            actorSprite = curActor.sadPortrait;
                            break;
                        default:
                            actorSprite = curActor.neutralPortrait;
                            break;
                    }
                    if(actorSprite != null)
                    {
                        switch (curSlot)
                        {
                            case PortraitSlot.Left1:
                                LeftSideMainPortrait.sprite = actorSprite;
                                break;
                            case PortraitSlot.Left2:
                                LeftSideSubPortrait.sprite = actorSprite;
                                break;
                            case PortraitSlot.Right1:
                                RightSideMainPortrait.sprite= actorSprite;
                                break;
                            case PortraitSlot.Right2:
                                RightSideSubPortrait.sprite = actorSprite;
                                break;
                        }
                    }

                }
            }
            LeftSideMainPortrait.gameObject.SetActive(LeftSideMainPortrait.sprite != null);
            LeftSideSubPortrait.gameObject.SetActive(LeftSideSubPortrait.sprite != null);
            RightSideMainPortrait.gameObject.SetActive(RightSideMainPortrait.sprite != null);
            RightSideSubPortrait.gameObject.SetActive(RightSideSubPortrait.sprite != null);
        }
        public void UpdatePortraits(DialogueLines line)
        {
            ActorInScene main = line.mainActor;
            ActorInScene talkingTo = line.talkingToActor;
            int talkingToActorId = line.talkingToActor.actorId;

            bool mainInScene = actorPortraitMap.ContainsKey(main.actorId);
            bool talkingToInScene = talkingToActorId != -1 && actorPortraitMap.ContainsKey(talkingToActorId);

            if (!mainInScene)
            {
                // Determine side for main actor
                if (talkingToInScene)
                {
                    var oppositeSide = GetOppositeSideSlots(actorPortraitMap[talkingToActorId].slot);
                    TryPlaceActor(main.actorId, main.actorExpression, oppositeSide);
                }
                else
                {
                    var leftSlot = TryFindAvailableSlot(LeftSlots);
                    var rightSlot = TryFindAvailableSlot(RightSlots);

                    if (leftSlot != null)
                    {
                        PlaceActorInSlot(main.actorId, main.actorExpression, leftSlot.Value);
                        if (talkingToActorId != -1)
                            TryPlaceActor(talkingToActorId, talkingTo.actorExpression, RightSlots);
                    }
                    else if (rightSlot != null)
                    {
                        PlaceActorInSlot(main.actorId, main.actorExpression, rightSlot.Value);
                        if (talkingToActorId != -1)
                            TryPlaceActor(talkingToActorId, talkingTo.actorExpression, LeftSlots);
                    }
                    else
                    {
                        // Fallback: replace someone from left
                        PlaceActorInSlot(main.actorId, main.actorExpression, LeftSlots[0]);
                        if (talkingToActorId != -1)
                            PlaceActorInSlot(talkingToActorId, talkingTo.actorExpression, RightSlots[0]);
                    }
                }
            }
            else
            {
                // Main already in scene, just update expression
                actorPortraitMap[main.actorId].expression = main.actorExpression;

                if (talkingToActorId != -1)
                {

                    if (talkingToActorId != -1)
                    {
                        if (!talkingToInScene)
                        {
                            var side = GetOppositeSideSlots(actorPortraitMap[main.actorId].slot);
                            TryPlaceActor(talkingToActorId, talkingTo.actorExpression, side);
                        }
                        else
                        {
                            // Both in scene, check if on same side
                            var mainSlot = actorPortraitMap[main.actorId].slot;
                            var talkingToSlot = actorPortraitMap[talkingToActorId].slot;
                            if (IsSameSide(mainSlot, talkingToSlot))
                            {
                                var oppositeSide = GetOppositeSideSlots(mainSlot);
                                TryPlaceActor(talkingToActorId, talkingTo.actorExpression, oppositeSide);
                            }
                        }
                    }
                }
            }
        }

        private void TryPlaceActor(int actorId, ActorExpression expr, PortraitSlot[] preferredSlots)
        {
            var slot = TryFindAvailableSlot(preferredSlots);
            if (slot.HasValue)
                PlaceActorInSlot(actorId, expr, slot.Value);
            else
                PlaceActorInSlot(actorId, expr, preferredSlots[0]); // Replace someone
        }
        private bool IsSameSide(PortraitSlot a, PortraitSlot b)
        {
            return (LeftSlots.Contains(a) && LeftSlots.Contains(b)) || (RightSlots.Contains(a) && RightSlots.Contains(b));
        }
        private PortraitSlot[] GetOppositeSideSlots(PortraitSlot slot)
        {
            return LeftSlots.Contains(slot) ? RightSlots : LeftSlots;
        }

        private PortraitSlot? TryFindAvailableSlot(PortraitSlot[] slots)
        {
            foreach (var s in slots)
            {
                if (!slotMap.ContainsKey(s))
                    return s;
            }
            return null;
        }

        private void PlaceActorInSlot(int actorId, ActorExpression expr, PortraitSlot slot)
        {
            // Remove current occupant if any
            if (slotMap.ContainsKey(slot))
            {
                var replaced = slotMap[slot];
                actorPortraitMap.Remove(replaced.actorId);
            }

            var portrait = new ActivePortrait { actorId = actorId, slot = slot, expression = expr };
            actorPortraitMap[actorId] = portrait;
            slotMap[slot] = portrait;

            // TODO: Visually instantiate or update portrait on screen
        }

    }
    // The available portrait positions in priority order
    public enum PortraitSlot { Left1 = 0, Left2 = 2, Right1 = 1, Right2 = 3 }

    // Structure representing an actor's portrait currently on screen
    public class ActivePortrait
    {
        public int actorId;
        public PortraitSlot slot;
        public ActorExpression expression;
    }
}
