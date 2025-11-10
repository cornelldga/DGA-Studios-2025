using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Load the World Hub scene
    /// </summary>
    public void LoadWorldHub()
    {
        SceneManager.LoadScene("World Hub");
    }

    /// <summary>
    /// Closes the game by quitting the application
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

}
