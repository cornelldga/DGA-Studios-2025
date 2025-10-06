using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] TextAsset jsonTextFile;
    private int progressionInt;
    private bool inRange = false;
    private GameObject trigger;
    private InputAction interact;
    private PlayerInputActions playerControls;

    /// <summary>
    /// Enables the player input map to turn on.
    /// </summary>
    private void OnEnable()
    {
        playerControls = new PlayerInputActions();
        interact = playerControls.Player.Interact;
        interact.Enable();
    }
    /// <summary>
    /// Disables the player input map to turn on.
    /// </summary>
    private void OnDisable()
    {
        interact.Disable();
    }

    /// <summary>
    /// Checks if player pressed interact key
    /// </summary>
    private void Update()
    {
        if (interact.WasPressedThisFrame())
        {
            if (!DialogueManager.Instance.dialogueOngoing && inRange)
            {
                TriggerDialogue();
            }
        }
    }

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
            inRange = true;
        }
        else if (collision.CompareTag("Player") && DialogueManager.Instance.dialogueOngoing)
        {
            trigger.SetActive(false);
            inRange = false;
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
