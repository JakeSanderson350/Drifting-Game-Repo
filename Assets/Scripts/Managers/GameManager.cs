using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
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
                Debug.LogError($"There should only ever be one instance of {nameof(GameManager)}!");
            }
        }
    }
    private static GameManager singleton;

    [SerializeField] private float difficultyIncrementTime = 5.0f;
    public int difficulty;

    [SerializeField] private GameObject gameOverScreen;
    private bool gameOver = false;

    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private ScoreManager scoreManager;

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
        CarState.onPauseGame += PauseGame;
    }

    private void OnDisable()
    {
        CarState.onCarDeath -= GameOver;
        CarState.onPauseGame -= PauseGame;
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
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
        gameOver = true;
    }
    private void PauseGame()
    {
        pauseScreen.GetComponent<UIManager>().SetUIStatus(true);
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

    public int GetGameScore()
    {
        return scoreManager.GetScore();
    }
}
