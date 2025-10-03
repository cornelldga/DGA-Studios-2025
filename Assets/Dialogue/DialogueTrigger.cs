using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] TextAsset jsonTextFile;

    private GameObject trigger;

    /// <summary>
    /// Triggers start dialogue in dialogue manager with the correct dialogue ID given by the button.
    /// </summary>
    /// <param name="currentDialogueID">The dialogueID to show.</param>
    public void TriggerDialogue(string currentDialogueID)
    {
        DialogueManager.Instance.StartDialogue(jsonTextFile, currentDialogueID);
    }

    /// <summary>
    /// Shows dialogue button when player in radius and hides when player in dialogue
    /// </summary>
    /// <param name="collision">The player</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !DialogueManager.Instance.dialogueOngoing)
        {
            trigger = DialogueManager.Instance.transform.GetChild(0).gameObject;
            trigger.SetActive(true);
        }
        else if (collision.CompareTag("Player") && DialogueManager.Instance.dialogueOngoing)
        {
            trigger.SetActive(false);
        }
    }

    /// <summary>
    /// Hides dialogue button when player leaves
    /// </summary>
    /// <param name="collision">The player</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && trigger != null)
        {
            trigger.SetActive(false);
        }
    }
}
