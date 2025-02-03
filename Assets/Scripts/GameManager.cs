using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TimeController _timeController;
    [SerializeField] private SimpleCarController _simpleCarController;

    private void OnEnable()
    {
        EventManager.Subscribe(GameEvents.OnStartGame, Initialize);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEvents.OnStartGame, Initialize);
    }

    public void Initialize()
    {
        _timeController.Initialize();
        _simpleCarController.Initialize();
    }

}
