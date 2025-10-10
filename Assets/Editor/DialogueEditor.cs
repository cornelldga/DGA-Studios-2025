using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Codice.Client.Common;

public class DialogueEditor : EditorWindow
{
    string fileName = "Bob";
    // dialogueGameProgress is based on in-game events like defeating bosses and increases sequentially (starts with 0);
    // for example, defeating the first boss increases the dialogueGameProgress to 1, so this dialogue would be available
    // to the player only after the first boss is defeated and before the second boss is defeated.
    int dialogueGameProgress = 0;
    // dialogueSequenceProgress refers to this specific sequence of dialogue and increases sequentially (starts with 0);
    int dialogueSequenceProgress = 0;
    int nextDialogueSequenceProgress = 1;
    string dialogueText = "Hello!";
    string emotion = "";

    [MenuItem("Tools/Dialogue Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DialogueEditor));
    }

    private void OnGUI()
    {
        GUILayout.Label("Create New Dialogue Script", EditorStyles.boldLabel);
        fileName = EditorGUILayout.TextField("Character Name", fileName);
        dialogueGameProgress = EditorGUILayout.IntSlider("In-Game Progress", dialogueGameProgress, 0, 5);
        dialogueSequenceProgress = EditorGUILayout.IntSlider("Current Dialogue Frame", dialogueSequenceProgress,
            0, 15);
       //if (GUILayout.Button("View Dialogue"))
        //{
        //    ViewDialogue();
        //}
        //if (GUILayout.Button("Edit Dialogue"))
        //{
       //     EditDialogue();
       // }
       // if (GUILayout.Button("Delete Dialogue"))
       // {
       //     DeleteDialogue();
       // }
        dialogueText = EditorGUILayout.TextField("Dialogue Text", dialogueText);

        Rect dropdownRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
        string dropdownLabel = string.IsNullOrEmpty(emotion) ? "Select Emotion" : $"{emotion}";
        if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(dropdownLabel), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("neutral"), false, OnOptionSelected, "neutral");
            menu.AddItem(new GUIContent("happy"), false, OnOptionSelected, "happy");
            menu.AddItem(new GUIContent("sad"), false, OnOptionSelected, "sad");
            menu.AddItem(new GUIContent("angry"), false, OnOptionSelected, "angry");
            menu.ShowAsContext();
        }

        void OnOptionSelected(object userData)
        {
            if (userData != null)
            {
                emotion = userData.ToString();
            }
        }

        nextDialogueSequenceProgress = EditorGUILayout.IntSlider("Next Dialogue Frame" +
            " (if there is no dialogue to transition to, set this to -1)",
            nextDialogueSequenceProgress, -1, 15);

        if (GUILayout.Button("Create Dialogue"))
        {
            CreateDialogue();
        }
    }

    private void CreateDialogue()
    {
        if (fileName == "")
        {
            Debug.LogError("Please enter a character name.");
            return;
        }
        if (dialogueText == "")
        {
            Debug.LogError("Please enter dialogue text.");
            return;
        }
        if (emotion == "")
        {
            Debug.LogError("Please select an emotion.");
            return;
        }
        if (dialogueSequenceProgress == nextDialogueSequenceProgress)
        {
            Debug.LogError("The Current Dialogue Frame cannot be the same as the Next Dialogue Frame");
        }
        string filePath = Path.Combine(Application.dataPath, "Dialogue/Dialogue Text",
            fileName + ".json");
        if (File.Exists(filePath))
        {
            List<DialogueLine> lines = JsonUtility.FromJson<DialogueData>
                (File.ReadAllText(filePath)).dialogueLines;
            DialogueLine newLine = WriteJSON();
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                if (lines[i].dialogueID == newLine.dialogueID)
                {
                    lines.RemoveAt(i);
                }
            }
            lines.Add(newLine);
            File.WriteAllText(filePath,JsonUtility.ToJson(new DialogueData { dialogueLines = lines }));
        }
        else
        {
            List<DialogueLine> lines = new List<DialogueLine>();
            lines.Add(WriteJSON());
            File.WriteAllText(filePath, JsonUtility.ToJson(new DialogueData { dialogueLines = lines}));
        }
    }

    private void ViewDialogue()
    {
        
    }

    private void EditDialogue()
    {

    }

    private void DeleteDialogue()
    {
        
    }

    private DialogueLine WriteJSON()
    {
        DialogueLine currentLine = new DialogueLine();
        currentLine.dialogueID = "progress" + dialogueGameProgress.ToString() + "_dialogue"
            + dialogueSequenceProgress.ToString();
        currentLine.dialogueText = dialogueText;
        currentLine.emotion = emotion;
        string nextDialogueString = nextDialogueSequenceProgress.ToString();
        if (nextDialogueSequenceProgress == -1)
        {
            nextDialogueString = "";
        }
        currentLine.nextDialogueID = "progress" + dialogueGameProgress.ToString() + "_dialogue"
            + nextDialogueString;
        return currentLine;
    }


}
