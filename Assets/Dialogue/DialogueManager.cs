using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; 

/// <summary>
/// This script iterates through a dialogue sequence from a JSON file
/// using DialogueLine and DialogueData.
/// </summary>

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private InputAction submit;
    private PlayerInputActions playerControls;
    private DialogueData currentDialogueData;
    private string currentDialogueID;
    public bool dialogueOngoing;
    private bool isTyping; 
    public Animator animator;
    private bool fadeIn;
    private bool fadeOut;
    private bool isFading;
    private string currentCharacterName;
    [SerializeField] private CharacterEmotions[] characters;
    [SerializeField] public GameObject popup;
    [SerializeField] private Image backgroundImg;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image npcImg;
    [SerializeField] private float typingSpeed = 0.05f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Enables the UI input map to turn on.
    /// </summary>
    private void OnEnable()
    {
        playerControls = new PlayerInputActions();
        submit = playerControls.UI.Submit;
        submit.Enable();
    }

    /// <summary>
    /// Disables the UI input map to turn on.
    /// </summary>
    private void OnDisable()
    {
        submit.Disable();
    }

    /// <summary>
    /// Checks if they are clicking through dialogue and if the fade for blurring background needs to occur.
    /// </summary>
    private void Update()
    {
        if (submit.WasPressedThisFrame())
        {
            if (dialogueOngoing)
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

        if (isFading)
        {
            Fade();
        }
    }
    
    /// <summary>
    /// Dialogue begins, calls DisplayNextLine() to begin dialogue sequence.
    /// Changes the npcImg sprite to the correct characters sprite
    /// </summary>
    /// <param name="dialogueID">The dialogueID of the first dialogue to show.</param>
    public void StartDialogue(TextAsset file, string dialogueID)
    {
        isFading = true;
        fadeIn = true;
        animator.SetBool("isOpen", true);
        if (file != null)
        {
            currentDialogueData = JsonUtility.FromJson<DialogueData>
                ("{\"dialogueLines\":" + file.text + "}");
            currentDialogueID = "progress" + dialogueID + "_dialogueA";
            if (file.name.Length > 7)
            {
                nameText.fontSize = 18;
            }

            currentCharacterName = file.name;
            nameText.text = file.name;
            
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
        dialogueOngoing = true;
        if (currentDialogueID == "")
        {
            EndDialogue();
            return;
        }
        
        if (currentDialogueData.dialogueLines.Length > 0)
        {
            foreach (DialogueLine line in currentDialogueData.dialogueLines)
            {
                if (line.dialogueID == currentDialogueID)
                {
                    npcImg.sprite = GetEmotionSprite(currentCharacterName, line.emotion);
                    StopAllCoroutines();
                    StartCoroutine(TypeSentence(line));
                    break; 
                }
            }
        }
    }

    /// <summary>
    /// Does the animation for typing text.
    /// </summary>
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
        fadeOut = true;
        Fade();
        dialogueOngoing = false;
        animator.SetBool("isOpen", false);
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
    /// Helper function to get the character sprite with wanted emotion
    /// </summary>
    private Sprite GetEmotionSprite(string characterName, string emotion)
    {
        foreach (CharacterEmotions character in characters)
        {
            if (character.characterName == characterName)
            {
                string targetName = characterName + "_" + emotion;
                foreach (Sprite sprite in character.emotionSprites)
                {
                    if (sprite != null && sprite.name == targetName)
                        return sprite;
                }
                break;
            }
        }
        return npcImg.sprite; 
    }

    /// <summary>
    /// Fades in the gray background to prioritize the dialogue
    /// </summary>
    private void Fade()
    {
        if (fadeIn)
        {
            Color color = backgroundImg.color;
            if (color.a < .36f)
            {
                color.a += Time.deltaTime;
                if (color.a >= .36f)
                {
                    color.a = .36f;
                    fadeIn = false;
                }
                backgroundImg.color = color;
            }
        }
        if (fadeOut)
        {
            Color color = backgroundImg.color;
            if (color.a > 0)
            {
                color.a -= Time.deltaTime;
                if (color.a <= 0)
                {
                    color.a = 0;
                    fadeOut = false;
                }
                backgroundImg.color = color;
            }
        }
    }

}