using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;


public enum DialogueCriteria
{
    First,
    AcceptedQuest,
    CompletedQuest,
    CompletedWithoutAccept,
    FinalDialogue
}

[Serializable]
public class DialogueHolder
{
    public DialogueContainer dialogueObject;
    public DialogueCriteria criteria;
}

public class DialogueReader : MonoBehaviour
{
    [SerializeField] private Canvas dialoguePopup;

    [SerializeField] private List<DialogueHolder> multiDialogues;
    [SerializeField] private string NPCName;
    private DialogueCriteria currentCriteria;
    private DialogueContainer dialogueContainer;
    private List<NodeLinkData> currentOutputNodes;
    
    private bool boxIsUp = false;
    private Canvas currentDialogue;

    private DialogueSystem dialogueSystem;
    private List<TextMeshProUGUI> texts;
    
    private string currentNodeGuid;

    private string currentNpcLine;

    private Transform playerTrans;

    private int clickCount;
    public delegate void TradeStartDelegate(); 
    public static event TradeStartDelegate OnStartTrade;
    
    public delegate void StartEndDialogueDelegate(); 
    public static event StartEndDialogueDelegate OnStartEndDialogue;
    
    
    

    private void Start()
    {
        currentCriteria = DialogueCriteria.First;
        playerTrans = GameObject.FindWithTag("Player").transform;
     
    }


    


    private void StartTrade()
    {
        if (OnStartTrade != null)
        {
            OnStartTrade();
        }
    }

    private void ChangeDialogueContainer()
    {
        dialogueContainer = multiDialogues.Single(x => x.criteria == currentCriteria).dialogueObject;
    }
    
    private string GetFirstLineOfDialogue()
    {
        ChangeDialogueContainer();
        
        currentNodeGuid = dialogueContainer.NodeLinks[0].TargetNodeGUID;

        return dialogueContainer.DialogueNodeData.Single(x => x.NodeGUID == currentNodeGuid).DialogueText;
    }

    private void OnMouseDown()
    {
        if (!boxIsUp && Vector3.Distance(transform.position, playerTrans.position) < 5)
        {
            boxIsUp = true;
            currentDialogue = Instantiate(dialoguePopup);
            texts = currentDialogue.GetComponentsInChildren<TextMeshProUGUI>().ToList();

            texts[0].text = GetFirstLineOfDialogue();
            GetOutputNodesFromNode();
            SetupReplyButtons();
            
            texts[1].text = NPCName;
        }

        if (OnStartEndDialogue != null && Vector3.Distance(transform.position, playerTrans.position) < 5)
        {
            OnStartEndDialogue();
        }
    }

    private void SetupReplyButtons()
    {
        clickCount = 0;
        var buttons = currentDialogue.GetComponentsInChildren<Button>().ToList();
       
        foreach (var button in buttons)
        {
            Button butt = button;
            butt.onClick.RemoveAllListeners();
            butt.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        for (int i = 0; i < currentOutputNodes.Count; i++)
        {
            int test = i;
            string targetGuid = currentOutputNodes[test].TargetNodeGUID;
            string chosenLine = currentOutputNodes[test].PortName;
            buttons[i].onClick.AddListener(() => ClickContinue(targetGuid, chosenLine));
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentOutputNodes[i].PortName;
        }

    }

    private string GetDialogueFromNode(string guid)
    {
        return dialogueContainer.DialogueNodeData.Single(x => x.NodeGUID == guid).DialogueText;
    }
    
    private void GetOutputNodesFromNode()
    { 
       currentOutputNodes = dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == currentNodeGuid).ToList();
    }

    private void ShutDownDialogue()
    {
        Destroy(currentDialogue.gameObject);
        boxIsUp = false;

        if (OnStartEndDialogue != null)
        {
            OnStartEndDialogue();
        }
    }

    private void PauseDialogue()
    {
        currentDialogue.gameObject.SetActive(false);
       
        StartTrade();
    }

    private void ResumeDialogue()
    {
        currentDialogue.gameObject.SetActive(true);

        currentNodeGuid = currentOutputNodes[0].TargetNodeGUID;
        GetOutputNodesFromNode();
        SetupReplyButtons();
        texts[0].text = GetDialogueFromNode(currentNodeGuid);

    }
    
    private void ClickContinue(string nextNodeGuid, string selectedLine)
    {
        if (clickCount < 6)
        {
            clickCount++;

            string nextLine = GetDialogueFromNode(nextNodeGuid);

            if (selectedLine == "Trade")
            {
                currentNodeGuid = nextNodeGuid;
                PauseDialogue();
            }
            
            else
            {
                texts[0].text = nextLine;
                currentNodeGuid = nextNodeGuid;

                GetOutputNodesFromNode();
                SetupReplyButtons();

             
                if (selectedLine == "Leave")
                {
                    if (currentCriteria is DialogueCriteria.CompletedQuest or DialogueCriteria.CompletedWithoutAccept)
                    {
                        currentCriteria = DialogueCriteria.FinalDialogue;
                        GetComponent<Collider>().enabled = false;
                    }
                    ShutDownDialogue();
                }
            }
        }
    }
}
