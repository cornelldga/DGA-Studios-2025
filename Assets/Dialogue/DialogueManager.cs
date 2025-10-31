using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
/// This script iterates through a dialogue sequence from a JSON file
/// using DialogueLine and DialogueData.
/// </summary>

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public Animator dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image backgroundImg;
    [SerializeField] private Image npcImg;
    [SerializeField] private float typingSpeed;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    DialogueData currentDialogueData;
    string currentDialogueID;
    Dictionary<DialogueEmotion, Sprite> currentEmotions;
    bool ongoingDialogue = false;

    bool isTyping;
    bool bossFight;
    bool fadeIn;
    bool fadeOut;
    bool isFading;
    string sceneName;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// Returns if there is ongoing dialouge
    /// </summary>
    public bool OngoingDialogue()
    {
        return ongoingDialogue;
    }

    public void OnContinueDialogue()
    {
        if (ongoingDialogue)
        {
            if (isTyping)
            {
                CompleteCurrentLine();
            }
            else
            {
                DisplayNextLine();
                Debug.Log("display");
            }
        }
    }

    /// <summary>
    /// Checks if they are clicking through dialogue and if the fade for blurring background needs to occur.
    /// </summary>
    private void Update()
    {
        if (isFading)
        {
            Fade();
        }
    }

    /// <summary>
    /// Dialogue begins by finding the start dialogue associated with the  specified progress.
    /// Calls DisplayNextLine() to begin dialogue sequence.
    /// Changes the npcImg sprite to the correct characters sprite
    /// </summary>
    /// <param name="progress">The progression number of the npc's dialogue</param>
    /// <param name="file">The json file associated to the specific character.</param>
    /// <param name="dialogueBoxSprite">The dialogue box sprite</param>
    /// <param name="emotionDictionary">The dictionary of sprites associated to the character's emotions.</param>
    public void StartDialogue(TextAsset file, int progress, Sprite dialogueBoxSprite,
        Dictionary<DialogueEmotion, Sprite> emotionDictionary, string scene, bool boss)
    {
        if (file != null)
        {
            ongoingDialogue = true;
            dialogueBox.SetBool("isOpen", true);
            currentDialogueData = JsonUtility.FromJson<DialogueData>(file.text);
            // Format followed by DialogueEditor.BuildLine()
            currentDialogueID = progress.ToString() + "_" + "start";
            sceneName = scene;
            bossFight = boss;
            dialogueBox.GetComponent<Image>().sprite = dialogueBoxSprite;
            currentEmotions = emotionDictionary;
            GameManager.Instance.FreezePlayer(true);
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
        if (currentDialogueID == "")
        {
            if (bossFight)
            {
                DialogueChoice();
            } else
            {
                EndDialogue();
            }
            return;
        }
        if (currentDialogueData.dialogueLines.Count > 0)
        {
            foreach (DialogueLine line in currentDialogueData.dialogueLines)
            {
                if (line.dialogueID == currentDialogueID)
                {
                    npcImg.sprite = currentEmotions[(DialogueEmotion)line.emotion];
                    StopAllCoroutines();
                    StartCoroutine(TypeSentence(line));
                    return; 
                }
            }
            throw new System.Exception("No dialogue lines found");
        }
    }

    /// <summary>
    /// Does the animation for typing text.
    /// </summary>
    /// <param name="line">The line of text from the JSON which is being displayed.</param>
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
        ongoingDialogue = false;
        GameManager.Instance.FreezePlayer(false);
        dialogueBox.SetBool("isOpen", false); 
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
    /// Sets UI buttons active.
    /// </summary>
    private void DialogueChoice()
    {
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivates UI buttons and loads new scene.
    /// </summary>
    public void Fight()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        GameManager.Instance.LoadScene(sceneName);
    }

    /// <summary>
    /// Sets UI buttons inactive and ends dialogue.
    /// </summary>
    public void NoFight()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        EndDialogue();
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
        else if (fadeOut)
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