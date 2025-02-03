using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartPopUpController : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private TMP_InputField _usernameInputField;

    private string _username;
    public string Username => _username;
    
    public void Initialize()
    {
        _closeButton.onClick.AddListener(CloseStartPopUp);
        _startButton.onClick.AddListener(StartGame);

        _usernameInputField.characterLimit = 15;

        _usernameInputField.onValueChanged.AddListener(OnUsernameValueChanged);

        CloseStartPopUp();
        _startButton.interactable = false;
    }

    private void OnUsernameValueChanged(string username)
    {
        _startButton.interactable = !string.IsNullOrWhiteSpace(username);
    }

    private void CloseStartPopUp()
    {
        gameObject.SetActive(false);
    }

    private void StartGame()
    {
        _username = _usernameInputField.text.Trim();
        EventManager.Execute(GameEvents.OnStartGame);
        Debug.Log("StartGame, username: " + _username);
    }
}