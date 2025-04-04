using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{

    [SerializeField] private GameObject pauseCanvas;

    void Start()
    {
        pauseCanvas.SetActive(false);
    }

    public void ResumeGame()
    {
        SetUIStatus(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("AlphaScene");
        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SetUIStatus(bool status)
    {
        pauseCanvas.SetActive(status);

        if (status)
        {
            Time.timeScale = 0f; 
        }
        else
        {
            Time.timeScale = 1f;  
        }
    }
}
