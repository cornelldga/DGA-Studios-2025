using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] public int bossProgression;
    [SerializeField] public GameObject quest;
    private Dictionary<DialogueEmotion, Sprite> emotionDictionary = new Dictionary<DialogueEmotion, Sprite>();

    [Header("Boss and interactable Fields")]
    [SerializeField] private string sceneName = "";


    /// <summary>
    /// Set the emotion dictionary
    /// </summary>
    private void Start()
    {
        if (dialogueType == DialogueType.Boss || dialogueType == DialogueType.NPC)
        {
            emotionDictionary[DialogueEmotion.Neutral] = neutralSprite;
            emotionDictionary[DialogueEmotion.Happy] = happySprite;
            emotionDictionary[DialogueEmotion.Sad] = sadSprite;
        }
        if (dialogueType == DialogueType.Boss)
        {
            if (bossProgression == PlayerPrefs.GetInt("progression",0))
            {
                quest.SetActive(true);
            } else
            {
                quest.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Triggers start dialogue in dialogue manager with the correct dialogue ID given by the button.
    /// </summary>
    public void TriggerDialogue()
    {
        if (!GameManager.Instance.GetDialogueManager.OngoingDialogue())
        {
            GameManager.Instance.GetDialogueManager.StartDialogue(jsonTextFile, dialogueBoxSprite, emotionDictionary, sceneName, dialogueType);
        }

    }

    /// <summary>
    /// Will trigger dialogue when interacted.
    /// </summary>
    public void Interact()
    {
        TriggerDialogue();
    }
}