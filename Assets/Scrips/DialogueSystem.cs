using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

public class DialogueSystem : MonoBehaviour
{
	protected int dialogueState = 0;
    protected Dialogue gameData;
    protected Dictionary<int, string[]> usableDialogue = new Dictionary<int, string[]>();
    protected Dictionary<int, int[]> indexKeywordsUsableDialogue = new Dictionary<int, int[]>();

    public void ShowNextLine(ref Dictionary<int, string[]> usableDialogue, ref Coroutine typingCoroutine, ref int dialogueCounter, ref TextMeshProUGUI dialogueText, ref int dialogueState, ref GameObject dialogueCanvas, ref bool toTriggerDialogue, float delayBetweenWords, Action<string> callback)
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
            dialogueState++;
            dialogueCanvas.SetActive(false);
            dialogueCounter = 0;
            toTriggerDialogue = true;
        }
    }
}