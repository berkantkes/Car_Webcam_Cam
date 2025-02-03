using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{
    [SerializeField] private PhotoGallery _photoGallery;
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private TimeController _timeController;
    [SerializeField] private GameObject _inGamePanel;

    private void Awake()
    {
        _mainMenu.Initialize();
        _photoGallery.Initialize(_mainMenu.GetStartPopUpController(), _timeController);
    }
    
    private void OnEnable()
    {
        EventManager.Subscribe(GameEvents.OnStartGame, OpenPhotoGalleryPopup);
        EventManager.Subscribe(GameEvents.OnFinishGame, OpenPhotoGalleryPopup);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEvents.OnStartGame, OpenPhotoGalleryPopup);
        EventManager.Unsubscribe(GameEvents.OnFinishGame, OpenPhotoGalleryPopup);
    }
    private void OpenPhotoGalleryPopup()
    {
        _photoGallery.gameObject.SetActive(true);
        _inGamePanel.SetActive(false);
    }
    private void OpenInGamePanel()
    {
        _inGamePanel.SetActive(true);
    }
}
