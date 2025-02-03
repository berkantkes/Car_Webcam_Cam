using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HighResScreenshot : MonoBehaviour
{
    [SerializeField] private Canvas targetCanvas; // Hangi Canvas'tan render alınacak
    [SerializeField] private int resolutionMultiplier = 2; // Çözünürlük katlaması
    [SerializeField] private Camera uiCamera; // Mevcut UI kameramız

    private string savePath;

    private void Start()
    {
        // StreamingAssets içinde kaydetme klasörü
        savePath = Path.Combine(Application.streamingAssetsPath, "RaceResults");
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
    }

    public void CaptureScreenshot()
    {
        StartCoroutine(RenderCanvasToTexture());
    }

    private IEnumerator RenderCanvasToTexture()
    {
        yield return new WaitForEndOfFrame(); // Çerçeve tamamlandığında render al

        if (uiCamera == null)
        {
            Debug.LogError("UI Kamerası atanmadı!");
            yield break;
        }

        // 🎯 Mevcut Kamera Ayarlarını Kaydet
        RenderTexture originalRenderTexture = uiCamera.targetTexture;
        CameraClearFlags originalClearFlags = uiCamera.clearFlags;
        Color originalBackgroundColor = uiCamera.backgroundColor;
        RenderMode previousRenderMode = targetCanvas.renderMode;

        // 🎯 Canvas'ı geçici olarak World Space moduna al
        targetCanvas.renderMode = RenderMode.WorldSpace;
        RectTransform canvasRect = targetCanvas.GetComponent<RectTransform>();

        // 🎯 Yüksek çözünürlük ayarları
        int width = (int)(canvasRect.rect.width * resolutionMultiplier);
        int height = (int)(canvasRect.rect.height * resolutionMultiplier);
        RenderTexture renderTexture = new RenderTexture(width, height, 24);

        // 🎯 Kamerayı ayarla
        uiCamera.targetTexture = renderTexture;
        uiCamera.clearFlags = CameraClearFlags.SolidColor;
        uiCamera.backgroundColor = Color.black;
        uiCamera.orthographic = true;
        uiCamera.orthographicSize = canvasRect.rect.height / 2;
        uiCamera.transform.position = new Vector3(canvasRect.position.x, canvasRect.position.y, -10);

        // 🎯 Kamera ile render al
        uiCamera.Render();

        // 🎯 RenderTexture'tan Texture2D'ye aktar
        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        // 🎯 PNG olarak kaydet
        string fileName = "RaceResult_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(savePath, fileName);
        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Screenshot saved to: " + filePath);

        // 🎯 Kamera ve Canvas ayarlarını eski haline döndür
        uiCamera.targetTexture = originalRenderTexture;
        uiCamera.clearFlags = originalClearFlags;
        uiCamera.backgroundColor = originalBackgroundColor;
        targetCanvas.renderMode = previousRenderMode;

        // 🎯 Bellek temizliği
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(screenshot);
    }
}
