using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreUI;
    [SerializeField] private CarDriftController car;

    private float score = 0.0f;
    private float driftLength = 0.0f;
    private float scoreMultiplier = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0.0f;
    }

    // Update is called once per frame
    void Update()
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

            score += (Mathf.Abs(car.GetDriftAngle()) * scoreMultiplier) / 10;
        }
        else
        {
            StartCoroutine(StopDriftStreak());
        }

        scoreUI.text = "Score: " + (int)score + "\nMultiplier: " + scoreMultiplier;
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
}
