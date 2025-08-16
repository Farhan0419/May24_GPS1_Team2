using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class DialogueSystem : MonoBehaviour
{
	protected int dialogueState = 0;
    protected Dictionary<int, string[]> usableDialogue = new Dictionary<int, string[]>();
    //protected Dictionary<int, int[]> indexKeywordsUsableDialogue = new Dictionary<int, int[]>();
    protected List<string> dialogueType = new List<string>();

    protected InputAction nextConversation;

    [SerializeField] protected GameObject dialogueCanvas;
    protected Canvas dialogueCanvasComponent;
    protected RectTransform dialogueCanvasRT;
    protected GameObject conversationDialogueBox;
    protected GameObject remarkDialogueBox;
    protected TextMeshProUGUI conversationDialogueText;
    protected TextMeshProUGUI remarkDialogueText;
    protected TextMeshProUGUI dialogueText;
    protected PlayerMovement playerMovement;

    protected private Vector2 redgiePosition;

    // this is unused, implement in the future
    //protected float widthDialogueBox;
    //protected float heightDialogueBox;
    //protected float xDialogueBoxOffset;
    //protected float yDialogueBoxOffset;

    protected int dialogueCounter = 0;
    [SerializeField] protected float delayBetweenWords = 0.05f;
    [SerializeField] protected float delayBetweenRemarks = 3f;

    protected Coroutine typingCoroutine;

    protected bool nextRemarkDialogue = false;

    protected HashSet<int> executedStates = new HashSet<int>();

    //private string sizeKeyword = "(enlarge font)";
    //[SerializeField] private float normalTextSize = 5.5f;
    //[SerializeField] private float enlargeTextSize = 8f;

    [Header("Dialogue Box")]
    protected Vector3 startScale = Vector3.zero;
    [SerializeField] protected Vector3 endScale = new Vector3(1.2f, 1.2f, 1.5f);
    [SerializeField] protected float dialoguePopDuration = 0.4f;
    protected bool isDialogueReady = false;
    protected bool isDialogueBoxScalingTrigger = false;

    protected bool isLineFullyShown = false;
    protected int actualLineLength = 0;

    [Header("Audio")]
    [SerializeField] protected AudioClip dialogueTypingSoundClip;
    [SerializeField] protected bool stopAudioSource = true;
    [Range(1, 5)]
    [SerializeField] protected int frequencyValue = 1;
    [Range(-3, 3)]
    [SerializeField] protected float minPitch = 0.5f;
    [Range(-3, 3)]
    [SerializeField] protected float maxPitch = 3f;
    protected AudioSource audioSource;

    [SerializeField] protected bool isDebug = true;

    public bool IsDialogueReady
    {
        get => isDialogueReady;
    }

    public string DialogueType
    {
        get => dialogueType[dialogueState];
    }

    protected void Awake()
    {
        audioSource = this.gameObject.AddComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the DialogueSystem GameObject.");
        }
    }

    protected virtual void Start()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerMovement>();
        redgiePosition = GameObject.FindWithTag("Redgie").transform.position;

        if(dialogueCanvas != null)
        {
            dialogueCanvasComponent = dialogueCanvas.GetComponentInChildren<Canvas>();
            dialogueCanvasRT = dialogueCanvasComponent.GetComponent<RectTransform>(); ;
            conversationDialogueBox = GameObject.FindWithTag("ConvoSpeechBubble");
            remarkDialogueBox = GameObject.FindWithTag("RemarkSpeechBubble");
            conversationDialogueText = conversationDialogueBox.GetComponentInChildren<TextMeshProUGUI>();
            remarkDialogueText = remarkDialogueBox.GetComponentInChildren<TextMeshProUGUI>();
            

            dialogueCanvas.SetActive(false);
        }

    }

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
        if (nextConversation != null)
        {
            TriggerToShowLine();
        }
    }

    protected void ShowNextLine(ref Dictionary<int, string[]> usableDialogue, ref Coroutine typingCoroutine, ref int dialogueCounter, ref TextMeshProUGUI dialogueText, ref int dialogueState, ref GameObject dialogueCanvas, float delayBetweenWords, Action<string> typeLetters, Action<Vector3, Vector3, string> scaleDialogueBox)
    {
        int dialogueLength = usableDialogue[dialogueState].Length;
        typingCoroutine = null;

        if (dialogueCounter < dialogueLength)
        {
            string currentLine = usableDialogue[dialogueState][dialogueCounter];

            if (dialogueType[dialogueState] == "Conversation")
            {
                if(!isLineFullyShown)
                {
                    dialogueText.maxVisibleCharacters = currentLine.Length;
                    isLineFullyShown = true;
                    dialogueCounter++;
                }
                else
                {
                    isLineFullyShown = false;
                    typeLetters?.Invoke(currentLine);
                }
            }
            else
            {
                dialogueCounter++;
                isLineFullyShown = false;
                typeLetters?.Invoke(currentLine);
            }
        }
        else
        {
            dialogueCounter = 0;
            isLineFullyShown = false;
            isDialogueReady = false;

            if (dialogueCanvas.activeSelf)
            {
                scaleDialogueBox?.Invoke(endScale, startScale, "");
            }

            if (dialogueType[dialogueState] == "Conversation")
            {
                playerMovement.EnablePlayerMovement();
            }
        }
    }

    protected void TriggerToShowLine(bool isConvo = true)
    {
        if (dialogueCanvas != null)
        {
            if (dialogueCanvas.activeSelf && isDialogueReady)
            {
                if (dialogueType[dialogueState] == "Conversation" && isConvo)
                {
                    if (typingCoroutine != null)
                    {
                        StopCoroutine(typingCoroutine);
                    }

                    ShowNextLine(ref usableDialogue, ref typingCoroutine, ref dialogueCounter, ref dialogueText, ref dialogueState,
                        ref dialogueCanvas, delayBetweenWords, ToTypeLetters, ToScaleDialogueBox);
                }
                else if (dialogueType[dialogueState] == "Remark" && nextRemarkDialogue)
                {

                    nextRemarkDialogue = false;
                    ShowNextLine(ref usableDialogue, ref typingCoroutine, ref dialogueCounter, ref dialogueText, ref dialogueState,
                        ref dialogueCanvas, delayBetweenWords, ToTypeLetters, ToScaleDialogueBox);
                }
            }  
        }
    }

    protected void changeDialogueBoxes()
    {
        if (dialogueType[dialogueState] == "Conversation")
        {
            conversationDialogueBox.SetActive(true);
            remarkDialogueBox.SetActive(false);
        }
        else if (dialogueType[dialogueState] == "Remark")
        {
            conversationDialogueBox.SetActive(false);
            remarkDialogueBox.SetActive(true);
        }
    }

    protected void setValuesBasedOnDialogueType()
    {
        if (dialogueType[dialogueState] == "Conversation")
        {
            dialogueCanvasComponent.sortingOrder = 3;
            playerMovement.DisablePlayerMovement();
        }
        else if (dialogueType[dialogueState] == "Remark")
        {
            dialogueCanvasComponent.sortingOrder = 1;
        }
    }

    protected void PlayDialogueSound(int currentDisplayedCharacterCount, int frequencyValue)
    {
        if (currentDisplayedCharacterCount % frequencyValue == 0)
        {
            if (stopAudioSource)
            {
                audioSource.Stop();
            }
            audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(dialogueTypingSoundClip);
        }
    }

    protected void initializeDialogueValues()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        executedStates.Add(dialogueState);
        dialogueCounter = 0;
        dialogueCanvas.SetActive(true);
        isDialogueBoxScalingTrigger = true;
        setValuesBasedOnDialogueType();

        if(dialogueType[dialogueState] == "Conversation")
        {
            dialogueText = conversationDialogueText;
        }
        else if (dialogueType[dialogueState] == "Remark")
        {
            dialogueText = remarkDialogueText;
        }

        ToTypeLetters(usableDialogue[dialogueState][dialogueCounter]);
    }

    protected void ToTypeLetters(string msg)
    {
        typingCoroutine = StartCoroutine(TypeLetters(msg, dialogueText, delayBetweenWords));
    }

    private IEnumerator TypeLetters(string sentence, TextMeshProUGUI dialogueText, float delayBetweenWords)
    {
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.text = sentence;
        dialogueText.ForceMeshUpdate();

        bool isRichTextTag = false;
        actualLineLength = 0;

        for (int i = 0; i < sentence.Length; i++)
        {
            if (sentence[i] == '<' || isRichTextTag)
            {
                isRichTextTag = true;
                if (sentence[i] == '>')
                {
                    isRichTextTag = false;
                }
                continue;
            }

            actualLineLength++;
        }

        for (int i = 0; i < actualLineLength; i++)
        {
            PlayDialogueSound(i, frequencyValue);
            dialogueText.maxVisibleCharacters = i+1;

            yield return new WaitForSeconds(delayBetweenWords);
        }

        if (dialogueType[dialogueState] == "Remark")
        {
            yield return new WaitForSeconds(delayBetweenRemarks);
            nextRemarkDialogue = true;
        }

        isLineFullyShown = true;
        dialogueCounter++;
        typingCoroutine = null;

    }

    protected void ToScaleDialogueBox(Vector3 startScale, Vector3 endScale, string condition)
    {
        isDialogueBoxScalingTrigger = false;
        if (isDialogueReady && condition == "popin") return;
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

    protected void TrackAndAssignRedgiePosition()
    {
        Vector2 currentRedgiePosition = GameObject.FindWithTag("Redgie").transform.position;

        if (currentRedgiePosition != redgiePosition)
        {
            redgiePosition = currentRedgiePosition;
        }
    }
}