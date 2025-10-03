using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] TextAsset jsonTextFile;

    public void TriggerDialogue(string currentDialogueID)
    {
        DialogueManager.Instance.StartDialogue(jsonTextFile, currentDialogueID);
    }
    
    
}
