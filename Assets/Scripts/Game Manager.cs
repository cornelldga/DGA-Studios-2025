using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles all universal game logic and game state of the scene
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public Player player;

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
        AudioManager.Instance.PlayMusic(currentSong);
    }

    /// <summary>
    /// Load the given scene name
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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
        player.enabled = !freeze;
        if (freeze)
        {
            player.StopPlayer();
        }
    }

    /// <summary>
    /// Makes the player lose the game and initiates a Lose Game Screen
    /// </summary>
    public void LoseGame()
    {
        Debug.Log("Lose Game");
        FreezePlayer(true);
        LoadScene(GetCurrentSceneName());
    }
    public void BossDefeated(string nextSceneName)
    {
        LoadScene(nextSceneName);
    }

}
