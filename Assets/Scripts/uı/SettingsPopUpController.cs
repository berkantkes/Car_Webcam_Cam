using UnityEngine;
using UnityEngine.UI;

public class SettingsPopUpController : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _soundButton;
    [SerializeField] private GameObject _offSlider;
    [SerializeField] private GameObject _onSlider;
    [SerializeField] private AudioListener _audioListener; // AudioListener dışarıdan atanacak

    private bool _soundStatus;
    private const string SoundPrefKey = "SoundStatus"; // PlayerPrefs anahtarı

    public void Initialize()
    {
        _closeButton.onClick.AddListener(CloseSettingsPopUp);
        _soundButton.onClick.AddListener(ToggleSound);

        LoadSoundStatus(); // Kaydedilmiş ses durumunu yükle
        UpdateSoundUI();
        ApplySoundStatus(); // AudioListener ile senkronize et

        CloseSettingsPopUp();
    }

    private void ToggleSound()
    {
        _soundStatus = !_soundStatus;

        SaveSoundStatus(); // PlayerPrefs'e kaydet
        UpdateSoundUI();
        ApplySoundStatus(); // Ses durumunu uygula
    }

    private void UpdateSoundUI()
    {
        _onSlider.SetActive(_soundStatus);
        _offSlider.SetActive(!_soundStatus);
    }

    private void ApplySoundStatus()
    {
        if (_audioListener != null)
        {
            _audioListener.enabled = _soundStatus;
        }
    }

    private void CloseSettingsPopUp()
    {
        gameObject.SetActive(false);
    }

    private void SaveSoundStatus()
    {
        PlayerPrefs.SetInt(SoundPrefKey, _soundStatus ? 1 : 0);
        PlayerPrefs.Save(); // PlayerPrefs'i kaydet
    }

    private void LoadSoundStatus()
    {
        _soundStatus = PlayerPrefs.GetInt(SoundPrefKey, 1) == 1; // Varsayılan olarak açık (1) al
    }
}