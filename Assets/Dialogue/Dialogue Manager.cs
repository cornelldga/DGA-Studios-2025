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
    [SerializeField] private Animator dialogueAnim;
    [SerializeField] Image dialogueBox;
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
    private int dialogueProgression;

    [Header("Input Controls")]
    [SerializeField] InputActionReference continueDialogueAction;

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
        gameObject.SetActive(false);
        dialogueProgression = 0;
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
        if (context.action.WasPressedThisFrame() && ongoingDialogue)
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
    /// <param name="file">The json file associated to the specific character.</param>
    /// <param name="dialogueBoxSprite">The dialogue box sprite</param>
    /// <param name="emotionDictionary">The dictionary of sprites associated to the character's emotions.</param>
    /// <param name="scene">Scene name for transitions (boss fight or interactable)</param>
    /// <param name="type">Type of dialogue (NPC, Boss, or Interactive)</param>
    public void StartDialogue(TextAsset file, Sprite dialogueBoxSprite,
        Dictionary<DialogueEmotion, Sprite> emotionDictionary, string scene, DialogueType type)
    {
        Debug.Log(GameManager.Instance.player.progression);
        if (file != null)
        {
            gameObject.SetActive(true);
            nameText.text = file.name;
            ongoingDialogue = true;
            dialogueAnim.SetBool("isOpen", true);
            currentDialogueData = JsonUtility.FromJson<DialogueData>(file.text);
            currentDialogueType = type;
            SetDialogueProgression(nameText.text);
            // Format followed by DialogueEditor.BuildLine()
            currentDialogueID = dialogueProgression + "_" + "start";
            sceneName = scene;
            dialogueBox.sprite = dialogueBoxSprite;
            // Does emotion sprites IF a boss dialogue
            if (currentDialogueType == DialogueType.Interactive)
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
        dialogueAnim.SetBool("isOpen", false);
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
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);

        if (currentDialogueType == DialogueType.Boss)
        {
            yesButton.GetComponentInChildren<TextMeshProUGUI>().text = "Fight!";
            noButton.GetComponentInChildren<TextMeshProUGUI>().text = "Back";
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
         GameManager.Instance.LoadScene(sceneName);
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

    /// <summary>
    /// Sets the dialogue progression number based on name text.
    /// </summary>
    private void SetDialogueProgression(string name)
    {
        if (nameText.text=="Drover")
        {
            dialogueText.color = new Color(0.15f, 0.1f, 0.05f, 1.0f);
            if (GameManager.Instance.player.progression < 4)
            {
                dialogueProgression = 0;
            } else
            {
                dialogueProgression = 1;
                currentDialogueType = 0;
            }
        } else if (nameText.text=="Julius")
        {
            dialogueText.color = new Color(0.15f, 0.1f, 0.05f, 1.0f);
            if (GameManager.Instance.player.progression < 5)
            {
                dialogueProgression = 0;
            } else
            {
                dialogueProgression = 1;
                currentDialogueType = 0;
            }
        } else if (nameText.text=="Ace & Mirage")
        {
            dialogueText.color = Color.white;
            Debug.Log(GameManager.Instance.player.progression);
            if (GameManager.Instance.player.progression < 6)
            {
                dialogueProgression = 0;
            } else
            {
                dialogueProgression = 1;
                currentDialogueType = 0;
            }
        } else if (nameText.text=="Ash")
        {
            dialogueText.color = new Color(0.15f, 0.1f, 0.05f, 1.0f);
            if (GameManager.Instance.player.progression < 7)
            {
                dialogueProgression = 0;
            } else
            {
                dialogueProgression = 1;
                currentDialogueType = 0;
            }
        } else if (nameText.text=="Granny")
        {
            dialogueText.color = new Color(0.15f, 0.1f, 0.05f, 1.0f);
            if (GameManager.Instance.player.progression < 4)
            {
                dialogueProgression = 0;
                currentDialogueType = 0;
            } else if (GameManager.Instance.player.progression < 5)
            {
                dialogueProgression = 1;
                currentDialogueType = 0;
            }  else if (GameManager.Instance.player.progression < 6)
            {
                dialogueProgression = 2;
                currentDialogueType = 0;
            } else if (GameManager.Instance.player.progression < 7)
            {
                dialogueProgression = 3;
                currentDialogueType = 0;
            } else
            {
                dialogueProgression = 4;
            }
        }
    }
}