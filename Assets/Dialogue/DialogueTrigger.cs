using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] TextAsset jsonTextFile;
    private int progressionInt;
    private GameObject trigger;


    /// <summary>
    /// Triggers start dialogue in dialogue manager with the correct dialogue ID given by the button.
    /// </summary>
    /// <param name="currentDialogueID">The dialogueID to show.</param>
    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(jsonTextFile, progressionInt.ToString());
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
