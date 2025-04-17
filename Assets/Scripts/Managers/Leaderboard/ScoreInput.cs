using UnityEngine;
using TMPro;

public class ScoreInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputName;
    private int inputScore;

    public void SubmitScore()
    {
        inputScore = GameManager.Instance.GetGameScore();

        LeaderboardManager.Instance.SetLeaderboardEntry(inputName.text, inputScore);
    }
}
