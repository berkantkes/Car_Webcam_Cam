using UnityEngine;
using UnityEngine.UI;

public class SettingsPopUpController : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _soundButton;
    [SerializeField] private GameObject _offSlider;
    [SerializeField] private GameObject _onSlider;

    private bool _soundStatus = true;

    public void Initialize()
    {
        _closeButton.onClick.AddListener(CloseSettingsPopUp);
        _soundButton.onClick.AddListener(ToggleSound);

        UpdateSoundUI();
        CloseSettingsPopUp();
    }

    private void ToggleSound()
    {
        _soundStatus = !_soundStatus;

        UpdateSoundUI();
    }

    private void UpdateSoundUI()
    {
        _onSlider.SetActive(_soundStatus);
        _offSlider.SetActive(!_soundStatus);
    }

    private void CloseSettingsPopUp()
    {
        gameObject.SetActive(false);
    }
}