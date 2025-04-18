using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("BetaBuild"); // Change to whatever our play scene gonna be
    }

    public void QuitGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu"); // Change to whatever our play scene gonna be
    }

    public void QuitApplication()
    {
        Time.timeScale = 1.0f;
        Application.Quit();
    }
}
