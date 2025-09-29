[System.Serializable]
public class DialogueLine
{
    public string dialogueID;
    public string dialogueText;
    public string nextDialogueID;
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public string choiceText;
    public string nextDialogueChoiceID;
}

[System.Serializable]
public class DialogueData
{
    public DialogueLine[] dialogueLines;
}
