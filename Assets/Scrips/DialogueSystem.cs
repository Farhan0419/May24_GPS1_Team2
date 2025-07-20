using UnityEngine;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
	protected int dialogueState = 0;
    protected Dialogue gameData;
    protected Dictionary<int, string[]> usableDialogue = new Dictionary<int, string[]>();
    protected Dictionary<int, int[]> indexKeywordsUsableDialogue = new Dictionary<int, int[]>();
}