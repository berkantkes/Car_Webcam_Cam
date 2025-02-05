using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class RaceResultManager : MonoBehaviour
{
    private string savePath;

    public void Initialize()
    {
        // 📌 **Kayıt yolu: StreamingAssets/RaceResults**
        savePath = Path.Combine(Application.streamingAssetsPath, "RaceResults");

        // 📌 **Klasör yoksa oluştur**
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
    }

    public void SaveRaceResult(string username, float raceTime, Sprite raceResultSprite)
    {
        // 📌 **Sprite'ı PNG olarak kaydet**
        string spriteFileName = username + "_result.png";
        string spriteFilePath = Path.Combine(savePath, spriteFileName);
        SaveSpriteAsPNG(raceResultSprite, spriteFilePath);

        // 📌 **Yarış sonucu JSON olarak kaydet**
        RaceResult newResult = new RaceResult(username, raceTime, spriteFileName);
        string jsonFilePath = Path.Combine(savePath, username + "_result.json");
        string jsonData = JsonUtility.ToJson(newResult, true);
        File.WriteAllText(jsonFilePath, jsonData);

        Debug.Log($"Yarış sonucu JSON olarak kaydedildi: {jsonFilePath}");
    }

    private void SaveSpriteAsPNG(Sprite sprite, string filePath)
    {
        Texture2D texture = sprite.texture;
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        Debug.Log($"Sprite PNG olarak kaydedildi: {filePath}");
    }

    public List<RaceResult> GetTop3RaceResults()
    {
        if (!Directory.Exists(savePath))
        {
            Debug.LogWarning("RaceResults klasörü bulunamadı.");
            return new List<RaceResult>();
        }

        List<RaceResult> raceResults = new List<RaceResult>();
        string[] files = Directory.GetFiles(savePath, "*.json");

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            RaceResult result = JsonUtility.FromJson<RaceResult>(json);
            raceResults.Add(result);
        }

        // 📌 **En iyi 3 yarışçıyı bul**
        return raceResults.OrderBy(r => r.RaceTime).Take(3).ToList();
    }

    public Sprite LoadSpriteFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Sprite dosyası bulunamadı: " + filePath);
            return null;
        }

        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}

