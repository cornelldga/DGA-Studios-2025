using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] TextAsset jsonTextFile;
    [SerializeField] Sprite dialogueBoxSprite;
    [SerializeField] DialogueType dialogueType = DialogueType.NPC;

    [Header("Boss-only Fields")]
    [SerializeField] Sprite neutralSprite;
    [SerializeField] Sprite happySprite;
    [SerializeField] Sprite sadSprite;
    Dictionary<DialogueEmotion, Sprite> emotionDictionary = new Dictionary<DialogueEmotion, Sprite>();

    [Header("Boss and interactable Fields")]
    [SerializeField] string sceneName = "";
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