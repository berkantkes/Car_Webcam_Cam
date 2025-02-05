using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FinishGameCanvasManager : MonoBehaviour
{
    [SerializeField] private UserRaceResultPanelManager _userRaceResultPanelManager;
    [SerializeField] private LeaderboardManager _leaderboardManager;
    [SerializeField] private RaceResultRenderManager _raceResultRenderManager;
    [SerializeField] private RaceResultManager _raceResultManager;

    public void Initialize(StartPopUpController startPopUpController, TimeController timeController)
    {
        _userRaceResultPanelManager.Initialize(startPopUpController, timeController, _raceResultRenderManager);
        _raceResultRenderManager.Initialize(_raceResultManager);
        _raceResultManager.Initialize();
    }
    
    public async void FinishGame()
    {
        _userRaceResultPanelManager.gameObject.SetActive(true);
        _userRaceResultPanelManager.LoadPhotos();
        
        await UniTask.WaitForSeconds(5);
        _leaderboardManager.gameObject.SetActive(true);
        _leaderboardManager.Initialize();
        _userRaceResultPanelManager.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        _leaderboardManager.gameObject.SetActive(false);
        _raceResultRenderManager.ResetRenderCamera();
    }
}
