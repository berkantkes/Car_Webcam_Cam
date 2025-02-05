using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleLeaderboardMemberController : MonoBehaviour
{
    [SerializeField] private Image _raceResultImage;
    [SerializeField] private TMP_Text _usernameText;
    [SerializeField] private TMP_Text _raceTimeText;

    public void Initialize(Sprite raceResultImage, string username, float raceTime)
    {
        _raceResultImage.sprite = raceResultImage;
        _usernameText.SetText(username);
        
        float elapsedTime = raceTime;
        
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int fraction = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

        _raceTimeText.SetText("Time : " + string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction));
    }
}
