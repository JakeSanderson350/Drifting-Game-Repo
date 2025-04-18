using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerED : MonoBehaviour
{
    public static GameManagerED Instance
    {
        get => singleton;
        set
        {
            if (value == null)
            {
                singleton = null;
            }
            else if (singleton == null)
            {
                singleton = value;
            }
            else if (singleton != value)
            {
                Destroy(value);
                Debug.LogError($"There should only ever be one instance of {nameof(GameManagerED)}!");
            }
        }
    }
    private static GameManagerED singleton;

    [SerializeField] private float difficultyIncrementTime = 5.0f;
    public int difficulty;

    [SerializeField] private GameObject gameOverScreen;
    private bool gameOver = false;

    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private ScoreManager scoreManager;

    [SerializeField] private AudioSource engineSounds;
    [SerializeField] private bool isPaused = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnEnable()
    {
        CarState.onCarDeath += GameOver;
        Killzone.onCarDeath += GameOver;
        CarState.togglePause += TogglePause;
        UIManager.togglePause += TogglePause;
    }

    private void OnDisable()
    {
        CarState.onCarDeath -= GameOver;
        Killzone.onCarDeath -= GameOver;
        CarState.togglePause -= TogglePause;
        UIManager.togglePause -= TogglePause;
    }

    // Start is called before the first frame update
    void Start()
    {
        difficulty = 0;
        StartCoroutine(IncrementDifficulty());
        scoreManager = GetComponent<ScoreManager>();
    }

    private void GameOver()
    {
        engineSounds.Stop();
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.volume = 0f;
        gameOver = true;
    }


    public void TogglePause()
    {
        isPaused = !isPaused;
        UpdatePauseState();
    }

    public bool GetPauseStatus()
    {
        return isPaused;
    }

    private void UpdatePauseState()
    {
        pauseScreen.GetComponent<UIManager>().SetUIStatus(isPaused);
        if (isPaused)
        {
            engineSounds.Pause();
            AudioListener.volume = 0f;
            Time.timeScale = 0f; 
        }
        else
        {
            engineSounds.UnPause();
            AudioListener.volume = 1f;
            Time.timeScale = 1f; 
        }
    }

    private IEnumerator IncrementDifficulty()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(difficultyIncrementTime);

            difficulty++;
        }
    }

    public int GetDifficulty()
    {
        return difficulty;
    }
    public void SetDifficulty(float value)
    {
        difficulty = Mathf.RoundToInt(value);
        Debug.Log("Difficulty set to: " + difficulty);
    }

    public int GetGameScore()
    {
        return scoreManager.GetScore();
    }
}
