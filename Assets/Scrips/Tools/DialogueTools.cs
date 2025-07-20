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
}



