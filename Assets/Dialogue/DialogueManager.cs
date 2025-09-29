using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TextAsset dialogueJsonFile;
    private DialogueData currentDialogueData;
    private string currentDialogueID;
    private int dialogueIndex;
    public Animator animator;
    [SerializeField] public GameObject character;
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
        Debug.Log("start");
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
                if (line.nextDialogueID != "")
                {
                    currentDialogueID = line.nextDialogueID;
                }
            }
            dialogueIndex++;
        }
        else
        {
            EndDialogue();
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

    public void EndDialogue()
    {
        Debug.Log("Dialogue end");
        animator.SetBool("isOpen", false);
    }

}