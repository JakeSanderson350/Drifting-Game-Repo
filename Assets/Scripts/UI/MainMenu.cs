using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayGame()
    {
        SceneManager.LoadScene("AlphaScene"); // Change to whatever our play scene gonna be
    }

    // Quit the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
