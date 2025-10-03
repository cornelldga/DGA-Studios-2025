using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueLine dialogue;
    [SerializeField] TextAsset jsonTextFile;

    public void TriggerDialogue(string currentDialogueID)
    {
        DialogueManager.Instance.StartDialogue(jsonTextFile, currentDialogueID);


        DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
        //dialogueManager.StartDialogue(currentDialogueID);
    }
    
    
}
