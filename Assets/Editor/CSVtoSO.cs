using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class CSVtoSO
{
    private static string fileName = "Level 1, Room 1";
    private static string dialogueFilePath = $"/Editor/CSV/{fileName}.tsv";
    private static int counter = 0;

    [MenuItem("Utilities/Generate Dialogue")]

    public static void GenerateDialogue()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + dialogueFilePath, new UTF8Encoding(true));

        Dialogue dialogue = ScriptableObject.CreateInstance<Dialogue>();

        foreach (string line in allLines)
        {
            counter++;
            if (counter == 1)
            {
                continue;
            }

            string[] columns = line.Split('\t');
            Debug.Log(line);

            if (columns.Length > 0)
            {
                dialogue.dialogueType.Add(columns[0].Trim());
            }
 
            // Need recursive for more types of dialogue
            if (columns.Length > 2)
            {
                dialogue.dialogueLines1.Add(columns[2].Trim('"'));
            }

            if (columns.Length > 3)
            {
                dialogue.dialogueLines2.Add(columns[3].Trim('"'));
            }

            if (columns.Length > 4)
            {
                dialogue.dialogueLines3.Add(columns[4].Trim('"'));
            }           
        }

        AssetDatabase.CreateAsset(dialogue, $"Assets/Resources/ScriptableObjects/Dialogues/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }
}

