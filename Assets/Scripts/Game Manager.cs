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
    [SerializeField] private DialogueManager dialogueManager;
    [Header("Pause Menu")]
    [Tooltip("Reference to the pause button")]
    [SerializeField] private GameObject pauseButton;
    [Tooltip("Reference to the pause menu")]
    [SerializeField] private GameObject pauseMenu;
    [Tooltip("Reference to the volume sliders panel")]
    [SerializeField] private GameObject sliders;
    [Tooltip("Slider for controlling music volume")]
    [SerializeField] private Slider musicSlider;
    [Tooltip("Slider for controlling SFX volume")]
    [SerializeField] private Slider sfxSlider;
    private bool volumeOpened = false;
    
    [Tooltip("Animator for scene transitions")]
    [SerializeField] private Animator animator;
    
    [HideInInspector] public Player player;

    [Header("World Settings")]
    [Tooltip("The background music for the current scene")]
    [SerializeField] private MusicType currentSong;
    
    public MusicType CurrentSong => currentSong;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            SceneManager.sceneLoaded += OnSceneLoaded;
            pauseMenu.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes volume sliders on the AudioManager.
    /// </summary>
    private void Start()
    {
        AudioManager.Instance?.InitVolumeSliders(musicSlider, sfxSlider);
    }

    /// <summary>
    /// Listens for the Escape key to toggle the pause menu or close the volume panel if it's open.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && loadoutManager.gameObject.activeSelf == false && dialogueManager.OngoingDialogue() == false && GetCurrentSceneName() != "Main Menu")
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

        if (GetCurrentSceneName() != "Main Menu" && player != null)
        {
            pauseButton.SetActive(true);
        }
        else
        {
            pauseButton.SetActive(false);
        }
    }

    /// <summary>
    /// Toggles the pause menu and freezes/unfreezes the player accordingly. If closing the pause menu while the volume panel is open, it will also close the volume panel.
    /// </summary>
    /// <param name="isActive"></param>
    public void TogglePauseMenu(bool isActive)
    {
        if (!isActive && volumeOpened)
            CloseVolumePanel();

        pauseMenu.SetActive(isActive);
        Time.timeScale = isActive ? 0 : 1;
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
        TogglePauseMenu(false);
        LoadScene("Main Menu");
    }

    /// <summary>
    /// Closes the volume panel and shows the main pause menu options.
    /// </summary>
    public void onBackClicked()
    {
        sliders.SetActive(false);
        pauseMenu.transform.Find("Back").gameObject.SetActive(false);
        pauseMenu.transform.Find("Resume").gameObject.SetActive(true);
        pauseMenu.transform.Find("Volume").gameObject.SetActive(true);
        pauseMenu.transform.Find("Quit").gameObject.SetActive(true);
        volumeOpened = false;
    }

    /// <summary>
    /// Hides the main pause menu options and shows the volume sliders.
    /// </summary>
    public void onVolumeClicked()
    {
        // Hide Resume/Quit, show sliders 
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
        AudioManager.Instance.SetMusicVolume(0);
        musicSlider.value = 0;
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
        if (GetCurrentSceneName() == "Saloon")
        {
            StartCoroutine(TransitionAnim("Saloon Exit"));
        }
        else
        {
            StartCoroutine(TransitionAnim(sceneName));
        }
    }

    /// <summary>
    /// Animation dependent on the scene.
    /// </summary>
    /// <param name="scene">Name of the trigger animation</param>
    IEnumerator TransitionAnim(string scene)
    {
        FreezePlayer(true);

        // If scene doesnt have a transition animation, use World Hub transition
        for (int triggerIndex = 0; triggerIndex < animator.parameterCount; triggerIndex++)
        {
            if (animator.parameters[triggerIndex].name == scene || (scene == "Saloon Exit"))
            {
                animator.SetTrigger(scene);
            }
            else
            {
                animator.SetTrigger("World Hub");
            }
        }
        
        yield return null;
        
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);
        animator.ResetTrigger("Scene Loaded");

        if (scene == "Saloon Exit")
        {
            SceneManager.LoadScene("World Hub");
        }
        else
        {
            SceneManager.LoadScene(scene);
        }
    }

    /// <summary>
    /// Called when a new scene is loaded. Triggers the animation.
    /// </summary>
    /// <param name="scene">The loaded scene</param>
    /// <param name="mode">The scene load mode</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        animator.SetTrigger("Scene Loaded");
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
        if (freeze)
        {
            player.StopPlayer();
        }
        player.enabled = !freeze;
    }

    /// <summary>
    /// Makes the player lose the game and initiates a Lose Game Screen
    /// </summary>
    public void LoseGame()
    {
        FreezePlayer(true);
        LoadScene(GetCurrentSceneName());
    }

    /// <summary>
    /// Called when a boss is defeated, loads the next scene.
    /// </summary>
    /// <param name="nextSceneName">Name of the next scene to load</param>
    public void BossDefeated(string nextSceneName)
    {
        LoadScene(nextSceneName);
    }

}
