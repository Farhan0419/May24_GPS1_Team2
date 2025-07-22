using System.Collections.Generic;
using UnityEngine;

public class DialogueTools
{
	public static string[] DisplayableDialogue(string dialogues)
	{
		return dialogues.Split(';');
	}

	public static List<int> CheckForKeyWords(ref string[] dialogueLines, string keyword)
	{
		int counter = 0;
		List<int> indexToLinesForChanges = new List<int>();

		for(int i = 0; i < dialogueLines.Length; i++)
		{
			string line = dialogueLines[i];

			if (line.Contains(keyword))
			{
				indexToLinesForChanges.Add(counter);
				line = line.Replace(keyword, "");
			}

			dialogueLines[i] = line;
			counter++;
        }

		return indexToLinesForChanges;
	}

    public static void loadDialogueAsset(ref string[] usableDialogue, ref int[] indexKeywordsUsableDialogue, string scriptableObjectFile, bool isDebug)
    {
        Dialogue gameData = Resources.Load<Dialogue>(scriptableObjectFile);

        if (gameData != null)
        {
            int numberOfDialogues = gameData.dialogueLines1.ToArray().Length;

            for (int i = 0; i < numberOfDialogues; i++)
            {
                string[] currentDialogue = DialogueTools.DisplayableDialogue(gameData.dialogueLines1[i]);

                usableDialogue[i] = currentDialogue;

                int linesInDialogue = currentDialogue.Length;

                indexKeywordsUsableDialogue[i] = DialogueTools.CheckForKeyWords(ref currentDialogue, sizeKeyword).ToArray();
            }

            if (isDebug)
            {
                foreach (KeyValuePair<int, int[]> entry in indexKeywordsUsableDialogue)
                {
                    Debug.Log($"Key: {entry.Key}, Value: {entry.Value}");

                    foreach (int lineIndex in entry.Value)
                    {
                        Debug.Log(lineIndex);
                    }
                }

                foreach (KeyValuePair<int, string[]> entry in usableDialogue)
                {
                    Debug.Log($"Key: {entry.Key}, Value: {entry.Value}");

                    foreach (string line in entry.Value)
                    {
                        Debug.Log(line);
                    }
                }
            }
        }
    }

    public static void setTextCustomization(ref TextMeshProUGUI dialogueText, int[] indexKeywordsUsableDialogue, ref int dialogueCounter)
    {
        int lengthOfIndexKeywords = indexKeywordsUsableDialogue[dialogueState].Length;

        if (lengthOfIndexKeywords > 0)
        {
            foreach (int index in indexKeywordsUsableDialogue[dialogueState])
            {
                if (dialogueCounter == index)
                {
                    Debug.Log(dialogueCounter);
                    Debug.Log("bold");
                    dialogueText.fontSize = enlargeTextSize;
                    dialogueText.fontStyle = FontStyles.Bold;
                }
                else
                {
                    dialogueText.fontSize = normalTextSize;
                    dialogueText.fontStyle = FontStyles.Normal;
                }
            }
        }
        else
        {
            dialogueText.fontSize = normalTextSize;
            dialogueText.fontStyle = FontStyles.Normal;
            Debug.Log("null");
        }
    }

    private void ShowNextLine(ref string[] usableDialogue, ref Coroutine typingCoroutine, ref int dialogueCounter, ref TextMeshProUGUI dialogueText, int[] indexKeywordsUsableDialogue, ref int dialogueState, ref GameObject dialogueCanvas, ref bool toTriggerDialogue, float delayBetweenWords)
    {
        int dialogueLength = usableDialogue[dialogueState].Length;
        typingCoroutine = null;

        if (dialogueCounter < dialogueLength)
        {
            DialogueTools.setTextCustomization(dialogueText, indexKeywordsUsableDialogue, dialogueCounter);
            string currentLine = usableDialogue[dialogueState][dialogueCounter];
            typingCoroutine = StartCoroutine(TypeLetters(currentLine, dialogueText, delayBetweenWords));

            //dialogueText.text = usableDialogue[dialogueState][dialogueCounter];
            dialogueCounter++;
        }
        else
        {
            dialogueState++;
            dialogueCanvas.SetActive(false);
            dialogueCounter = 0;
            toTriggerDialogue = true;
        }
    }

    private IEnumerator TypeLetters(string sentence, ref TextMeshProUGUI dialogueText, float delayBetweenWords)
    {
        dialogueText.text = "";

        for (int i = 0; i < sentence.Length; i++)
        {
            dialogueText.text += sentence[i];
            yield return new WaitForSeconds(delayBetweenWords);
        }
    }
}



