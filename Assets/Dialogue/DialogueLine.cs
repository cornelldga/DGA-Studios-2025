[System.Serializable]
public class DialogueLine
{
    public string dialogueID;
    public string characterName;
    public string dialogueText;
    public string nextDialogueID;
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public string choiceText;
    public string nextDialogueChoiceId;
}

[System.Serializable]
public class DialogueData
{
    public DialogueLine[] dialogueLines;
}
