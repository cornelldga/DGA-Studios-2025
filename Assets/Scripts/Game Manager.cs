using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using System.Collections;

/// <summary>
/// Handles all universal game logic and game state of the scene
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("References")]
    [Tooltip("Reference to the LoadoutManager.")]
    [SerializeField] private LoadoutManager loadoutManager;
    public LoadoutManager GetLoadoutManager => loadoutManager;
    [Tooltip("Reference to the DialogueManager.")]
    [SerializeField] private DialogueManager dialogueManager;
    public DialogueManager GetDialogueManager => dialogueManager;
    [Tooltip("Reference to the CutsceneManager.")]
    [SerializeField] private CutsceneManager cutsceneManager;
    public CutsceneManager GetCutsceneManager => cutsceneManager;

    [Space(10)]
    [Header("Pause Menu UI")]
    [Tooltip("Reference to the pause button")]
    [SerializeField] private GameObject pauseButton;
    [Tooltip("Reference to the pause menu")]
    [SerializeField] private GameObject pauseMenu;
    [Tooltip("Reference to the volume sliders panel")]
    [SerializeField] private GameObject sliders;
    [SerializeField] private Animator pauseMenuAnimator;

    [Space(5)]
    [Header("Audio Controls")]
    [Tooltip("Slider for controlling music volume")]
    [SerializeField] private Slider musicSlider;
    [Tooltip("Slider for controlling SFX volume")]
    [SerializeField] private Slider sfxSlider;

    [Space(10)]
    [Header("Transitions")]
    [Tooltip("Animator for scene transitions")]
    [SerializeField] private Animator transitions;

    [Header("World Settings")]
    [Tooltip("The background music for the current scene")]
    [SerializeField] private MusicType currentSong;

    [SerializeField] private int progression;
    [SerializeField] bool setProgression;

    private bool volumeOpened = false;
    private Player playerInstance;

    /// <summary>
    /// Public access to the current player instance. 
    /// Set by the Player script in its Start or Awake method.
    /// </summary>
    public Player player 
    { 
        get => playerInstance; 
        set => playerInstance = value; 
    }

    public MusicType CurrentSong => currentSong;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            if (setProgression)
            {
                Debug.Log("Progression Set to " + setProgression.ToString());
                PlayerPrefs.SetInt("progression", progression);
            }
            pauseMenu.SetActive(false);
        }
        else
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Fixed Game Manager bug of multiple instances.
    /// </summary>
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this) Instance = null;
    }

    /// <summary>
    /// Initializes volume sliders on the AudioManager.
    /// </summary>
    private void Start()
    {
        AudioManager.Instance?.InitVolumeSliders(musicSlider, sfxSlider);
    }

    private void Update()
    {
        HandlePauseInput();
        UpdatePauseButtonVisibility();
    }

    /// <summary>
    /// Listens for the Escape key to toggle the pause menu or close the volume panel if it's open.
    /// </summary>
    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (loadoutManager == null || !loadoutManager.gameObject.activeSelf) && GetCurrentSceneName() != "Main Menu")
        {
            if (pauseMenu.activeSelf && volumeOpened)
            {
                CloseVolumePanel();
            }
            else
            {
                TogglePauseMenu(!pauseMenu.activeSelf);
            }
        }
    }

    /// <summary>
    /// Toggles visibility of the pause button based on scene and player presence.
    /// </summary>
    private void UpdatePauseButtonVisibility()
    {
        bool isMainMenu = GetCurrentSceneName() == "Main Menu";
        pauseButton.SetActive(!isMainMenu && playerInstance != null);
    }

    /// <summary>
    /// Toggles the pause menu and freezes/unfreezes the player accordingly. If closing the pause menu while the volume panel is open, it will also close the volume panel.
    /// </summary>
    /// <param name="isActive"></param>
    public void TogglePauseMenu(bool isActive)
    {
        if (!isActive && volumeOpened)
            CloseVolumePanel();

        if (isActive)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            pauseMenuAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            pauseMenuAnimator.Play("Music On", 0, 0f);
            if (musicSlider != null)
            {
                onMusicVolumeChanged(musicSlider.value);
            }
        }
        else
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }

        FreezePlayer(isActive);
    }

    /// <summary>
    /// Closes the pause menu and unfreezes the player.
    /// </summary>
    public void onResumeClicked()
    {
        TogglePauseMenu(false);
    }

    /// <summary>
    /// Closes the pause menu, unfreezes the player, and loads the main menu scene.
    /// </summary>
    public void onExitClicked()
    {
        if (dialogueManager.OngoingDialogue()) dialogueManager.EndDialogue();
        TogglePauseMenu(false);
        LoadScene("Main Menu");
    }

    public void onBackClicked() => CloseVolumePanel();

    public void onVolumeClicked()
    {
        pauseMenu.transform.Find("Resume").gameObject.SetActive(false);
        pauseMenu.transform.Find("Quit").gameObject.SetActive(false);
        pauseMenu.transform.Find("Volume").gameObject.SetActive(false);
        pauseMenu.transform.Find("Back").gameObject.SetActive(true);
        sliders.SetActive(true);
        volumeOpened = true;
    }

    /// <summary>
    /// Mutes the music.
    /// </summary>
    public void onMuteMusicClicked()
    {
        if (musicSlider != null)
        {
            musicSlider.value = musicSlider.minValue;
        }
    }

    public void onMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }

        if (pauseMenuAnimator != null)
        {
            pauseMenuAnimator.speed = value <= 0.001f ? 0 : 1;
        }
    }

    /// <summary>
    /// Mutes the SFX.
    /// </summary>
    public void onMuteSFXClicked()
    {
        AudioManager.Instance.SetSFXVolume(0);
        sfxSlider.value = 0;
    }

    /// <summary>
    /// Closes the volume panel and shows the main pause menu options. Called when the Back button in the volume panel is clicked or when the Escape key is pressed while the volume panel is open.
    /// </summary>
    private void CloseVolumePanel()
    {
        sliders.SetActive(false);
        pauseMenu.transform.Find("Resume").gameObject.SetActive(true);
        pauseMenu.transform.Find("Quit").gameObject.SetActive(true);
        pauseMenu.transform.Find("Volume").gameObject.SetActive(true);
        pauseMenu.transform.Find("Back").gameObject.SetActive(false);
        volumeOpened = false;
    }

    /// <summary>
    /// Check if on pause button
    /// </summary>
    public bool PointerOnPause()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            pauseButton.GetComponent<RectTransform>(), 
            Input.mousePosition);
    }

    /// <summary>
    /// Sets the loadout manager to active.
    /// </summary>
    /// <param name="isOpen">If the loadout manager should be open</param>
    public void ToggleLoadoutManager(bool isOpen)
    {
        FreezePlayer(isOpen);
        loadoutManager.gameObject.SetActive(isOpen);
    }

    /// <summary>
    /// Gets the currently equipped bases and mixers from the loadout.
    /// </summary>
    /// <param name="equippedBases">Array to store equipped bases</param>
    /// <param name="equippedMixers">Array to store equipped mixers</param>
    public void GetLoadout(ref BaseType[] equippedBases, ref MixerType[] equippedMixers)
    {
        equippedBases = loadoutManager.GetEquippedBases();
        equippedMixers = loadoutManager.GetEquippedMixers();
    }

    /// <summary>
    /// Load the given scene name and does the transition animation dependent on the scene.
    /// </summary>
    /// <param name="sceneName">Name of scene to transition to</param>
    public void LoadScene(string sceneName)
    {
        string transitionTrigger = (GetCurrentSceneName() == "Saloon" && sceneName == "World Hub") ? "Saloon Exit" : sceneName;
        StartCoroutine(TransitionAnim(transitionTrigger));
    }

    /// <summary>
    /// Animation dependent on the scene.
    /// </summary>
    /// <param name="scene">Name of the trigger animation</param>
    IEnumerator TransitionAnim(string scene)
    {
        FreezePlayer(true);

        bool hasSpecificTransition = false;
        foreach (var param in transitions.parameters)
        {
            if (param.name == scene || scene == "Saloon Exit")
            {
                hasSpecificTransition = true;
                break;
            }
        }

        transitions.SetTrigger(hasSpecificTransition ? scene : "World Hub");
        
        yield return null;
        float animLength = transitions.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);

        string targetScene = (scene == "Saloon Exit") ? "World Hub" : scene;
        SceneManager.LoadScene(targetScene);
    }

    /// <summary>
    /// Called when a new scene is loaded. Triggers the animation.
    /// </summary>
    /// <param name="scene">The loaded scene</param>
    /// <param name="mode">The scene load mode</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (transitions != null)
            transitions.SetTrigger("Scene Loaded");

    }

    /// <summary>
    /// Checks for cutscenes to play.
    /// </summary>
    public void CheckForCutscenes()
    {
        string currentScene = GetCurrentSceneName();

        if (currentScene == "World Hub" && PlayerPrefs.GetInt("progression", 0)==0)
        {
            CutsceneManager.Instance.PlayBackstoryCutscene(() => transitions.SetTrigger("Scene Loaded"));
        }

        else if (currentScene == "Saloon" && PlayerPrefs.GetInt("progression", 0)==0)
        {
            CutsceneManager.Instance.PlayMeetBobbyCutscene();
        }
    }

    /// <summary>
    /// Load the given scene name
    /// </summary>
    /// <returns>The name of the current active scene</returns>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Toggles the player controller to freeze/unfreeze the player
    /// </summary>
    /// <param name="freeze">Whether to freeze the player</param>
    public void FreezePlayer(bool freeze)
    {
        if (playerInstance == null) return;
        if (freeze) playerInstance.StopPlayer();
        playerInstance.enabled = !freeze;
    }

    public void LoseGame()
    {
        FreezePlayer(true);
        LoadScene(GetCurrentSceneName());
    }

}
