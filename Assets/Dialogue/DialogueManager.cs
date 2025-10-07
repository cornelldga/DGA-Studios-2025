using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script iterates through a dialogue sequence from a JSON file
/// using DialogueLine and DialogueData.
/// </summary>

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private DialogueData currentDialogueData;
    private string currentDialogueID;
    public bool dialogueOngoing;
    public Animator animator;
    private bool fadeIn;
    private bool fadeOut;
    private bool isFading;
    [SerializeField] private Sprite[] characters;
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

    void Update()
    {
        if (isFading)
        {
            Fade();
        }
    }
    /// <summary>
    /// Dialogue begins, calls DisplayNextLine() to begin dialogue sequence.
    /// </summary>
    /// <param name="dialogueID">The dialogueID of the first dialogue to show.</param>
    public void StartDialogue(TextAsset file, string dialogueID)
    {
        // GameManager.Instance.pausePlayer();
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
            
            if (characters != null && characters.Length > 0)
            {
                npcImg.sprite = characters[0];
                foreach (Sprite character in characters)
                {
                    if (character != null && character.name == file.name)
                    {
                        npcImg.sprite = character;
                        break;
                    }
                }
            }

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
        if (currentDialogueData.dialogueLines.Length > 0)
        {
            foreach (DialogueLine line in currentDialogueData.dialogueLines)
            {
                if (line.dialogueID == currentDialogueID)
                {
                    StopAllCoroutines();
                    StartCoroutine(TypeSentence(line));
                }
            }
        }
    }

    /// <summary>
    /// Does the animation for typing text and continues after a short time,
    /// </summary>
    IEnumerator TypeSentence(DialogueLine line)
    {
        dialogueText.text = "";
        foreach (char letter in line.dialogueText.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(2f);
        if (line.nextDialogueID == "")
        {
            EndDialogue();
        }
        else
        {
            currentDialogueID = line.nextDialogueID;
            DisplayNextLine();
        }
    }

    /// <summary>
    /// Dialogue ends, and UI is removed from the screen.
    /// </summary>
    public void EndDialogue()
    {
        // GameManager.Instance.unPausePlayer();
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