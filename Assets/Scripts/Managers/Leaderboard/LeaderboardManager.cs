using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using Dan.Main;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance
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
                Debug.LogError($"There should only ever be one instance of {nameof(LeaderboardManager)}!");
            }
        }
    }
    private static LeaderboardManager singleton;

    [SerializeField] private List<TextMeshProUGUI> names;
    [SerializeField] private List<TextMeshProUGUI> scores;

    private string publicLeaderboardKey = "68aa34b61f3e4ebad0d18b1e64d81eda69b803c87341994ad4a43b51ed4b5303";

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

    public void Start()
    {
        GetLeaderboard();
    }

    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((msg) =>
        {
            int loopLength = (msg.Length < names.Count) ? msg.Length : names.Count;

            for (int i = 0; i < loopLength; i++)
            {
                names[i].text = msg[i].Username;
                scores[i].text = msg[i].Score.ToString();
            }
        }
        ));
    }

    public void SetLeaderboardEntry(string _username, int _score)
    {
        //LeaderboardCreator.ResetPlayer();
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, _username, _score, ((msg) =>
        {
            GetLeaderboard();
            LeaderboardCreator.ResetPlayer();
        }
        ));
        
    }
}
