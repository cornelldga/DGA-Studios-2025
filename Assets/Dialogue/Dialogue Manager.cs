using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// The types of dialogue
/// </summary>
public enum DialogueType
{
    NPC,
    Boss,
    Interactive
}

/// <summary>
/// This script iterates through a dialogue sequence from a JSON file
/// using DialogueLine and DialogueData.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [SerializeField] private Animator dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [Tooltip("Where the actual NPC/Boss name is displayed")]
    [SerializeField] private TextMeshProUGUI nameText;
    [Tooltip("The grey out background for when dialogue plays")]
    [SerializeField] private Image backgroundImg;
    [Tooltip("Where the bosses sprites will show")]
    [SerializeField] private Image npcImg;

    [Header("Choice Buttons")]
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [Header("Settings")]
    [SerializeField] private float typingSpeed;
    private DialogueData currentDialogueData;
    private string currentDialogueID;
    private Dictionary<DialogueEmotion, Sprite> currentEmotions;
    private bool ongoingDialogue = false;
    private bool isTyping;
    private DialogueType currentDialogueType;
    private string sceneName;

    /// <summary>
    /// Initializes the singleton, hides choices at start up.
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Returns if there is ongoing dialouge
    /// </summary>
    public bool OngoingDialogue()
    {
        return ongoingDialogue;
    }

    /// <summary>
    /// Finishes the dialogue line if not finished yet, goes to next line otherwise.
    /// </summary>
    public void OnContinueDialogue()
    {
        if (ongoingDialogue)
        {
            if (isTyping)
            {
                CompleteCurrentLine();
            }
            else
            {
                DisplayNextLine();
            }
        }
    }

    /// <summary>
    /// Dialogue begins by finding the start dialogue associated with the  specified progress.
    /// Calls DisplayNextLine() to begin dialogue sequence.
    /// Changes the npcImg sprite to the correct characters sprite if a boss
    /// If an interactable or boss will change scene to what is next
    /// </summary>
    /// <param name="progress">The progression number of the npc's dialogue</param>
    /// <param name="file">The json file associated to the specific character.</param>
    /// <param name="dialogueBoxSprite">The dialogue box sprite</param>
    /// <param name="emotionDictionary">The dictionary of sprites associated to the character's emotions.</param>
    /// <param name="scene">Scene name for transitions (boss fight or interactable)</param>
    /// <param name="type">Type of dialogue (NPC, Boss, or Interactive)</param>
    public void StartDialogue(TextAsset file, int progress, Sprite dialogueBoxSprite,
        Dictionary<DialogueEmotion, Sprite> emotionDictionary, string scene, DialogueType type)
    {
        if (file != null)
        {
            nameText.text = file.name;
            ongoingDialogue = true;
            dialogueBox.SetBool("isOpen", true);
            currentDialogueData = JsonUtility.FromJson<DialogueData>(file.text);
            // Format followed by DialogueEditor.BuildLine()
            currentDialogueID = progress.ToString() + "_" + "start";
            currentDialogueType = type;
            sceneName = scene;
            dialogueBox.GetComponent<Image>().sprite = dialogueBoxSprite;
            // Does emotion sprites IF a boss dialogue
            if (type == DialogueType.Interactive)
            {
                npcImg.gameObject.SetActive(false);
            }
            else
            {
                this.currentEmotions = emotionDictionary;
                npcImg.gameObject.SetActive(true);
            }
            GameManager.Instance.FreezePlayer(true);
            DisplayNextLine();
        }
    }

    /// <summary>
    /// Searches for a DialogueLine with ID matching currentDialogueID, begins
    /// Coroutine to display dialogue. If nextDialogueID is empty, ends dialogue;
    /// else, sets currentDialogueID to nextDialogueID 
    /// </summary>
    public void DisplayNextLine()
    {
        if (currentDialogueID == "")
        {
            if (currentDialogueType == DialogueType.Boss || currentDialogueType == DialogueType.Interactive)
            {
                DialogueChoice();
            }
            else
            {
                Debug.Log("1");
                EndDialogue();
            }
            return;
        }
        if (currentDialogueData.dialogueLines.Count > 0)
        {
            foreach (DialogueLine line in currentDialogueData.dialogueLines)
            {
                if (line.dialogueID == currentDialogueID)
                {
                    if (currentDialogueType == DialogueType.Boss || currentDialogueType == DialogueType.NPC)
                    {
                        npcImg.sprite = currentEmotions[(DialogueEmotion)line.emotion];
                    }
                    StopAllCoroutines();
                    StartCoroutine(TypeSentence(line));
                    return;
                }
            }
            throw new System.Exception("No dialogue lines found");
        }
    }

    /// <summary>
    /// Does the animation for typing text.
    /// </summary>
    /// <param name="line">The line of text from the JSON which is being displayed.</param>
    IEnumerator TypeSentence(DialogueLine line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in line.dialogueText.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        currentDialogueID = line.nextDialogueID;
    }

    /// <summary>
    /// Completes the typing animation instantly
    /// </summary>
    private void CompleteCurrentLine()
    {
        StopAllCoroutines();
        isTyping = false;

        foreach (DialogueLine line in currentDialogueData.dialogueLines)
        {
            if (line.dialogueID == currentDialogueID)
            {
                dialogueText.text = line.dialogueText;
                currentDialogueID = line.nextDialogueID;
                break;
            }
        }
    }

    /// <summary>
    /// Dialogue ends, and UI is removed from the screen.
    /// </summary>
    public void EndDialogue()
    {
        ongoingDialogue = false;
        GameManager.Instance.FreezePlayer(false);
        dialogueBox.SetBool("isOpen", false);
    }

    /// <summary>
    /// Sets text of the given file to currentDialogueData through DialogueData class.
    /// </summary>
    /// <param name="file">The JSON text file to parse from.</param>
    public void SetJSON(TextAsset file)
    {
        if (file != null)
        {
            currentDialogueData = JsonUtility.FromJson<DialogueData>
                ("{\"dialogueLines\":" + file.text + "}");
        }
    }

    /// <summary>
    /// Sets UI buttons active and updates button text based on dialogue type.
    /// </summary>
    private void DialogueChoice()
    {
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);

        if (currentDialogueType == DialogueType.Boss)
        {
            yesButton.GetComponentInChildren<TextMeshProUGUI>().text = "Let's Fight!";
            noButton.GetComponentInChildren<TextMeshProUGUI>().text = "Not Yet";
        }
        else if (currentDialogueType == DialogueType.Interactive)
        {
            yesButton.GetComponentInChildren<TextMeshProUGUI>().text = "Yes";
            noButton.GetComponentInChildren<TextMeshProUGUI>().text = "No";
        }
    }

    /// <summary>
    /// Handles the "Yes" choice for boss fights or interactive objects like doors.
    /// </summary>
    public void YesChoice()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        if (nameText.text == "Loadout"){
            GameManager.Instance.ToggleLoadoutManager(true);
            EndDialogue();
        }
        else
        {
            GameManager.Instance.LoadScene(sceneName);
        }
    }

    /// <summary>
    /// Handles the "No" choice, ending dialogue without taking action. Retriggers nearby interaction zones
    /// </summary>
    public void NoChoice()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(GameManager.Instance.player.transform.position, 1.5f))
        {
            collider.gameObject.GetComponent<InteractionZone>()?.SetCanInteract(true);
        }
        EndDialogue();
    }
}