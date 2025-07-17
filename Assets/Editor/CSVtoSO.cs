using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class CSVtoSO
{
    private static string dialogueFilePath = "/Editor/CSV/Test/Level1Room_test1.txt";
    private static int counter = 0;

    [MenuItem("Utilities/Generate Dialogue")]

    public static void GenerateDialogue()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + dialogueFilePath, new UTF8Encoding(true));

        Dialogue dialogue = ScriptableObject.CreateInstance<Dialogue>();

        foreach (string line in allLines)
        {
          
            if(counter == 0)
            {
                Debug.Log(line);
                continue;
            }

            string[] columns = line.Split('\t');

            if (columns.Length > 0)
            {
                dialogue.dialogueType.Add(columns[0].Trim());
            }

            // maybe need to use recursion
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

            counter++;
        }

        AssetDatabase.CreateAsset(dialogue, "Assets/ScriptableObjects/Dialogues/Dialogue.asset");
        AssetDatabase.SaveAssets();
    }
}

//create another function for dialogue system to called to split the dialogue lines into different sections from split(";")
// once breaking the lines into sections, then check each line got "(enlarge font)" this text, then remove it from the line
// Then the index for the line will be store, so that it knows which line to enlarge the font
