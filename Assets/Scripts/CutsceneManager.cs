using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles all universal cutscene logic and individual cutscenes
/// </summary>
public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    [SerializeField] InputActionReference continueAction;
    [SerializeField] Animator cutsceneAnimator;
    private float[] frameTimestamps;
    private float clipLength;
    private string stateName;
    private int currentFrame;
    private bool active;
    private System.Action onComplete;
    private static float[] IntroTimestamps = { 0f, 5f, 10f, 15f, 20f, 25f, 30f, 35f, 40f, 45f };
    private const float IntroClipLength = 45f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable() => continueAction.action.performed += ContinueCutscene;
    private void OnDisable() => continueAction.action.performed -= ContinueCutscene;

    /// <summary>
    /// Starts the intro cutscene.
    /// </summary>
    /// <param name="onComplete">action to execute when the cutscene completes</param>
    public void PlayIntroCutscene(System.Action onComplete = null)
    {
        StartCutscene("intro_cutscene", IntroTimestamps, IntroClipLength, () =>
        {
            GameManager.Instance.player.progression++;
            GameManager.Instance.player.SavePlayer();
            onComplete?.Invoke();
        });
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

        cutsceneAnimator.gameObject.SetActive(true);
        cutsceneAnimator.Play(stateName, 0, 0f);
        cutsceneAnimator.speed = 0f;

        GameManager.Instance.FreezePlayer(true);
    }

    /// <summary>
    /// Listens for the continue action to advance the cutscene.
    /// </summary>
    /// <param name="context">Check if action was pressed</param>
    void ContinueCutscene(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame() && active)
            DisplayNextFrame();
    }

    /// <summary>
    /// Goes to the next frame, or ends the cutscene if on the last frame.
    /// </summary>
    public void DisplayNextFrame()
    {
        currentFrame++;
        if (currentFrame >= frameTimestamps.Length)
        {
            EndCutscene();
            return;
        }
        float normalized = frameTimestamps[currentFrame] / clipLength;
        cutsceneAnimator.Play(stateName, 0, normalized);
        cutsceneAnimator.speed = 0f;
    }

    /// <summary>
    /// Ends the cutscene and fires onComplete which triggers the Scene Loaded
    /// transition in GameManager, fading out into the world.
    /// </summary>
    public void EndCutscene()
    {
        active = false;
        cutsceneAnimator.gameObject.SetActive(false);
        GameManager.Instance.FreezePlayer(false);
        onComplete?.Invoke();
    }
}