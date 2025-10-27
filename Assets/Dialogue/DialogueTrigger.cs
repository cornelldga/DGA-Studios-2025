using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] TextAsset jsonTextFile;
    [SerializeField] Sprite dialogueBoxSprite;
    [SerializeField] Sprite neutralSprite;
    [SerializeField] Sprite happySprite;
    [SerializeField] Sprite sadSprite;
    [SerializeField] string sceneName = "";
    private int progressionInt;

    Dictionary<DialogueEmotion, Sprite> emotionDictionary = new Dictionary<DialogueEmotion, Sprite>();

    /// <summary>
    /// Set the emotion dictionary
    /// </summary>
    private void Start()
    {
        emotionDictionary[DialogueEmotion.Neutral] = neutralSprite;
        emotionDictionary[DialogueEmotion.Happy] = happySprite;
        emotionDictionary[DialogueEmotion.Sad] = sadSprite;
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
            dialogueBoxSprite, emotionDictionary, sceneName);
        }
            
    }
}