using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue")]
public class Dialogue : ScriptableObject
{
    public List<string> dialogueType = new List<string>();
    public List<string> dialogueLines1 = new List<string>();
    public List<string> dialogueLines2 = new List<string>();
    public List<string> dialogueLines3 = new List<string>();
}
