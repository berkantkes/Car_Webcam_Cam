using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGamePanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;

    public TMP_Text GetTimerText()
    {
        return _timerText;
    }
}
