using UnityEngine;
using System.Collections.Generic;

public class Level1room1DialogueSystem : DialogueSystem
{

	private string scriptableObjectFile = "ScriptableObjects/Dialogues/Level1Room1Real";
	private string sizeKeyword = "(enlarge font)";
	[SerializeField] private bool isDebug = true;

    private void Start()
	{

		gameData = Resources.Load<Dialogue>(scriptableObjectFile);

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

    //MagUnet meets Redgie
    //MagUnet jumps into paint puddle
    //When the exit door is opened and the level is completed
}