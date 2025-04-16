using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject overallCanvas;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;

    void Start()
    {
        overallCanvas = this.gameObject;
        pauseMenu = this.transform.Find("Pause").gameObject;
        optionsMenu = this.transform.Find("Options").gameObject;

        optionsMenu.SetActive(false);
        overallCanvas.SetActive(false);
    }

    public void ResumeGame()
    {
        SetUIStatus(false);
    }

    public void RestartGame()
    {
        overallCanvas.SetActive(false);
        SceneManager.LoadScene("RotationFix");
        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SetUIStatus(bool status)
    {
        overallCanvas.SetActive(status);

        if (status)
        {
            Time.timeScale = 0f; 
        }
        else
        {
            Time.timeScale = 1f;  
        }
    }
    
    public void OpenOptions()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void CloseOptions()

    {
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
}
