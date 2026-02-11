using UnityEngine;
using UnityEngine.SceneManagement;
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
        }
        else
        {
            Destroy(gameObject);
        }
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
            if (animator.parameters[triggerIndex].name == scene)
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
