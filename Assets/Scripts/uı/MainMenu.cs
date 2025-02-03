using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private StartPopUpController _startPopUpController; 
    [SerializeField] private SettingsPopUpController _settingsPopUpController;
    [SerializeField] private PhotoGallery _photoGallery;

    private bool isSoundOn = true;

    public void Initialize()
    {
        gameObject.SetActive(true);
        _startButton.onClick.AddListener(OpenStartPopup);
        _settingsButton.onClick.AddListener(OpenSettingsPopup);
        _settingsButton.onClick.AddListener(OpenSettingsPopup);

        _startPopUpController.Initialize();
        _settingsPopUpController.Initialize();
    }

    private void OnEnable()
    {
        EventManager.Subscribe(GameEvents.OnStartGame, StartGame);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEvents.OnStartGame, StartGame);
    }

    private void StartGame()
    {
        gameObject.SetActive(false);
    }

    private void OpenStartPopup()
    {
        _startPopUpController.gameObject.SetActive(true);
    }

    private void OpenSettingsPopup()
    {
        _settingsPopUpController.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public StartPopUpController GetStartPopUpController()
    {
        return _startPopUpController;
    }
}