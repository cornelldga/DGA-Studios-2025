using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]

/// <summary>
/// Triggers dialogue sequences when interacted with for all dialogue types.
/// </summary> 
public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [Header("General Settings")]
    [SerializeField] private TextAsset jsonTextFile;
    [SerializeField] private Sprite dialogueBoxSprite;
    [SerializeField] private DialogueType dialogueType = DialogueType.NPC;

    [Header("Boss-only Fields")]
    [SerializeField] private Sprite neutralSprite;
    [SerializeField] private Sprite happySprite;
    [SerializeField] private Sprite sadSprite;
    private Dictionary<DialogueEmotion, Sprite> emotionDictionary = new Dictionary<DialogueEmotion, Sprite>();

    [Header("Boss and interactable Fields")]
    [SerializeField] private string sceneName = "";
    private int progressionInt;


    /// <summary>
    /// Set the emotion dictionary
    /// </summary>
    private void Start()
    {
        if (dialogueType == DialogueType.Boss)
        {
            emotionDictionary[DialogueEmotion.Neutral] = neutralSprite;
            emotionDictionary[DialogueEmotion.Happy] = happySprite;
            emotionDictionary[DialogueEmotion.Sad] = sadSprite;
        }
    }

    /// <summary>
    /// Will trigger dialogue when interacted.
    /// </summary>
    public void Interact()
    {
        TriggerDialogue();
    }

    /// <summary>
    /// Triggers start dialogue in dialogue manager with the correct dialogue ID given by the button.
    /// </summary>
    public void TriggerDialogue()
    {
        if (!DialogueManager.Instance.OngoingDialogue())
        {
            DialogueManager.Instance.StartDialogue(jsonTextFile, progressionInt,
            dialogueBoxSprite, emotionDictionary, sceneName, dialogueType);
        }

    }
}