using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Handles all universal cutscene logic and individual cutscenes
/// </summary>
public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    [Tooltip("Animator for the cutscene")]
    [SerializeField] Animator cutsceneAnimator;
    private float[] frameTimestamps;
    private float clipLength;
    private string stateName;
    private System.Action onComplete;
    private float lastInputTime;
    private const float INPUT_COOLDOWN = 0.2f;

    [Header("Auto Timing")]
    [Tooltip("Time between each character being typed")]
    [SerializeField] float typingSpeed = 0.03f;

    [Tooltip("Delay after each line before advancing")]
    [SerializeField] float autoAdvanceDelay = 2f;

    [Tooltip("Delay after final line before ending the cutscene")]
    [SerializeField] float finalPanelDelay = 2f;

    [Header("Cutscene Details")]
    [SerializeField] GameObject introOverlay;
    [SerializeField] GameObject skipButton;

    [Header("Dialogue Details")]
    [SerializeField] TextAsset cutscene_1;
    [SerializeField] Sprite dialogueBoxSprite;
    [SerializeField] Sprite dukeSprite;
    [SerializeField] TextMeshProUGUI cutsceneNameText;

    [Header("Backstory Text")]
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextAsset backstoryJson;
    private BackstoryData backstoryData;

    // Runtime state
    private bool isActive;
    private bool isTyping;

    // Panel tracking
    private int currentPanelIndex;
    private int currentLineIndex;

    // Coroutines
    private Coroutine typingCoroutine;
    private Coroutine advanceCoroutine;

    private static float[] backstoryTimestamps = { 0f, 15f, 25f, 40f, 55f, 70f, 85f, 100f, 115f, 130f, 145f };
    private const float backstoryClipLength = 155f;

    [System.Serializable]
    private class BackstoryData
    {
        public BackstoryPanel[] panels;
    }

    [System.Serializable]
    private class BackstoryPanel
    {
        public string[] lines;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (backstoryJson != null)
        {
            backstoryData = JsonUtility.FromJson<BackstoryData>(backstoryJson.text);
        }
    }

    void Update()
    {
        if (!isActive) return;

        if (Keyboard.current != null &&
            (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame))
        {
            HandleContinue();
            return;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleContinue();
        }
    }

    void HandleContinue()
    {
        if (!isActive) return;
        if (Time.time <= lastInputTime + INPUT_COOLDOWN) return;

        lastInputTime = Time.time;

        if (isTyping)
        {
            CompleteCurrentLine();
            return;
        }

        AdvanceImmediately();
    }

    /// <summary>
    /// Starts the backstory cutscene.
    /// </summary>
    public void PlayBackstoryCutscene(System.Action onComplete = null)
    {
        if (introOverlay != null) introOverlay.SetActive(true);

        StartCutscene("backstory_cutscene", backstoryTimestamps, backstoryClipLength, () =>
        {
            if (GameManager.Instance != null)
            {
                onComplete?.Invoke();
            }
        });
    }

    /// <summary>
    /// Starts the meet bobby cutscene.
    /// </summary>
    /// <param name="onComplete">action to execute when the cutscene completes</param>
    public void PlayMeetBobbyCutscene(System.Action onComplete = null)
    {
        isActive = true;
        skipButton.SetActive(true);
        SetCutsceneName("Duke");
        DialogueManager dialogueManager = GameManager.Instance.GetDialogueManager;
        dialogueManager.SetNameTextVisible(false);
        this.onComplete = () =>
        {
            // GameManager.Instance.LoadScene("Tutorial"); <- wait for tutorial to be complete to uncomment
            PlayerPrefs.SetInt("progression", 1);
            onComplete?.Invoke();
        };

        dialogueManager.StartDialogue(cutscene_1, dialogueBoxSprite,
            new Dictionary<DialogueEmotion, Sprite> { { DialogueEmotion.Neutral, dukeSprite } },
            null, DialogueType.NPC, "Tutorial");
        StartCoroutine(WaitForDialogueEnd());
    }

    /// <summary>
    /// Updates the cutscene-specific name text.
    /// </summary>
    public void SetCutsceneName(string speakerName)
    {
        if (cutsceneNameText == null) return;

        cutsceneNameText.gameObject.SetActive(true);
        cutsceneNameText.text = speakerName;
    }

    /// <summary>
    /// Waits for the dialogue to finish before ending the cutscene.
    /// </summary>
    IEnumerator WaitForDialogueEnd()
    {
        yield return null;
        DialogueManager dialogueManager = GameManager.Instance.GetDialogueManager;
        yield return new WaitUntil(() => !dialogueManager.OngoingDialogue());
        EndCutscene();
    }

    /// <summary>
    /// Starts a cutscene with the given state, frame timestamps, and clip length.
    /// </summary>
    void StartCutscene(string state, float[] timestamps, float length, System.Action onComplete = null)
    {
        frameTimestamps = timestamps;
        clipLength = length;
        stateName = state;
        this.onComplete = onComplete;
        isActive = true;

        currentPanelIndex = 0;
        currentLineIndex = 0;
        lastInputTime = -999f;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (advanceCoroutine != null) StopCoroutine(advanceCoroutine);

        cutsceneAnimator.gameObject.SetActive(true);
        skipButton.SetActive(true);
        cutsceneAnimator.Play(stateName, 0, 0f);
        cutsceneAnimator.speed = 1f;

        GameManager.Instance.FreezePlayer(true);
        StartPanelText(0);
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        if (advanceCoroutine != null) StopCoroutine(advanceCoroutine);
        advanceCoroutine = StartCoroutine(AutoAdvance());
    }

    void CompleteCurrentLine()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        isTyping = false;
        dialogueText.text = backstoryData.panels[currentPanelIndex].lines[currentLineIndex];

        if (advanceCoroutine != null) StopCoroutine(advanceCoroutine);
        advanceCoroutine = StartCoroutine(AutoAdvance());
    }

    IEnumerator AutoAdvance()
    {
        float delay = (currentPanelIndex == backstoryData.panels.Length - 1 &&
                       currentLineIndex == backstoryData.panels[currentPanelIndex].lines.Length - 1)
            ? finalPanelDelay
            : autoAdvanceDelay;

        yield return new WaitForSeconds(delay);

        if (!isActive) yield break;

        if (currentLineIndex < backstoryData.panels[currentPanelIndex].lines.Length - 1)
        {
            currentLineIndex++;
            typingCoroutine = StartCoroutine(TypeLine(backstoryData.panels[currentPanelIndex].lines[currentLineIndex]));
        }
        else
        {
            if (currentPanelIndex == backstoryData.panels.Length - 1)
                EndCutscene();
            else
                SkipToNextTimestamp();
        }
    }

    void StartPanelText(int panelIndex)
    {
        if (backstoryData == null || backstoryData.panels == null) return;
        if (panelIndex >= backstoryData.panels.Length) return;

        currentPanelIndex = panelIndex;
        currentLineIndex = 0;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (advanceCoroutine != null) StopCoroutine(advanceCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(backstoryData.panels[panelIndex].lines[0]));
    }

    void AdvanceImmediately()
    {
        if (advanceCoroutine != null) StopCoroutine(advanceCoroutine);

        if (currentLineIndex < backstoryData.panels[currentPanelIndex].lines.Length - 1)
        {
            currentLineIndex++;
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeLine(backstoryData.panels[currentPanelIndex].lines[currentLineIndex]));
        }
        else
        {
            if (currentPanelIndex == backstoryData.panels.Length - 1)
                EndCutscene();
            else
                SkipToNextTimestamp();
        }
    }

    /// <summary>
    /// Skips to the next timestamp in the cutscene. If on the last frame, ends the cutscene.
    /// </summary>
    public void SkipToNextTimestamp()
    {
        if (string.IsNullOrEmpty(stateName)) return;

        int nextIndex = currentPanelIndex + 1;

        if (nextIndex >= frameTimestamps.Length)
        {
            EndCutscene();
            return;
        }

        cutsceneAnimator.Play(stateName, 0, frameTimestamps[nextIndex] / clipLength);
        cutsceneAnimator.speed = 1f;
        cutsceneAnimator.Update(0f);
        StartPanelText(nextIndex);
    }

    /// <summary>
    /// Ends the cutscene, hides the cutscene animator, unfreezes the player, and invokes the onComplete action.
    /// </summary>
    public void EndCutscene()
    {
        if (!isActive) return;
        isActive = false;
        stateName = null;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (advanceCoroutine != null) StopCoroutine(advanceCoroutine);

        if (dialogueText != null) dialogueText.text = "";
        if (introOverlay != null) introOverlay.SetActive(false);
        if (cutsceneAnimator != null) cutsceneAnimator.gameObject.SetActive(false);
        if (skipButton != null) skipButton.SetActive(false);

        if (cutsceneNameText != null)
        {
            cutsceneNameText.text = "";
            cutsceneNameText.gameObject.SetActive(false);
        }

        DialogueManager dialogueManager = GameManager.Instance.GetDialogueManager;
        dialogueManager.SetNameTextVisible(true);

        if (dialogueManager.gameObject.activeSelf)
        {
            dialogueManager.EndDialogue();
        }

        GameManager.Instance.FreezePlayer(false);

        onComplete?.Invoke();
    }
}