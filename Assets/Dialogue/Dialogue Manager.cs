using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

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
    private System.Action onComplete;
    [SerializeField] private Animator dialogueAnim;
    [SerializeField] Image dialogueBox;
    [SerializeField] TextMeshProUGUI dialogueText;
    [Tooltip("Where the actual NPC/Boss name is displayed")]
    [SerializeField] TextMeshProUGUI nameText;
    private Vector2 defaultNamePos;
    private Quaternion defaultNameRot;
    [Tooltip("The gray out background for when dialogue plays")]
    [SerializeField] Image grayBackground;
    [Tooltip("Where the bosses sprites will show")]
    [SerializeField] private Image npcImg;
    private string currentFileName;
    private bool isCutscene;
    public Dictionary<DialogueEmotion, Sprite> dukeEmotions;

    [Header("Choice Buttons")]
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [Header("Settings")]
    [SerializeField] float typingSpeed;
    private DialogueData currentDialogueData;
    private string currentDialogueID;
    private Dictionary<DialogueEmotion, Sprite> currentEmotions;
    private bool ongoingDialogue = false;
    private bool isTyping;
    private DialogueType currentDialogueType;
    private string sceneName;

    [Header("Dialogue Choices")]
    [SerializeField] GameObject choices;
    [SerializeField] TMP_Text yesButtonText;
    [SerializeField] TMP_Text noButtonText;
    [SerializeField] string bossYesText;
    [SerializeField] string bossNoText;


    [Header("Input Controls")]
    [SerializeField] InputActionReference continueDialogueAction;

    private void OnEnable()
    {
        continueDialogueAction.action.Enable();
        continueDialogueAction.action.performed += ContinueDialogue;
    }

    private void OnDisable()
    {
        continueDialogueAction.action.Disable();
        continueDialogueAction.action.performed -= ContinueDialogue;
    }

    private void Start()
    {
        choices.SetActive(false);
        gameObject.SetActive(false);
        RectTransform rect = nameText.GetComponent<RectTransform>();
        defaultNamePos = rect.anchoredPosition;
        defaultNameRot = rect.localRotation;
    }

    /// <summary>
    /// Listens to advance dialogue via space or enter key. This is incase input actions fails.
    /// </summary>
    void Update()
    {
        if (!ongoingDialogue) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (isTyping) CompleteCurrentLine();
            else DisplayNextLine();
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
    void ContinueDialogue(InputAction.CallbackContext context)
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
            continueDialogueAction.action.Enable();
            gameObject.SetActive(true);
            isCutscene = file.name.StartsWith("cutscene");
            currentFileName = isCutscene ? "" : file.name;
            nameText.text = currentFileName;   

            if (file.name == "cutscene_1")
            {
                RectTransform rect = nameText.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(-470f, -107f);
                rect.localRotation = Quaternion.Euler(0f, 0f, 8.225f);
            }

            if (nameText.text=="Mirage & Ace")
            {
                dialogueText.color = Color.white;
            } else
            {
                dialogueText.color = new Color(0.15f, 0.1f, 0.05f, 1.0f);
            }
            ongoingDialogue = true;
            dialogueAnim.SetBool("isOpen", true);
            currentDialogueData = JsonUtility.FromJson<DialogueData>(file.text);
            // Format followed by DialogueEditor.BuildLine()
            currentDialogueID = progress.ToString() + "_" + "start";
            currentDialogueType = type;
            sceneName = scene;
            dialogueBox.sprite = dialogueBoxSprite;
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
        } else
        {
            // Automatic player loadout
            if (scene=="Loadout")
            {
                EndDialogue();
                GameManager.Instance.ToggleLoadoutManager(true);
            } else
            {
                // Automatic entry/exit for the saloon   
                GameManager.Instance.LoadScene(scene); 
            }
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
                    string speakerName = string.IsNullOrEmpty(line.speaker) ? currentFileName : line.speaker;

                    nameText.text = speakerName;

                    bool hasEmotion = currentEmotions != null && currentEmotions.ContainsKey((DialogueEmotion)line.emotion);
                    bool isDuke = (speakerName == "Duke");

                    if (isCutscene)
                    {
                        // Hide if not Duke during cutscenes
                        npcImg.gameObject.SetActive(isDuke);
                    }
                    else
                    {
                        npcImg.gameObject.SetActive(hasEmotion);
                    }

                    if (npcImg.gameObject.activeSelf)
                    {
                        npcImg.sprite = currentEmotions.ContainsKey((DialogueEmotion)line.emotion) ? currentEmotions[(DialogueEmotion)line.emotion] : currentEmotions[DialogueEmotion.Neutral];
                    }

                    StopAllCoroutines();
                    StartCoroutine(TypeSentence(line));
                    return; 
                }
            }
            
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
        dialogueAnim.SetBool("isOpen", false);
        RectTransform rect = nameText.GetComponent<RectTransform>();
        rect.anchoredPosition = defaultNamePos;
        rect.localRotation = defaultNameRot;
    }

    /// <summary>
    /// Animation trigger that closes the dialogue completely
    /// </summary>
    public void AnimationCloseDialogue()
    {
        gameObject.SetActive(false);
        GameManager.Instance.FreezePlayer(false);
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
        choices.SetActive(true);

        if (currentDialogueType == DialogueType.Boss)
        {
            yesButtonText.text = bossYesText;
            noButtonText.text = bossNoText;
        }
        else if (currentDialogueType == DialogueType.Interactive)
        {
            yesButtonText.text = "Yes";
            noButtonText.text = "No";
        }
    }

    /// <summary>
    /// Handles the "Yes" choice for boss fights or interactive objects like doors.
    /// </summary>
    public void YesChoice()
    {
        ongoingDialogue = false;
        choices.SetActive(false);
        gameObject.SetActive(false);
        GameManager.Instance.LoadScene(sceneName);
    }

    /// <summary>
    /// Handles the "No" choice, ending dialogue without taking action. Retriggers nearby interaction zones
    /// </summary>
    public void NoChoice()
    {
        ongoingDialogue = false;
        choices.SetActive(false);
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(GameManager.Instance.player.transform.position, 1.5f))
        {
            collider.gameObject.GetComponent<InteractionZone>()?.SetCanInteract(true);
        }
        EndDialogue();
    }
}