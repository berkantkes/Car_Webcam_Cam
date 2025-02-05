using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Serialization;
using UnityEngine.Windows.WebCam;

public class TakePhotoManager : MonoBehaviour
{
    private BannerManager _bannerManager;
    private PhotoCapture photoCaptureObject;
    private Texture2D targetTexture;
    private string photoSavePath;
    private bool isPhotoModeActive = false;
    private int captureCount = 0;     
    private bool webcamInitialized = false; // 📌 WebCam'in zaten başlatılıp başlatılmadığını kontrol eder
    
    public void Initialize(BannerManager bannerManager)
    {
        _bannerManager = bannerManager;
        // 📌 **Yeni yol: Assets/Resources/CapturedPhotos**
        photoSavePath = Path.Combine(Application.dataPath, "Resources", "CapturedPhotos");
        
        if (!Directory.Exists(photoSavePath))
        {
            Directory.CreateDirectory(photoSavePath);
        }
        
    }

    private void OnEnable()
    {
        EventManager.Subscribe(GameEvents.OnStartGame, StartTakePhoto);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEvents.OnStartGame, StartTakePhoto);
    }

    private void StartTakePhoto()
    { 
        if (webcamInitialized) 
        {
            Debug.LogWarning("Photo Mode is already active! Skipping initialization.");
            return; // 📌 Eğer WebCam zaten başlatılmışsa, tekrar açmaya çalışma
        }
        PhotoCapture.CreateAsync(false, captureObject =>
        {
            photoCaptureObject = captureObject;
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending(res => res.width * res.height).First();
            CameraParameters cameraParameters = new CameraParameters
            {
                hologramOpacity = 0.0f,
                cameraResolutionWidth = cameraResolution.width,
                cameraResolutionHeight = cameraResolution.height,
                pixelFormat = CapturePixelFormat.BGRA32
            };

            photoCaptureObject.StartPhotoModeAsync(cameraParameters, result =>
            {
                isPhotoModeActive = result.success;
                webcamInitialized = true;
                Debug.Log("Photo mode started: " + isPhotoModeActive);
                
                // 📌 **12 saniyede bir fotoğraf çekmeye başla**
                StartCoroutine(CapturePhotoRoutine());
            });
        });
    }
    
    private IEnumerator CapturePhotoRoutine()
    {
        captureCount = 0; 
        while (isPhotoModeActive && captureCount < 10) // 📌 10 kere çalışsın
        {
            yield return new WaitForSeconds(12f);
            TakePhoto();
            captureCount++; // 📌 Her çalıştığında sayaç artır
        }
    }

    private void TakePhoto()
    {
        if (!isPhotoModeActive) return;

        photoCaptureObject.TakePhotoAsync((captureResult, photoCaptureFrame) =>
        {
            StartCoroutine(OnCapturedPhotoToMemory(captureResult, photoCaptureFrame));
        });
    }

    private IEnumerator OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            List<byte> imageBufferList = new List<byte>();
            photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);
            targetTexture = new Texture2D(1280, 720, TextureFormat.BGRA32, false);
            targetTexture.LoadRawTextureData(imageBufferList.ToArray());
            targetTexture.Apply();

            // 📌 **Görüntüyü Dikey Olarak Çevir**
            yield return StartCoroutine(FlipTextureVertically(targetTexture));

            // PNG olarak kaydet
            byte[] imageBytes = targetTexture.EncodeToPNG();
            string filePath = Path.Combine(photoSavePath, "CapturedPhoto_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");

            File.WriteAllBytes(filePath, imageBytes);
            Debug.Log($"Photo saved to: {filePath}");

            // 📌 **Fotoğrafı bir Sprite olarak oluştur ve BannerManager'a gönder**
            Sprite photoSprite = Sprite.Create(targetTexture, new Rect(0, 0, targetTexture.width, targetTexture.height), new Vector2(0.5f, 0.5f));
            _bannerManager.SetBannerSprite(photoSprite);

            // 📌 **Resources.Load ile erişebilmek için dosya adını kaydet**
            string resourcesPath = "CapturedPhotos/" + Path.GetFileNameWithoutExtension(filePath);
            Debug.Log($"Photo saved in Resources: {resourcesPath}");
        }
    }

    private IEnumerator FlipTextureVertically(Texture2D original)
    {
        int width = original.width;
        int height = original.height;
        Color[] pixels = original.GetPixels();

        for (int y = 0; y < height / 2; y++)
        {
            int topIndex = y * width;
            int bottomIndex = (height - 1 - y) * width;

            for (int x = 0; x < width; x++)
            {
                Color temp = pixels[topIndex + x];
                pixels[topIndex + x] = pixels[bottomIndex + x];
                pixels[bottomIndex + x] = temp;
            }
            yield return null;
        }

        original.SetPixels(pixels);
        original.Apply();
    }

    private void OnDestroy()
    {
        if (isPhotoModeActive)
        {
            photoCaptureObject.StopPhotoModeAsync(result =>
            {
                photoCaptureObject.Dispose();
                photoCaptureObject = null;
                isPhotoModeActive = false;
                Debug.Log("Photo mode stopped.");
            });
        }
    }
}
