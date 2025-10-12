using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] TextAsset jsonTextFile;
    [SerializeField] public Sprite[] emotionSprites;
    private int progressionInt;

    public void Interact()
    {
        TriggerDialogue();
    }

    /// <summary>
    /// Triggers start dialogue in dialogue manager with the correct dialogue ID given by the button.
    /// </summary>
    public void TriggerDialogue()
    {
        Debug.Log("Trigger Dialogue");
        DialogueManager.Instance.StartDialogue(jsonTextFile, progressionInt.ToString(), emotionSprites);
    }
}