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
    protected TextMeshProUGUI dialogueText;

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

    protected void ShowNextLine(ref Dictionary<int, string[]> usableDialogue, ref Coroutine typingCoroutine, ref int dialogueCounter, ref TextMeshProUGUI dialogueText, ref int dialogueState, ref GameObject dialogueCanvas, float delayBetweenWords, Action<string> callback)
    {
        int dialogueLength = usableDialogue[dialogueState].Length;
        typingCoroutine = null;

        if (dialogueCounter < dialogueLength)
        {
            //DialogueTools.setTextCustomization(dialogueText, indexKeywordsUsableDialogue, dialogueCounter);
            string currentLine = usableDialogue[dialogueState][dialogueCounter];
            dialogueCounter++;
            callback?.Invoke(currentLine); 
        }
        else
        {
            dialogueCanvas.SetActive(false);
            dialogueCounter = 0;
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