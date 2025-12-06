using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Handles all universal game logic and game state of the scene
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] public Animator transition;
    [HideInInspector] public Player player;
    
    [SerializeField] [Tooltip("Reference to the loadout UI canvas that displays equipment selection")] private GameObject loadoutCanvas; 

    /// <summary>
    /// Sets the loadout manager to active.
    /// </summary>
    public void ToggleLoadoutManager(bool isOpen)
    {
        FreezePlayer(isOpen);
        loadoutCanvas.SetActive(isOpen);
    }


    [Header("World Settings")]
    [SerializeField] private MusicType currentSong;
    public MusicType CurrentSong => currentSong;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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
        transition.SetTrigger(scene);
        
        yield return null;
        
        float animLength = transition.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);

        SceneManager.LoadScene(scene);
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
        LoadScene(GetCurrentSceneName());
    }
    public void BossDefeated(string nextSceneName)
    {
        LoadScene(nextSceneName);
    }

}
