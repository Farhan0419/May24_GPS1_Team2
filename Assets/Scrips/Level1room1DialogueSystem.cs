using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Level1room1DialogueSystem : DialogueSystem
{
    // [Refactor] need to move some of these variables to DialogueSystem.cs in the future

    // [Bug] invoke event when the type is Conversation ??????????????????
    // [Bug] press c again to show full line, then press c again to show next line (type out)

    private string scriptableObjectFile = "ScriptableObjects/Dialogues/Level1Room1Real";

    //private string sizeKeyword = "(enlarge font)";
    //[SerializeField] private float normalTextSize = 5.5f;
    //[SerializeField] private float enlargeTextSize = 8f;

    private FormTransform formTransform;
    private PressurePlateScript pressurePlateScript;

    private int layerPlayer;
    private Vector2 redgiePosition;
    [SerializeField] private float dialogueDetectionRadius = 5f;

    private void OnEnable()
    {
        nextConversation = InputSystem.actions.FindAction("NextConversation");
        if (nextConversation != null)
        {
            nextConversation.performed += nextConversation_performed;
        }
    }

    private void OnDisable()
    {
        if (nextConversation != null)
        {
            nextConversation.performed -= nextConversation_performed;
            nextConversation = null;
        }
    }

    private void nextConversation_performed(InputAction.CallbackContext context)
    {
        if (dialogueCanvas.activeSelf)
        {
            if (dialogueType[dialogueState] == "Conversation")
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }

                ShowNextLine(ref usableDialogue, ref typingCoroutine, ref dialogueCounter, ref dialogueText, ref dialogueState,
                    ref dialogueCanvas, delayBetweenWords, ToTypeLetters);
            }
        }
    }

    private void Start()
	{
        layerPlayer = LayerMask.GetMask("Player");
        formTransform = GameObject.FindWithTag("Player").GetComponentInChildren<FormTransform>();

        dialogueCanvas = GameObject.FindWithTag("DialogueCanvas");
        dialogueText = GameObject.FindWithTag("DialogueCanvas").GetComponentInChildren<TextMeshProUGUI>();

        pressurePlateScript = GameObject.FindWithTag("PressurePlate").GetComponentInChildren<PressurePlateScript>();

        redgiePosition = GameObject.FindWithTag("Redgie").transform.position;

        dialogueCanvas.SetActive(false);

        DialogueTools.LoadDialogueAsset(ref usableDialogue, ref dialogueType, scriptableObjectFile, isDebug);
    }

    private void Update()
    {
        Vector2 currentRedgiePosition = GameObject.FindWithTag("Redgie").transform.position;

        if(currentRedgiePosition != redgiePosition)
        {
            redgiePosition = currentRedgiePosition;
        }

        DialogueTriggers();

        if (dialogueCanvas.activeSelf)
        {
            if (dialogueType[dialogueState] == "Remark" && nextRemarkDialogue)
            {
                nextRemarkDialogue = false;
                ShowNextLine(ref usableDialogue, ref typingCoroutine, ref dialogueCounter, ref dialogueText, ref dialogueState,
                    ref dialogueCanvas, delayBetweenWords, ToTypeLetters);
            }
        }
    }

    private void DialogueTriggers()
    {
        FirstDialogue();
        SecondDialogue();
        ThirdDialogue();
    }

    private void FirstDialogue()
    {
        if (executedStates.Contains(0)) return;

        Collider2D hit = Physics2D.OverlapCircle(redgiePosition, dialogueDetectionRadius, layerPlayer);

        if (hit != null && hit.CompareTag("Player"))
        {
            dialogueState = 0;
            initializeDialogueValues();
        }
    }

    private void SecondDialogue()
    {
        if (executedStates.Contains(1)) return;

        if (formTransform.IsPaint)
        {
            dialogueState = 1;
            initializeDialogueValues();
        }
    }

    private void ThirdDialogue()
    {
        if (executedStates.Contains(2)) return;

        if (pressurePlateScript.IsPressed)
        {
            dialogueState = 2;
            initializeDialogueValues();
        }
    }

    private void initializeDialogueValues()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        executedStates.Add(dialogueState);
        dialogueCounter = 0;
        dialogueCounter++;
        dialogueCanvas.SetActive(true);
        ToTypeLetters(usableDialogue[dialogueState][0]);    
    }

    private void ToTypeLetters(string msg)
    {
        typingCoroutine = StartCoroutine(TypeLetters(msg, dialogueText, delayBetweenWords));
    }

    private IEnumerator TypeLetters(string sentence, TextMeshProUGUI dialogueText, float delayBetweenWords)
    {
        dialogueText.text = "";

        for (int i = 0; i < sentence.Length; i++)
        {
            dialogueText.text += sentence[i];
            yield return new WaitForSeconds(delayBetweenWords);
        }

        if (dialogueType[dialogueState] == "Remark")
        {
            yield return new WaitForSeconds(delayBetweenRemarks);
            nextRemarkDialogue = true;
        }

        typingCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(redgiePosition, dialogueDetectionRadius);
    }
}