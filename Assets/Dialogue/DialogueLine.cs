using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script creates class DialogueLine with fields to store information collected
/// from a JSON file, and the class DialogueData is a dictionary where the key is a
/// string representing the dialogueID and the value if of type.
/// </summary>
[System.Serializable]
public class DialogueLine
{
    public string dialogueID;
    public string dialogueText;
    public DialogueEmotion emotion;
    public string nextDialogueID;
}

[System.Serializable]
public class DialogueData
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}