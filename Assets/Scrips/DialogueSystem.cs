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
    protected Dialogue gameData;
    protected Dictionary<int, string[]> usableDialogue = new Dictionary<int, string[]>();
    //protected Dictionary<int, int[]> indexKeywordsUsableDialogue = new Dictionary<int, int[]>();
    protected List<string> dialogueType = new List<string>();

    protected InputAction nextConversation;

    protected GameObject dialogueCanvas;
    protected RectTransform dialogueCanvasRT;
    protected TextMeshProUGUI dialogueText;
    protected PlayerMovement playerMovement;

    // this is unused, implement in the future
    protected float widthDialogueBox;
    protected float heightDialogueBox;
    protected float xDialogueBoxOffset;
    protected float yDialogueBoxOffset;

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

    protected void Awake()
    {
        audioSource = this.gameObject.AddComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the DialogueSystem GameObject.");
        }
    }

    protected void ShowNextLine(ref Dictionary<int, string[]> usableDialogue, ref Coroutine typingCoroutine, ref int dialogueCounter, ref TextMeshProUGUI dialogueText, ref int dialogueState, ref GameObject dialogueCanvas, float delayBetweenWords, Action<string> typeLetters, Action<Vector3, Vector3, string> scaleDialogueBox)
    {
        int dialogueLength = usableDialogue[dialogueState].Length;
        typingCoroutine = null;

        if (dialogueCounter < dialogueLength)
        {
            if (dialogueType[dialogueState] == "Conversation")
            {
                //Debug.Log(dialogueCounter);
                if(dialogueText.text != usableDialogue[dialogueState][(dialogueCounter-1)])
                {
                    //Debug.Log(dialogueText.text);
                    //Debug.Log(usableDialogue[dialogueState][(dialogueCounter-1)]);
                    // [Bug] cannot show full line when skip on last dialogueLine because of dialogueCounter < dialogueLength
                    dialogueText.text = usableDialogue[dialogueState][(dialogueCounter-1)];
                    return;
                }  
            }

            //DialogueTools.setTextCustomization(dialogueText, indexKeywordsUsableDialogue, dialogueCounter);
            string currentLine = usableDialogue[dialogueState][dialogueCounter];
            dialogueCounter++;
            Debug.Log(dialogueCounter);
            typeLetters?.Invoke(currentLine);
        }
        else
        {
            if (dialogueCanvas.activeSelf)
            {
                scaleDialogueBox?.Invoke(endScale, startScale, "");
            }

            dialogueCounter = 0;

            if (dialogueType[dialogueState] == "Conversation")
            {
                playerMovement.EnablePlayerMovement();
            }
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
}