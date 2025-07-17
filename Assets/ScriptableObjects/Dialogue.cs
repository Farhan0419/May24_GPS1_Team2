using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue")]
public class Dialogue : ScriptableObject
{
    // change to library instead
    public List<string> dialogueType = new List<string>();
    public List<string> dialogueLines1 = new List<string>();
    public List<string> dialogueLines2 = new List<string>();
    public List<string> dialogueLines3 = new List<string>();
}

//Dictionary<string, Dictionary<int, List<string>>> data = new Dictionary<string, Dictionary<int, List<string>>>
//{
//    { "Conversation", new Dictionary<int, List<string>>
//        {
//            { 1, new List<string> { "test", "test" } },
//            { 2, new List<string> { "test", "test" } }
//        }
//    },
//    { "Remark", new Dictionary<int, List<string>>
//        {
//            { 1, new List<string> { "test", "test" } },
//            { 2, new List<string> { "test", "test" } }
//        }
//    }
//};

//using System.Collections.Generic;

//[System.Serializable]
//public class RootData
//{
//    public List<Section> Conversation;
//    public List<Section> Remark;
//}

//[System.Serializable]
//public class Section
//{
//    public Dictionary<int, List<string>> Data;
//}
