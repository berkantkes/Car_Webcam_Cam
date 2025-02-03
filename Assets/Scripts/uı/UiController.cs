using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UiController : MonoBehaviour
{
    [FormerlySerializedAs("_photoGallery")] [SerializeField] private FinishCanvasManager finishCanvasManager;
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private TimeController _timeController;
    [SerializeField] private GameObject _inGamePanel;
    [SerializeField] private TakePhotoManager _takePhotoManager;

    private void Awake()
    {
        _mainMenu.Initialize();
        finishCanvasManager.Initialize(_mainMenu.GetStartPopUpController(), _timeController);
    }
    
    private void OnEnable()
    {
        EventManager.Subscribe(GameEvents.OnStartGame, OpenInGamePanel);
        EventManager.Subscribe(GameEvents.OnFinishGame, OpenPhotoGalleryPopup);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEvents.OnStartGame, OpenInGamePanel);
        EventManager.Unsubscribe(GameEvents.OnFinishGame, OpenPhotoGalleryPopup);
    }
    private void OpenPhotoGalleryPopup()
    {
        finishCanvasManager.gameObject.SetActive(true);
        _inGamePanel.SetActive(false);
    }
    private void OpenInGamePanel()
    {
        _inGamePanel.SetActive(true);
        _takePhotoManager.Initialize();
    }
}
