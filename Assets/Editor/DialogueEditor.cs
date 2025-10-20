using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class DialogueEditor : EditorWindow
{
    string fileName = "Bob";
    // dialogueGameProgress is based on in-game events like defeating bosses and increases sequentially (starts with 0);
    // for example, defeating the first boss increases the dialogueGameProgress to 1, so this dialogue would be available
    // to the player only after the first boss is defeated and before the second boss is defeated.
    int dialogueGameProgress = 0;
    // dialogueName refers to this specific dialogue's unique name identifier (should NEVER be the same as
    // another dialogue's unique name UNLESS they have different gameProgress values);
    string dialogueName = "intro01";
    string nextDialogueName = "";
    string dialogueText = "Hello!";
    DialogueEmotion emotion;

    [MenuItem("Tools/Dialogue Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DialogueEditor));
    }

    /// <summary>
    /// Displays the GUI elements for the different dialogue components.
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("Create New Dialogue Script", EditorStyles.boldLabel);
        fileName = EditorGUILayout.TextField("Character Name", fileName);
        dialogueGameProgress = EditorGUILayout.IntSlider("In-Game Progress", dialogueGameProgress, 0, 5);
        dialogueName = EditorGUILayout.TextField("Dialogue Name", dialogueName);
        dialogueText = EditorGUILayout.TextField("Dialogue Text", dialogueText);

        Rect dropdownRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
        string dropdownLabel = string.IsNullOrEmpty(emotion.ToString()) ? "Select Emotion" : $"{emotion}";
        if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(dropdownLabel), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();
            string[] emotions = Enum.GetNames(typeof(DialogueEmotion));
            foreach (string each in emotions)
            {
                DialogueEmotion emotionChoice = (DialogueEmotion)Enum.Parse(typeof(DialogueEmotion), each);
                menu.AddItem(new GUIContent(emotionChoice.ToString()), false, OnOptionSelected, emotionChoice);
            }
            menu.ShowAsContext();
        }

        void OnOptionSelected(object userData)
        {
            emotion = (DialogueEmotion) userData;
        }

        GUIStyle wrapStyle = new GUIStyle(EditorStyles.label);
            wrapStyle.wordWrap = true;

        GUILayout.Label("Please leave Next Dialogue ID blank if there are no further dialogue " +
            "frames to transition to; otherwise, copy the exact dialogueID here.", wrapStyle);


        nextDialogueName = EditorGUILayout.TextField("Next Dialogue ID", nextDialogueName);

        if (GUILayout.Button("Create Dialogue"))
        {
            CreateDialogue();
        }
    }

    /// <summary>
    /// Writes to a JSON; if the text file exists and the dialogueID is in the file, then
    /// it is replaced. Else, a brand new dialogue is created (or a new text file).
    /// </summary>

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
            File.WriteAllText(filePath, JsonUtility.ToJson(new DialogueData { dialogueLines = lines }));
        }
        else
        {
            List<DialogueLine> lines = new List<DialogueLine>();
            lines.Add(WriteJSON());
            File.WriteAllText(filePath, JsonUtility.ToJson(new DialogueData { dialogueLines = lines }));
        }
    }
    
    /// <summary>
    /// Creates the JSON string to be added to a text file.
    /// </summary>
    /// <returns>DialogueLine</returns>
    private DialogueLine WriteJSON()
    {
        DialogueLine currentLine = new DialogueLine();
        currentLine.dialogueID = "progress" + dialogueGameProgress.ToString() + "_"
            + dialogueName;
        currentLine.dialogueText = dialogueText;
        currentLine.emotion = emotion;
        if (nextDialogueName=="")
        {
            currentLine.nextDialogueID = "";
        } else
        {
        currentLine.nextDialogueID = nextDialogueName; 
        }
        return currentLine;
    }


}
