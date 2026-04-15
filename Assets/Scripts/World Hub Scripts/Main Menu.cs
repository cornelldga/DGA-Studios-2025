using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Load the Saloon scene
    /// </summary>
    public void LoadSaloon()
    {
        SceneManager.LoadScene("Saloon");
    }

    /// <summary>
    /// Closes the game by quitting the application
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

}
