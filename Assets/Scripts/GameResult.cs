using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewGameResult", menuName = "Game/Game Result")]
public class GameResult : ScriptableObject
{
    public string username;
    public float raceTime;
    public Texture2D raceSnapshot; // Görsel kaydedilecek

    // Görseli base64 string olarak kaydetmek için yardımcı fonksiyon
    public string GetBase64Image()
    {
        if (raceSnapshot == null) return "";
        byte[] imageBytes = raceSnapshot.EncodeToPNG();
        return System.Convert.ToBase64String(imageBytes);
    }
}