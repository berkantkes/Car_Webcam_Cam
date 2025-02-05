using System;
using UnityEngine;
using TMPro;

public class TimeController : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    
    private bool isCounting = false;
    private float elapsedTime = 0f;

    public float ElapsedTime => elapsedTime;

    public void Initialize(UiController uiController)
    {
        timerText = uiController.GetTimerText();
    }
    
    private void OnEnable()
    {
        EventManager.Subscribe(GameEvents.OnStartGame, ResetTimer);
        EventManager.Subscribe(GameEvents.OnFinishGame, StopTimer);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEvents.OnStartGame, ResetTimer);
        EventManager.Unsubscribe(GameEvents.OnFinishGame, StopTimer);
    }

    void Update()
    {
        if (isCounting)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int fraction = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);
    }

    private void StopTimer()
    {
        isCounting = false;
    }
    
    private void ResetTimer()
    {
        elapsedTime = 0f;
        isCounting = true;
        UpdateTimerText();
    }
}