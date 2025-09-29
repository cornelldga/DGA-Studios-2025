using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine;
using Unity.Multiplayer.Center.Common;

public class DialogueManager : MonoBehaviour
{
    public TextAsset dialogueJsonFile;
    private DialogueData currentDialogueData;
    private string currentDialogueID;
    private int dialogueIndex;
    private Choice[] choices;
    public Animator animator;
    [SerializeField] public GameObject character;
    [SerializeField] private GameObject choiceDisplay;
    [SerializeField] private TextMeshProUGUI firstChoiceText;
    [SerializeField] private TextMeshProUGUI secondChoiceText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float typingSpeed = 0.05f;
    private Image characterImg;
    void Start()
    {
        characterImg = character.GetComponent<Image>();
        if (dialogueJsonFile != null)
        {
            currentDialogueData = JsonUtility.FromJson<DialogueData>("{\"dialogueLines\":" + dialogueJsonFile.text + "}");
        }
    }

    public void StartDialogue(string dialogueID)
    {
        animator.SetBool("isOpen", true);
        dialogueIndex = 0;
        currentDialogueID = dialogueID;
        choiceDisplay.SetActive(false);
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (dialogueIndex < currentDialogueData.dialogueLines.Length)
        {
            DialogueLine line = currentDialogueData.dialogueLines[dialogueIndex];
            if (line.dialogueID == currentDialogueID)
            {
                StopAllCoroutines();
                StartCoroutine(TypeSentence(line));
                if (line.choices.Length == 0)
                {
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
                else
                {
                    choices = line.choices;
                    Choices();
                }
            }
            dialogueIndex++;
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
        // click anywhere, freeze game events
        if (true)
        {
            //if clicked
            DisplayNextLine();
        }
    }

    private void Choices()
    {
        choiceDisplay.SetActive(true);
        firstChoiceText.text = choices[0].choiceText;
        secondChoiceText.text = choices[1].choiceText;
    }

    public void FirstChoice()
    {
        currentDialogueID = choices[0].nextDialogueChoiceID;
        choiceDisplay.SetActive(false);
        DisplayNextLine();
    }

    public void SecondChoice()
    {
        currentDialogueID = choices[1].nextDialogueChoiceID;
        choiceDisplay.SetActive(false);
        DisplayNextLine();
    }  

    public void EndDialogue()
    {
        animator.SetBool("isOpen", false);
    }

}