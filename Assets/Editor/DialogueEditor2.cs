using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static Codice.Client.Common.Connection.AskCredentialsToUser;

public class DialogueEditor2 : EditorWindow
{
    string fileName = "";
    int dialogueGameProgress = 0;
    string dialogueName = "start";
    string nextDialogueName = "";
    string dialogueText = "";
    DialogueEmotion emotion;

    const string kFolder = "Assets/Dialogue/Dialogue Text";
    const string kExt = ".json";

    [MenuItem("Tools/Dialogue Editor 2")]
    public static void ShowWindow()
    {
        var win = GetWindow<DialogueEditor2>();
        win.titleContent = new GUIContent("Dialogue Editor");
        win.minSize = new Vector2(420, 340);
    }

    void OnGUI()
    {
        GUILayout.Label("Create / Update Dialogue", EditorStyles.boldLabel);

        fileName = EditorGUILayout.TextField("Character Name (file)", fileName);
        dialogueGameProgress = EditorGUILayout.IntSlider("In-Game Progress", dialogueGameProgress, 0, 99);
        dialogueName = EditorGUILayout.TextField("Dialogue Name", dialogueName);

        EditorGUILayout.LabelField("Dialogue Text");
        using (new EditorGUILayout.VerticalScope("box"))
        {
            dialogueText = EditorGUILayout.TextArea(dialogueText, GUILayout.MinHeight(80));
        }

        emotion = (DialogueEmotion)EditorGUILayout.EnumPopup("Emotion", emotion);

        EditorGUILayout.HelpBox(
            "The dialogue name \"start\" indicates the start of dialogue on that progress level."+
            "Leave \"Next Dialogue\" empty to end the dialogue.",
            MessageType.Info);

        nextDialogueName = EditorGUILayout.TextField("Next Dialogue", nextDialogueName);

        GUILayout.Space(8);
        using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(dialogueName)))
        {
            if (GUILayout.Button("Create / Update JSON", GUILayout.Height(32)))
            {
                CreateOrUpdateDialogue();
            }
        }
    }
    /// <summary>
    /// Creates or updates dialogue by generating the line and either replacing it the id already exists or adds it
    /// to the specified json file
    /// </summary>
    void CreateOrUpdateDialogue()
    {
        EnsureFolder(kFolder);

        string assetPath = Path.Combine(kFolder, fileName + kExt).Replace("\\", "/");
        string diskPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + assetPath;

        DialogueData data = ReadDialogueData(assetPath);
        DialogueLine newLine = BuildLine();

        // Replace any existing line with the same dialogueID
        bool replaced = false;
        for (int i = data.dialogueLines.Count - 1; i >= 0; --i)
        {
            if (data.dialogueLines[i].dialogueID == newLine.dialogueID)
            {
                data.dialogueLines.RemoveAt(i);
                replaced = true;
            }
        }
        data.dialogueLines.Add(newLine);

        // Write Json
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(diskPath, json);
        AssetDatabase.Refresh();

        if (replaced)
            ShowNotification(new GUIContent($"Updated {newLine.dialogueID} in {fileName}{kExt}"));
        else
            ShowNotification(new GUIContent($"Added {newLine.dialogueID} to {fileName}{kExt}"));

        Debug.Log("Dialogue saved: {assetPath}\n{json}");
    }

    /// <summary>
    /// Builds the dialogue line that contains inputted info
    /// </summary>
    /// <returns></returns>
    DialogueLine BuildLine()
    {
        var line = new DialogueLine
        {
            // DialogueManager uses this formating to read the correct dialogue line
            // Ex: if dialogueName = "yap attack" and dialogueGameProgress = 1, then dialogueID = "1_yap attack"
            dialogueID = $"{dialogueGameProgress}_{dialogueName}",
            dialogueText = dialogueText,
            emotion = emotion,
            nextDialogueID = string.IsNullOrWhiteSpace(nextDialogueName)
            ? ""
            : $"{dialogueGameProgress}_{nextDialogueName.Trim()}"
        };
        return line;
    }
    /// <summary>
    /// Reads the dialogue from the json
    /// </summary>
    /// <param name="assetPath">location of the json</param>
    /// <returns></returns>
    DialogueData ReadDialogueData(string assetPath)
    {
        var data = new DialogueData();

        TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
        if (ta == null || string.IsNullOrWhiteSpace(ta.text))
            return data;

        string json = ta.text.Trim();

        var parsed = JsonUtility.FromJson<DialogueData>(json);
        if (parsed != null && parsed.dialogueLines != null)
            return parsed;

        return data;
    }

    /// <summary>
    /// Insures that the referenced file path exists in order to edit/update that json
    /// </summary>
    /// <param name="projectRelativePath"></param>
    static void EnsureFolder(string projectRelativePath)
    {
        string[] parts = projectRelativePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        string current = "Assets";
        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }
            current = next;
        }
    }
}
