using UnityEngine;
using System.Collections;
using System.IO;

public class RaceResultRenderManager : MonoBehaviour
{
    private RaceResultManager _raceResultManager;
    public Camera renderCamera; // Canvas ile ilişkili kamera
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;

    private RenderTexture renderTexture;
    private string savePath;

    public void Initialize(RaceResultManager raceResultManager)
    {
        _raceResultManager = raceResultManager;
        // 📌 **Kayıt klasörünü ayarla**
        savePath = Path.Combine(Application.dataPath, "Resources", "RaceResult");
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
    }

    public void CaptureScreenshot(string username, float elapsedTime)
    {
        StartCoroutine(CaptureScreenSpaceCanvas(username, elapsedTime));
    }

    private IEnumerator CaptureScreenSpaceCanvas(string username, float elapsedTime)
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

        // 📌 **Dosya adını oluştur**
        string fileName = "race_result_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(savePath, fileName);

        // PNG olarak kaydet
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Yarış sonucu kaydedildi: " + filePath);

        // 📌 **Resources'dan erişim için yolu logla**
        string resourcesPath = "RaceResult/" + Path.GetFileNameWithoutExtension(filePath);
        Debug.Log($"Race result saved in Resources: {resourcesPath}");

        
        Sprite raceSprite = LoadSpriteFromFile(filePath);
        _raceResultManager.SaveRaceResult(username, elapsedTime, raceSprite);
        
        // Belleği temizle
        RenderTexture.active = null;
        renderCamera.targetTexture = null;
        renderTexture.Release();
        Destroy(renderTexture);
    }
    
    private Sprite LoadSpriteFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void ResetRenderCamera()
    {
        renderCamera.depth = 0;
    }
}
