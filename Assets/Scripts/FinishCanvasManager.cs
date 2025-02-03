using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinishCanvasManager : MonoBehaviour
{
    [SerializeField] private List<Image> photoSlots; 
    [SerializeField] private TMP_Text _timeText; 
    [SerializeField] private TMP_Text _usernameText;
    [SerializeField] private ScreenSpaceCanvasRenderer _screenSpaceCanvasRenderer;

    private TimeController _timeController;
    private StartPopUpController _startPopUpController;
    private string _photoFolderPath;

    public void Initialize(StartPopUpController startPopUpController, TimeController timeController)
    {
        _photoFolderPath = Path.Combine(Application.dataPath, "Resources/CapturedPhotos");
        _startPopUpController = startPopUpController;
        _timeController = timeController;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            _screenSpaceCanvasRenderer.CaptureScreenshot();
        }
    }

    public void LoadPhotos()
    {
        if (!Directory.Exists(_photoFolderPath)) return;

        string[] photoFiles = Directory.GetFiles(_photoFolderPath, "*.png")
            .OrderByDescending(f => new FileInfo(f).CreationTime)
            .Take(10) 
            .ToArray();

        for (int i = 0; i < photoSlots.Count; i++)
        {
            if (i < photoFiles.Length)
            {
                StartCoroutine(LoadImage(photoFiles[i], photoSlots[i]));
            }
            else
            {
                photoSlots[i].gameObject.SetActive(false);
            }
        }
        
        _usernameText.SetText("Username : " + _startPopUpController.Username);
        
        float elapsedTime = _timeController.ElapsedTime;
        
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int fraction = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

        _timeText.SetText("Time : " + string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction));
        _screenSpaceCanvasRenderer.CaptureScreenshot();
    }

    private IEnumerator LoadImage(string filePath, Image targetImage)
    {
        byte[] imageBytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), 
            new Vector2(0.5f, 0.5f));
        targetImage.sprite = newSprite;
        targetImage.gameObject.SetActive(true);

        yield return null;
    }
}