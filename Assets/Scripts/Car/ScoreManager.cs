using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreUI;
    [SerializeField] private CarDriftController car;
    [SerializeField] private ParticleManager particleManager;

    private float score = 0.0f;
    private float driftLength = 0.0f;
    private float scoreMultiplier = 1.0f;
    private Color textColor = Color.white;
    private bool isAlive;

    public float donutTimeLimit = 15.0f;
    private float screenUse = 0.8f;
    private float prevInput = 0.0f;
    private float timeTurning = 0.0f;

    private void OnEnable()
    {
        CarState.onCarDeath += GameOver;
    }

    private void OnDisable()
    {
        CarState.onCarDeath -= GameOver;
    }

    void Start()
    {
        score = 0.0f;
        isAlive = true;
    }

    void Update()
    {
        if (isAlive)
        {
            if (car.IsDrifting())
            {
                driftLength += Time.deltaTime;

                if (driftLength > 5.0f)
                {
                    scoreMultiplier = 2.0f;
                }
                if (driftLength > 10.0f)
                {
                    scoreMultiplier = 3.0f;
                }
                if (driftLength > 15.0f)
                {
                    scoreMultiplier = 4.0f;
                }
                if (driftLength > 20.0f)
                {
                    scoreMultiplier = 5.0f;
                }

                IsDonut();

                score += (Mathf.Abs(car.GetDriftAngle()) * scoreMultiplier) / 10;
            }
            else
            {
                StartCoroutine(StopDriftStreak());
            }
            if (particleManager != null)
            {
                scoreUI.color = textColor;
                textColor = particleManager.GetCurrentColor();
            }

            scoreUI.text = "Score: " + (int)score + "\nMultiplier: " + scoreMultiplier;
        }
    }

    private void IsDonut()
    {
        float input = Mathf.Clamp(TouchInput.centeredScreenPosition.x / screenUse, -1, 1);

        if ((input > 0 && prevInput > 0) || (input < 0 && prevInput < 0))
        {
            timeTurning += Time.deltaTime;

            if (timeTurning > donutTimeLimit)
            {
                scoreMultiplier = -10.0f;
            }
        }
        else
        {
            timeTurning = 0;
        }

        prevInput = input;
    }

    IEnumerator StopDriftStreak()
    {
        yield return new WaitForSeconds(2.0f);

        if (!car.IsDrifting())
        {
            driftLength = 0.0f;
            scoreMultiplier = 1.0f;
        }
    }

    float GetDriftLength() { return driftLength; }

    private void GameOver()
    {
        isAlive = false;
    }

    public int GetScore()
    {
        return (int)score;
    }
}
