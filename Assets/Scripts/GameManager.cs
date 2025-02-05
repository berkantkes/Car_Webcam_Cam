using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TimeController _timeController;
    [SerializeField] private SimpleCarController _simpleCarController;
    [SerializeField] private UiController _uiController;
    [SerializeField] private BannerManager _bannerManager;
    [SerializeField] private TakePhotoManager _takePhotoManager;
    [SerializeField] private SimpleCarController _carController;
    
    public void Awake()
    {
        _uiController.Initialize(_timeController);
        _simpleCarController.Initialize(_uiController);
        _takePhotoManager.Initialize(_bannerManager);
        _timeController.Initialize(_uiController);
    }
    
    private void OnEnable()
    {
        EventManager.Subscribe(GameEvents.OnRestartGame, RestartGame);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEvents.OnRestartGame, RestartGame);
    }

    private void RestartGame()
    {
        _uiController.RestartGame();
    }

}
