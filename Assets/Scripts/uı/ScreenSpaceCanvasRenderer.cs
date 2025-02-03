using UnityEngine;
using System.Collections;
using System.IO;

public class ScreenSpaceCanvasRenderer : MonoBehaviour
{
    public Camera renderCamera; // Canvas ile ilişkili kamera
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;

    private RenderTexture renderTexture;

    public void CaptureScreenshot()
    {
        StartCoroutine(CaptureScreenSpaceCanvas());
    }

    private IEnumerator CaptureScreenSpaceCanvas()
    {
        // RenderTexture oluştur
        renderCamera.depth = 2;
        renderTexture = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        renderCamera.targetTexture = renderTexture;

        // Bir kare bekleyerek kameranın RenderTexture’a çizmesini bekle
        yield return new WaitForEndOfFrame();

        // Texture2D oluştur ve RenderTexture'tan veri al
        Texture2D tex = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        tex.Apply();

        // PNG olarak kaydet
        byte[] bytes = tex.EncodeToPNG();
        string filePath = Application.streamingAssetsPath + "/race_result.png";
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Yarış sonucu kaydedildi: " + filePath);

        // Belleği temizle
        RenderTexture.active = null;
        renderCamera.targetTexture = null;
        renderTexture.Release();
        Destroy(renderTexture);
    }
}