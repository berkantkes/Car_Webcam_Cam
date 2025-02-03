using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Windows.WebCam;

public class BannerTrigger : MonoBehaviour
{
    [SerializeField] private List<Banner> bannerList;
    [SerializeField] private PhotoGallery _photoGallery;

    private int lapCount = 0;
    private int checkpointCount = 0;
    private int startLineTouchCount = 0;

    private PhotoCapture photoCaptureObject;
    private Texture2D targetTexture;
    private string photoSavePath;
    private bool isPhotoModeActive = false; // **Kamera aktif mi takip etmek için bayrak**

    void Start()
    {
        photoSavePath = Path.Combine(Application.dataPath, "Resources/CapturedPhotos");
        if (!Directory.Exists(photoSavePath))
        {
            Directory.CreateDirectory(photoSavePath);
        }

        // 📌 **Tek Seferde PhotoCapture Nesnesi Oluştur**
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

            // **Kamerayı Başlat**
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, result =>
            {
                isPhotoModeActive = result.success;
                Debug.Log("Photo mode started: " + isPhotoModeActive);
            });
        });
    }

    void Update()
    {
        if (checkpointCount >= bannerList.Count)
        {
            lapCount++;
            checkpointCount = 0;
            Debug.Log($"Tur tamamlandı! Şu an {lapCount}. turdayız.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Banner"))
        {
            if (lapCount >= 3) return;

            Banner hitBanner = bannerList.Find(b => b.bannerObject == other.gameObject);
            if (hitBanner == null) return;

            checkpointCount++;

            int bannerIndex = bannerList.IndexOf(hitBanner);

            if ((lapCount == 0 && bannerIndex % 2 == 0) ||
                (lapCount == 1 && bannerIndex % 2 != 0))
            {
                TakePhoto(hitBanner);
            }
        }
        else if (other.CompareTag("StartLine"))
        {
            startLineTouchCount++;
            if (startLineTouchCount == 4)
            {
                EventManager.Execute(GameEvents.OnFinishGame);
                Debug.Log("Game finished!");
                _photoGallery.LoadPhotos();
                
                // 📌 **Oyun bittiğinde kamerayı kapat**
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
    }

    void TakePhoto(Banner banner)
    {
        if (!isPhotoModeActive) return; // 📌 **Fotoğraf modu aktif değilse çekme**
        
        photoCaptureObject.TakePhotoAsync((captureResult, photoCaptureFrame) =>
        {
            StartCoroutine(OnCapturedPhotoToMemory(captureResult, photoCaptureFrame, banner));
        });
    }

    IEnumerator OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame, Banner banner)
    {
        if (result.success)
        {
            List<byte> imageBufferList = new List<byte>();
            photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);
            targetTexture = new Texture2D(1280, 720, TextureFormat.BGRA32, false);
            targetTexture.LoadRawTextureData(imageBufferList.ToArray());
            targetTexture.Apply();

            // 📌 Fotoğrafı ters çevirme işlemini optimize etmek için Coroutine kullan
            yield return StartCoroutine(FlipTextureVertically(targetTexture));

            // PNG olarak kaydet
            byte[] imageBytes = targetTexture.EncodeToPNG();
            string filePath = Path.Combine(photoSavePath, "CapturedPhoto_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");

            File.WriteAllBytes(filePath, imageBytes);
            Debug.Log($"Photo saved to: {filePath}");

            // Fotoğrafı ilgili banner'ın SpriteRenderer'ına atama
            Sprite photoSprite = Sprite.Create(targetTexture, new Rect(0, 0, targetTexture.width, targetTexture.height), new Vector2(0.5f, 0.5f));
            banner.spriteRenderer.sprite = photoSprite;
        }
    }

    // 📌 **Görüntüyü Dikey Olarak Çevirme (Coroutine ile daha hızlı)**
    IEnumerator FlipTextureVertically(Texture2D original)
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
                // Piksel yer değiştir
                Color temp = pixels[topIndex + x];
                pixels[topIndex + x] = pixels[bottomIndex + x];
                pixels[bottomIndex + x] = temp;
            }
            yield return null; // Her satırda bir kare bekle (Oyun donmasın diye)
        }

        original.SetPixels(pixels);
        original.Apply();
    }
}
