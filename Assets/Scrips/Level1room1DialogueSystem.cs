using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class Level1room1DialogueSystem : DialogueSystem
{
    // [Refactor] need to move some of these variables to DialogueSystem.cs in the future

    // [Bug] invoke event when the type is Conversation ??????????????????
    // [Bug] change dialoge box design for conversation and remarks
    //predictable dialogue -> consult with mr ken

    private string scriptableObjectFile = "ScriptableObjects/Dialogues/Level 1, Room 1";

    private FormTransform formTransform;
    private PressurePlateScript pressurePlateScript;

    private int layerPlayer;
    private Vector2 redgiePosition;
    [SerializeField] private float dialogueDetectionRadius = 5f;

    protected void OnEnable()
    {
        nextConversation = InputSystem.actions.FindAction("NextConversation");
        if (nextConversation != null)
        {
            nextConversation.performed += nextConversation_performed;
        }
    }

    protected void OnDisable()
    {
        if (nextConversation != null)
        {
            nextConversation.performed -= nextConversation_performed;
            nextConversation = null;
        }
    }

    protected void nextConversation_performed(InputAction.CallbackContext context)
    {
        if (dialogueCanvas.activeSelf && isDialogueReady)
        {
            if (dialogueType[dialogueState] == "Conversation")
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }

                ShowNextLine(ref usableDialogue, ref typingCoroutine, ref dialogueCounter, ref dialogueText, ref dialogueState,
                    ref dialogueCanvas, delayBetweenWords, ToTypeLetters, ToScaleDialogueBox);
            }
        }
    }

    private void Start()
	{
        layerPlayer = LayerMask.GetMask("Player");
        formTransform = GameObject.FindWithTag("Player").GetComponentInChildren<FormTransform>();
        playerMovement = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerMovement>();

        dialogueCanvas = GameObject.FindWithTag("DialogueCanvas");
        dialogueCanvasRT = dialogueCanvas.GetComponentInChildren<Canvas>().gameObject.GetComponent<RectTransform>(); ;
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

        if (dialogueCanvas.activeSelf && isDialogueReady)
        {
            if (dialogueType[dialogueState] == "Remark" && nextRemarkDialogue)
            {
                nextRemarkDialogue = false;
                ShowNextLine(ref usableDialogue, ref typingCoroutine, ref dialogueCounter, ref dialogueText, ref dialogueState,
                    ref dialogueCanvas, delayBetweenWords, ToTypeLetters, ToScaleDialogueBox);
            }
        }
    }

    private void DialogueTriggers()
    {
        if (isDialogueBoxScalingTrigger)
        {
            ToScaleDialogueBox(startScale, endScale, "popin");
        }

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
        isDialogueBoxScalingTrigger = true;
        ToTypeLetters(usableDialogue[dialogueState][0]);

        if (dialogueType[dialogueState] == "Conversation")
        {
            playerMovement.DisablePlayerMovement();
        }
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
            PlayDialogueSound(dialogueText.text.Length, frequencyValue);
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

    private void ToScaleDialogueBox(Vector3 startScale, Vector3 endScale, string condition)
    {
        isDialogueBoxScalingTrigger = false;
        if(isDialogueReady && condition == "popin")  return;
        StartCoroutine(ScaleDialogueBox(dialogueCanvasRT, startScale, endScale, dialoguePopDuration, condition));
    }

    private float EaseInBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }

    private float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return c3 * x * x * x - c1 * x * x;
    }

    // [refactor] a mistake to use coroutine, just normal function and let Update() execute it until the scaling is done
    private IEnumerator ScaleDialogueBox(RectTransform target, Vector3 startScale, Vector3 endScale, float duration, string condition)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            float easedT;

            if (condition == "popin")
            {
                easedT = EaseInBack(t);
            }
            else
            {
                easedT = EaseOutBack(t);
            }

            target.localScale = Vector3.LerpUnclamped(startScale, endScale, easedT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localScale = endScale;
        if (condition == "popin")
        {
            isDialogueReady = true;
        }
        else
        {
            isDialogueReady = false;
            dialogueCanvas.SetActive(false);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(redgiePosition, dialogueDetectionRadius);
    }

    private void OnValidate()
    {
        if (dialogueDetectionRadius <= 0f)
        {
            dialogueDetectionRadius = 1f;
        }
    }
}