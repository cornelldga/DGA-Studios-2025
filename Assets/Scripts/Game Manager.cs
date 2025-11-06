using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles all universal game logic and game state of the scene
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public PlayerController player;

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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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
    /// Toggles the player controller to freeze/unfreeze the player
    /// </summary>
    /// <param name="freeze"></param>
    public void FreezePlayer(bool freeze)
    {
        player.enabled = !freeze;
    }

    /// <summary>
    /// Makes the player lose the game and initiates a Lose Game Screen
    /// </summary>
    public void LoseGame()
    {
        Debug.Log("Lose Game");
    }

}
