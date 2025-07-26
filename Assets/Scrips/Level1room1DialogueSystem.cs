using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Level1room1DialogueSystem : DialogueSystem
{
    // [Refactor] need to move some of these variables to DialogueSystem.cs in the future
    // [Bug] press c too quickly 
    // [Bug] set sorting order for dialogue canvas that player is always on top of dialogue canvas, the other objects are behind the dialogue canvas
    // [Bug] invoke event when the type is Conversation ??????????????????
    // [Bug] type is remark then no need to press c to continue, just show the text for a few seconds.
    // [Bug] if currently dialogue remark is being displayed, what if conversation is triggered?
    // [Bug] align left dialogue text, not center

    private string scriptableObjectFile = "ScriptableObjects/Dialogues/Level1Room1Real";
	private string sizeKeyword = "(enlarge font)";
	[SerializeField] private bool isDebug = true;

    private int layerPlayer;
    private InputAction nextConversation;

    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private float normalTextSize = 5.5f;
    [SerializeField] private float enlargeTextSize = 8f;

    [SerializeField] private GameObject dialogueCanvas;

    [SerializeField] private float dialogueDetectionRadius = 5f;

    // this is unused, implement in the future
    [SerializeField] private float widthDialogueBox;
    [SerializeField] private float heightDialogueBox;
    [SerializeField] private float xDialogueBoxOffset;
    [SerializeField] private float yDialogueBoxOffset;

    private FormTransform formTransform;
    private PressurePlateScript pressurePlateScript;

    private Vector2 redgiePosition;

    private int dialogueCounter = 0;

    private bool toTriggerDialogue = true;

    [SerializeField] private float delayBetweenWords = 0.05f;

    private Coroutine typingCoroutine;

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
            ShowNextLine(ref usableDialogue, ref typingCoroutine, ref dialogueCounter, ref dialogueText, ref dialogueState, 
                ref dialogueCanvas, ref toTriggerDialogue, delayBetweenWords, ToTypeLetters);
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

        DialogueTools.LoadDialogueAsset(ref usableDialogue, scriptableObjectFile, isDebug);
    }

    private void Update()
    {
        Vector2 currentRedgiePosition = GameObject.FindWithTag("Redgie").transform.position;

        if(currentRedgiePosition != redgiePosition)
        {
            redgiePosition = currentRedgiePosition;
        }

        if(toTriggerDialogue)
        {
            switch (dialogueState)
            {
                case 0:
                    FirstDialogue();
                    break;

                case 1:
                    SecondDialogue();
                    break;

                case 2:
                    ThirdDialogue();
                    break;
            }
        }
    }

    private void FirstDialogue()
    {
        Collider2D hit = Physics2D.OverlapCircle(redgiePosition, dialogueDetectionRadius, layerPlayer);

        if (hit != null && hit.CompareTag("Player"))
        {
            initializeDialogueValues();
        }

    }

    private void SecondDialogue()
    {
        if(formTransform.IsPaint)
        {
            initializeDialogueValues();
        }
    }

    private void ThirdDialogue()
    {
        if (pressurePlateScript.IsPressed)
        {
            initializeDialogueValues();
        }
    }

    private void initializeDialogueValues()
    {
        //dialogueText.text = usableDialogue[dialogueState][0];
        //DialogueTools.setTextCustomization(dialogueText, indexKeywordsUsableDialogue, dialogueCounter);
        dialogueCounter++;
        toTriggerDialogue = false;
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(redgiePosition, dialogueDetectionRadius);
    }
}