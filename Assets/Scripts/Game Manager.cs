using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Handles all universal game logic and game state of the scene
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] LoadoutManager loadoutManager;
    [SerializeField] Animator animator;
    [HideInInspector] public Player player;


    /// <summary>
    /// Sets the loadout manager to active.
    /// </summary>
    public void ToggleLoadoutManager(bool isOpen)
    {
        FreezePlayer(isOpen);
        loadoutManager.gameObject.SetActive(isOpen);
    }


    [Header("World Settings")]
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

    public void GetLoadout(ref BaseType[] equippedBases, ref MixerType[] equippedMixers)
    {
        equippedBases = loadoutManager.GetEquippedBases();
        equippedMixers = loadoutManager.GetEquippedMixers();
    }

    /// <summary>
    /// Load the given scene name and does the transition animation dependent on the scene.
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionAnim(sceneName));
    }
    /// <summary>
    /// Animation dependent on the scene.
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    IEnumerator TransitionAnim(string scene)
    {
        FreezePlayer(true);
        animator.SetTrigger(scene);
        
        yield return null;
        
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);
        animator.ResetTrigger("Scene Loaded");
        SceneManager.LoadScene(scene);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        animator.SetTrigger("Scene Loaded");
    }


    /// <summary>
    /// Load the given scene name
    /// </summary>
    /// <param name="sceneName"></param>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Toggles the player controller to freeze/unfreeze the player
    /// </summary>
    /// <param name="freeze"></param>
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
        StartCoroutine(ReloadAfterDelay(1));
    }
    private IEnumerator ReloadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadScene(GetCurrentSceneName());
    }
    public void BossDefeated(string nextSceneName)
    {
        LoadScene(nextSceneName);
    }

}
