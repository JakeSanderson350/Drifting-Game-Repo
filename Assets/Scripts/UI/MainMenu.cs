using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("AlphaScene"); // Change to whatever our play scene gonna be
    }

    public void QuitGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu"); // Change to whatever our play scene gonna be
    }

    // Quit the game
    public void QuitApplication()
    {
        Time.timeScale = 1.0f;
        Application.Quit();
    }
}
