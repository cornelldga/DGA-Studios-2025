using UnityEngine;
using UnityEditor;
using System.IO;

public class DialogueEditor : EditorWindow
{
    string fileName = "Bob";
    // dialogueGameProgress is based on in-game events like defeating bosses and increases sequentially (starts with 0);
    // for example, defeating the first boss increases the dialogueGameProgress to 1, so this dialogue would be available
    // to the player only after the first boss is defeated and before the second boss is defeated.
    int dialogueGameProgress = 0;
    // dialogueSequenceProgress refers to this specific sequence of dialogue and increases sequentially (starts with 0);
    int dialogueSequenceProgress = 0;
    int nextDialogueGameProgress = 0;
    int nextDialogueSequenceProgress = 1;
    string dialogueText = "Hello!";
    string emotion;
    string jsonText;

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
        // The in-game progress of the player will not change during a dialogue sequence, so we set it to the 
        // dialogueGameProgress value
        nextDialogueGameProgress = dialogueGameProgress;
        dialogueSequenceProgress = EditorGUILayout.IntSlider("Current Dialogue Frame", dialogueSequenceProgress,
            0, 15);
        dialogueText = EditorGUILayout.TextField("Dialogue Text", dialogueText);

        Rect dropdownRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
        if (EditorGUI.DropdownButton(dropdownRect, new GUIContent("Select Option"), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Neutral"), false, OnOptionSelected, "neutral");
            menu.AddItem(new GUIContent("Happy"), false, OnOptionSelected, "happy");
            menu.AddItem(new GUIContent("Sad"), false, OnOptionSelected, "sad");
            menu.AddItem(new GUIContent("Angry"), false, OnOptionSelected, "angry");
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
        if (nextDialogueSequenceProgress != -1 && nextDialogueSequenceProgress < dialogueSequenceProgress)
        {
            Debug.LogError("The Next Dialogue Frame should not be less than the Current Dialogue" +
                "Frame unless set to -1 (in which case, there is no dialogue to transition to).");
            return;
        }
        if (emotion == "")
        {
            Debug.LogError("Please select an emotion.");
            return;
        }
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName + ".json");
        File.WriteAllText(filePath, WriteJSON());
    }

    private string WriteJSON()
    {
        DialogueLine currentLine = new DialogueLine();
        currentLine.dialogueID = "progress" + dialogueGameProgress.ToString() + "_dialogue"
            + dialogueSequenceProgress.ToString();
        jsonText = jsonText + currentLine;

        return jsonText;
    }


}
