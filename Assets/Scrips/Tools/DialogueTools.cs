using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueTools
{
	public static string[] DisplayableDialogue(string dialogues)
	{
		return dialogues.Split(';');
	}

    public static void LoadDialogueAsset(ref Dictionary<int, string[]> usableDialogue, ref List<string> dialogueTypes, string scriptableObjectFile, bool isDebug)
    {
        Dialogue gameData = Resources.Load<Dialogue>(scriptableObjectFile);

        if (gameData != null)
        {
            int numberOfDialogues = gameData.dialogueLines1.ToArray().Length;

            for (int i = 0; i < numberOfDialogues; i++)
            {
                string[] currentDialogue = DisplayableDialogue(gameData.dialogueLines1[i]);

                usableDialogue[i] = currentDialogue;

                dialogueTypes.Add(gameData.dialogueType[i]);
            }

            if (isDebug)
            {
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

    //public static List<int> CheckForKeyWords(ref string[] dialogueLines, string keyword)
    //{
    //	int counter = 0;
    //	List<int> indexToLinesForChanges = new List<int>();

    //	for(int i = 0; i < dialogueLines.Length; i++)
    //	{
    //		string line = dialogueLines[i];

    //		if (line.Contains(keyword))
    //		{
    //			indexToLinesForChanges.Add(counter);
    //			line = line.Replace(keyword, "");
    //		}

    //		dialogueLines[i] = line;
    //		counter++;
    //       }

    //	return indexToLinesForChanges;
    //}

    //public static void SetTextCustomization(ref TextMeshProUGUI dialogueText, int[] indexKeywordsUsableDialogue, ref int dialogueCounter)
    //{
    //    int lengthOfIndexKeywords = indexKeywordsUsableDialogue[dialogueState].Length;

    //    if (lengthOfIndexKeywords > 0)
    //    {
    //        foreach (int index in indexKeywordsUsableDialogue[dialogueState])
    //        {
    //            if (dialogueCounter == index)
    //            {
    //                Debug.Log(dialogueCounter);
    //                Debug.Log("bold");
    //                dialogueText.fontSize = enlargeTextSize;
    //                dialogueText.fontStyle = FontStyles.Bold;
    //            }
    //            else
    //            {
    //                dialogueText.fontSize = normalTextSize;
    //                dialogueText.fontStyle = FontStyles.Normal;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        dialogueText.fontSize = normalTextSize;
    //        dialogueText.fontStyle = FontStyles.Normal;
    //        Debug.Log("null");
    //    }
    //}
}



