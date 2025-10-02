/// <summary>
/// This script creates class DialogueLine with fields to store information collected
/// from a JSON file, and the class DialogueData to store a list of these DialogueLines.
/// </summary>
public class DialogueLine
{
    public string dialogueID;
    public string dialogueText;
    public string emotion;
    public string nextDialogueID;
}

public class DialogueData
{
    public DialogueLine[] dialogueLines;
}
