using TMPro;
using System.Collections;
using UnityEngine;
using System;

/// <summary>
/// This script iterates through a dialogue sequence from a JSON file
/// using DialogueLine and DialogueData.
/// </summary>

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private DialogueData currentDialogueData;
    private string currentDialogueID;
    private bool dialogueOngoing;
    public Animator animator;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
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
    /// Dialogue begins, calls DisplayNextLine() to begin dialogue sequence.
    /// </summary>
    /// <param name="dialogueID">The dialogueID of the first dialogue to show.</param>
    public void StartDialogue(TextAsset file, string dialogueID)
    {
        animator.SetBool("isOpen", true);
        if (file != null)
        {
            currentDialogueData = JsonUtility.FromJson<DialogueData>
                ("{\"dialogueLines\":" + file.text + "}");
            currentDialogueID = "DIALOGUE";
            DisplayNextLine();
        }
    }

    /// <summary>
    /// Searches for a DialogueLine with ID matching currentDialogueID, begins
    /// Coroutine to display dialogue; if nextDialogueID is empty, ends dialogue;
    /// else, sets currentDialogueID to nextDialogueID and continues,
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
                    if (line.nextDialogueID == "")
                    {
                        EndDialogue();
                    }
                    else
                    {
                        currentDialogueID = line.nextDialogueID;
                        Continue();
                    }
                }
            }
        }
    }

    IEnumerator TypeSentence(DialogueLine line)
    {
        dialogueText.text = "";
        foreach (char letter in line.dialogueText.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void Continue()
    {
        if (dialogueOngoing)
        {
            DisplayNextLine(); 
        }
        
    }

    /// <summary>
    /// Dialogue ends, and UI is removed from the screen.
    /// </summary>
    public void EndDialogue()
    {
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

}