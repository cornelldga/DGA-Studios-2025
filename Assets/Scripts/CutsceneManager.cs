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

    [Header("Backstory Text")]
    [SerializeField] TextMeshProUGUI dialogueText;
    // Runtime state
    private bool isActive;
    private bool isTyping;

    // Panel tracking
    private int currentPanelIndex;
    private int currentLineIndex;

    // Coroutines
    private Coroutine typingCoroutine;
    private Coroutine advanceCoroutine;
    private static string[][] backstoryLines = new string[][]
    {
        new string[] { "Hey!", "You! Yeah you! The one in front of the screen!", "Have you heard about the grand tale of Duke Sharpshot?", "…", "What?! Of course it's a family name!", "I'll tell you about the awe-inspiring and only slightly drunken story of Duke Sharpshot!" },
        new string[] { "Our story begins in the distant Town of \"Yonder\"…", "Or as the locals more affectionately call it:", "The Devil's Bowel." },
        new string[] { "Poverty turned the once-bustling mining town into a god-forsaken hellscape.", "For the assassins, thieves, and greedy moneymen that lived there...", "laws were suggestions, and crime was a virtue.", "Even the virtuous groveled at the feet of these criminals, for their own safety." },
        new string[] { "Well… all of them except one.", "For every lowlife eventually had to contend with the mighty…", "bold…", "brave…", "And all other relevant synonyms in my thesaurus." },
        new string[] { "Duke Sharpshot.", "Though I presume you could've already surmised that…" },
        new string[] { "Anyone within 20 miles of Yonder knew the name.", "His reflexes were lightning, his aim surgical, his resolve unbreakable.", "In a sea of devils and monsters, Duke was the lone battle angel.", "Just picture an angel who wields a .44 Magnum." },
        new string[] { "Though even angels have their limits.", "No matter how many duels he won, the crime only grew — 10-fold each year.", "He survived on 2-hour nights, the screams of the innocent haunting his sleep.", "The guilt for those he couldn't save echoed through his every waking moment." },
        new string[] { "After years of this cycle, Duke finally caved.", "The crime would persist no matter his efforts. So he walked away.", "Yonder's rot will fester, he told himself, with or without me." },
        new string[] { "The civilians, however, didn't share such acceptance.", "As small a light as he was — it was the only one they had.", "Without Duke, Yonder was truly doomed.", "But still, Duke kept walking." },
        new string[] { "He walked without direction. Hours became days. Days became weeks.", "The desert outside Yonder had swallowed stronger men whole.", "But Duke, fueled by guilt and a hunger for punishment, kept marching." },
        new string[] { "Eventually, his legs gave away.", "He began crawling through the desert, still without direction", "Just when his body couldn’t take anymore", "when he was about to escape the desert on the back of the Reaper itself…", "Hero's Haven..." }
    };
    private static float[] backstoryTimestamps = { 0f, 15f, 25f, 40f, 55f, 70f, 85f, 100f, 115f, 130f, 145f };
    private const float backstoryClipLength = 155f;

    // Temp variables
    private int temp;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        temp = 0;
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
    /// <param name="onComplete">action to execute when the cutscene completes</param>
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
        this.onComplete = () =>
        {
            // GameManager.Instance.LoadScene("Tutorial"); <- wait for tutorial to be complete to uncomment
            onComplete?.Invoke();
        };
        GameManager.Instance.GetDialogueManager.StartDialogue(cutscene_1, dialogueBoxSprite,
            new Dictionary<DialogueEmotion, Sprite> { { DialogueEmotion.Neutral, dukeSprite } },
            "Tutorial", DialogueType.NPC);
        StartCoroutine(WaitForDialogueEnd());

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
    /// <param name="state">name of cutscene</param>
    /// <param name="timestamps">array of timestamps for each frame</param>
    /// <param name="length">length of the cutscene clip</param>
    /// <param name="onComplete">action to execute when the cutscene completes</param>
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
        dialogueText.text = backstoryLines[currentPanelIndex][currentLineIndex];

        if (advanceCoroutine != null) StopCoroutine(advanceCoroutine);
        advanceCoroutine = StartCoroutine(AutoAdvance());
    }

    IEnumerator AutoAdvance()
    {
        float delay = (currentPanelIndex == backstoryLines.Length - 1 &&
                       currentLineIndex == backstoryLines[currentPanelIndex].Length - 1)
            ? finalPanelDelay
            : autoAdvanceDelay;

        yield return new WaitForSeconds(delay);

        if (!isActive) yield break;

        if (currentLineIndex < backstoryLines[currentPanelIndex].Length - 1)
        {
            currentLineIndex++;
            typingCoroutine = StartCoroutine(TypeLine(backstoryLines[currentPanelIndex][currentLineIndex]));
        }
        else
        {
            if (currentPanelIndex == backstoryLines.Length - 1)
                EndCutscene();
            else
                SkipToNextTimestamp();
        }
    }

    void StartPanelText(int panelIndex)
    {
        if (panelIndex >= backstoryLines.Length) return;

        currentPanelIndex = panelIndex;
        currentLineIndex = 0;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (advanceCoroutine != null) StopCoroutine(advanceCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(backstoryLines[panelIndex][0]));
    }

    void AdvanceImmediately()
    {
        if (advanceCoroutine != null) StopCoroutine(advanceCoroutine);
        if (currentLineIndex < backstoryLines[currentPanelIndex].Length - 1)
        {
            currentLineIndex++;
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeLine(backstoryLines[currentPanelIndex][currentLineIndex]));
        }
        else
        {
            if (currentPanelIndex == backstoryLines.Length - 1)
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
        if (introOverlay != null && introOverlay.activeSelf) introOverlay.SetActive(false);
        if (cutsceneAnimator != null && cutsceneAnimator.gameObject.activeSelf) cutsceneAnimator.gameObject.SetActive(false);

        if (GameManager.Instance.GetDialogueManager.gameObject.activeSelf == true)
        {
            GameManager.Instance?.GetDialogueManager.EndDialogue();
        }

        if (GameManager.Instance != null) GameManager.Instance.FreezePlayer(false); 
        skipButton.SetActive(false);
        onComplete?.Invoke();
        if (temp==0)
        {
            temp = 1;
        } else if (temp==1)
        {
            PlayerPrefs.SetInt("progression", 1);
        }
    }
}