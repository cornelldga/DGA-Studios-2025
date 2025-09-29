using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueLine dialogue;

    public void TriggerDialogue(string currentDialogueID)
    {
        DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
        dialogueManager.StartDialogue(currentDialogueID);
    }
}
