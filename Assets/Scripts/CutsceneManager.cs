using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles all universal cutscene logic and individual cutscenes
/// </summary>
public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    [Tooltip("Input to advance the cutscene")]
    [SerializeField] InputActionReference continueAction;
    [Tooltip("Animator for the cutscene")]
    [SerializeField] Animator cutsceneAnimator;
    private float[] frameTimestamps;
    private float clipLength;
    private string stateName;
    private int currentFrame;
    private bool active;
    private System.Action onComplete;
    private float lastInputTime;
    private const float inputCooldown = 0.4f; 
    private bool isOnLastFrame = false;
    private float endTimer = 0f;
    private const float autoEndDuration = 5f;

    [Header("Cutscene Details")]
    private static float[] backstoryTimestamps = { 0f, 5f, 10f, 15f, 20f, 25f, 30f, 35f, 40f, 45f };
    private const float backstoryClipLength = 45f;
    [SerializeField] GameObject introOverlay;
    [Tooltip("Button to skip the entire cutscene")]
    [SerializeField] public GameObject skipButton;

    [Header("Dialogue Details")]
    [SerializeField] public TextAsset cutscene_1;
    [SerializeField] public Sprite dialogueBoxSprite;
    [SerializeField] public Sprite dukeSprite;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void OnEnable() => continueAction.action.performed += ContinueCutscene;
    private void OnDisable() => continueAction.action.performed -= ContinueCutscene;

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
                GameManager.Instance.player.progression++;
                GameManager.Instance.player.SavePlayer();
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
        active = true;
        skipButton.SetActive(true);
        this.onComplete = () =>
        {
            GameManager.Instance.player.progression++;
            GameManager.Instance.player.SavePlayer();
            onComplete?.Invoke();
        };
        GameManager.Instance.player.progression++;
        DialogueManager.Instance.StartDialogue(cutscene_1, 1, dialogueBoxSprite,
            new Dictionary<DialogueEmotion, Sprite> { { DialogueEmotion.Neutral, dukeSprite } },
            "Tutorial", DialogueType.NPC);
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
        currentFrame = -1;
        stateName = state;
        this.onComplete = onComplete;
        active = true;

        continueAction.action.Enable();
        cutsceneAnimator.gameObject.SetActive(true);
        skipButton.SetActive(true);
        cutsceneAnimator.Play(stateName, 0, 0f);
        cutsceneAnimator.speed = 1f;

        GameManager.Instance.FreezePlayer(true);
    }

    /// <summary>
    /// Listens to advance cutscene input. This is in case input actions fails.
    /// </summary>
    /// <param name="context"></param>
    void ContinueCutscene(InputAction.CallbackContext context)
    {
        if (active && Time.time > lastInputTime + inputCooldown)
        {
            lastInputTime = Time.time;
            SkipToNextTimestamp();
        }
    }

    /// <summary>
    /// Listens to advance cutscene via space or enter key. This is incase input actions fails.
    /// </summary>
    void Update()
    {
        if (!active) return;

        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            if (Time.time > lastInputTime + inputCooldown)
            {
                lastInputTime = Time.time;
                SkipToNextTimestamp();
            }
        }

        if (isOnLastFrame)
        {
            endTimer += Time.deltaTime;
            if (endTimer >= autoEndDuration)
            {
                EndCutscene();
            }
        }
    }

    /// <summary>
    /// Skips to the next timestamp in the cutscene. If on the last frame, ends the cutscene.
    /// </summary>
    public void SkipToNextTimestamp()
    {
        if (string.IsNullOrEmpty(stateName)) return;
        if (isOnLastFrame)
        {
            EndCutscene();
            return;
        }

        AnimatorStateInfo stateInfo = cutsceneAnimator.GetCurrentAnimatorStateInfo(0);
        float currentTime = (stateInfo.normalizedTime % 1f) * clipLength;

        int nextIndex = -1;

        for (int i = 0; i < frameTimestamps.Length; i++)
        {
            if (frameTimestamps[i] > currentTime + 0.5f) 
            {
                nextIndex = i;
                break;
            }
        }

        if (nextIndex != -1)
        {
            float targetNormalized = frameTimestamps[nextIndex] / clipLength;
            cutsceneAnimator.Play(stateName, 0, targetNormalized);
            cutsceneAnimator.speed = 1f; 
            cutsceneAnimator.Update(0f); 

            if (nextIndex == frameTimestamps.Length - 1)
            {
                isOnLastFrame = true;
                endTimer = 0f;
            }
        }
        else
        {
            EndCutscene();
        }
    }

    /// <summary>
    /// Ends the cutscene, hides the cutscene animator, unfreezes the player, and invokes the onComplete action.
    /// </summary>
    public void EndCutscene()
    {
        if (introOverlay != null && introOverlay.activeSelf) introOverlay.SetActive(false);
        if (!active) return;
        active = false;
        isOnLastFrame = false; 
        endTimer = 0f;
        stateName = null; 
        if (cutsceneAnimator.gameObject.activeSelf) cutsceneAnimator.gameObject.SetActive(false);
        if (DialogueManager.Instance.OngoingDialogue()) DialogueManager.Instance.EndDialogue();
        GameManager.Instance.FreezePlayer(false);
        skipButton.SetActive(false);
        onComplete?.Invoke();
    }
}