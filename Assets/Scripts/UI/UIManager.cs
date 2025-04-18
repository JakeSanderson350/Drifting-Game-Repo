using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject overallCanvas;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private Slider difficultySlider;

    public static Action togglePause;
    void Start()
    {
        overallCanvas = this.gameObject;
        pauseMenu = this.transform.Find("Pause").gameObject;
        optionsMenu = this.transform.Find("Options").gameObject;

        optionsMenu.SetActive(false);
        overallCanvas.SetActive(false);

        difficultySlider.value = GameManagerED.Instance.GetDifficulty();
        GameManagerED.Instance.SetDifficulty(difficultySlider.value);

        AudioListener.volume = 1.0f;
    }

    public void ResumeGame()
    {
        togglePause.Invoke();
    }

    public void RestartGame()
    {
        AudioListener.volume = 1.0f;
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
