using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject playMenuButtons;
    [SerializeField] private string downHillScene;
    [SerializeField] private string flatScene;

    public void Play()
    {
        mainMenuButtons.SetActive(false);
        playMenuButtons.SetActive(true);
    }

    public void PlayGameDownhill()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(downHillScene); // Change to whatever our play scene gonna be
    }

    public void PlayGameFlat()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(flatScene); // Change to whatever our play scene gonna be
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
