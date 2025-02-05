using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UiController : MonoBehaviour
{
    [SerializeField] private FinishGameCanvasManager _finishGameCanvasManager;
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private InGamePanelController _inGamePanelController;

    private TimeController _timeController;
    public void Initialize(TimeController timeController)
    {
        _timeController = timeController;
        
        _mainMenu.Initialize();
    }
    
    private void OnEnable()
    {
        EventManager.Subscribe(GameEvents.OnStartGame, OpenInGamePanel);
        EventManager.Subscribe(GameEvents.OnFinishGame, OpenFinishGameCanvas);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEvents.OnStartGame, OpenInGamePanel);
        EventManager.Unsubscribe(GameEvents.OnFinishGame, OpenFinishGameCanvas);
    }
    private void OpenInGamePanel()
    {
        _inGamePanelController.gameObject.SetActive(true);
    }
    private void OpenFinishGameCanvas()
    {
        _finishGameCanvasManager.gameObject.SetActive(true);
        _finishGameCanvasManager.Initialize(_mainMenu.GetStartPopUpController(), _timeController);
        _inGamePanelController.gameObject.SetActive(false);
    }
    
    public void FinishGame()
    {
        _finishGameCanvasManager.FinishGame();
    }

    public void RestartGame()
    {
        _mainMenu.RestartGame();
        _finishGameCanvasManager.RestartGame();
    }

    public TMP_Text GetTimerText()
    {
        return _inGamePanelController.GetTimerText();
    }
}
