using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TextAsset dialogueJsonFile;
    private DialogueData currentDialogueData;
    private string currentDialogueID;
    private int dialogueIndex;

    void Start()
    {
        if (dialogueJsonFile != null)
        {
            currentDialogueData = JsonUtility.FromJson<DialogueData>("{\"dialogueLines\":" + dialogueJsonFile.text + "}");
        }
        StartDialogue();
    }

    public void StartDialogue()
    {
        dialogueIndex = 0;
        currentDialogueID = "npc1_greeting_1";
        Debug.Log("Dialogue starts");
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (dialogueIndex < currentDialogueData.dialogueLines.Length)
        {
            DialogueLine line = currentDialogueData.dialogueLines[dialogueIndex];
            if (line.dialogueID == currentDialogueID)
            {
                Debug.Log(line.dialogueText);
                if (line.nextDialogueID != "")
                {
                    currentDialogueID = line.nextDialogueID;
                }
            }
            dialogueIndex++;
            DisplayNextLine(); 
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        Debug.Log("Dialogue end");
    }

}