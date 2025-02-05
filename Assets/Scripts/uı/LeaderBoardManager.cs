using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private List<SingleLeaderboardMemberController> leaderboardEntries;
    [SerializeField] private Button _restartButton;
    [SerializeField] private GameObject _mainMenu;

    public void Initialize()
    {
        LoadLeaderboard();
    }

    private void OnEnable()
    {
        _restartButton.onClick.AddListener(RestartGame);
    }

    private void OnDisable()
    {
        _restartButton.onClick.RemoveListener(RestartGame);
    }

    private void RestartGame()
    {
        EventManager.Execute(GameEvents.OnRestartGame);
        _mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    private void LoadLeaderboard()
    {
        RaceResultManager raceResultManager = FindObjectOfType<RaceResultManager>();
        if (raceResultManager == null)
        {
            Debug.LogError("RaceResultManager bulunamadı!");
            return;
        }

        List<RaceResult> top3Results = raceResultManager.GetTop3RaceResults();

        for (int i = 0; i < leaderboardEntries.Count; i++)
        {
            if (i < top3Results.Count)
            {
                RaceResult result = top3Results[i];
                Sprite raceSprite = raceResultManager.LoadSpriteFromFile(Path.Combine(Application.streamingAssetsPath, "RaceResults", result.SpritePath));

                leaderboardEntries[i].Initialize(raceSprite, result.Username, result.RaceTime);
            }
            else
            {
                leaderboardEntries[i].gameObject.SetActive(false);
            }
        }
    }
}