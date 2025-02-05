using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private List<SingleLeaderboardMemberController> leaderboardEntries;

    public void Initialize()
    {
        LoadLeaderboard();
    }

    public void LoadLeaderboard()
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