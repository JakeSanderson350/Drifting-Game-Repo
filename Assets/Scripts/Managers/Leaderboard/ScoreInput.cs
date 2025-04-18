using UnityEngine;
using TMPro;

public class ScoreInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputName;
    private int inputScore;

    public void SubmitScore()
    {
        inputScore = GameManagerED.Instance.GetGameScore();

        string username = inputName.text;
        if (username.Length > 8)
        {
            username = username.Substring(0, 8);
        }

        LeaderboardManager.Instance.SetLeaderboardEntry(username, inputScore);
    }
}
