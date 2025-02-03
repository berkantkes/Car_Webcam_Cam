using System.IO;
using UnityEngine;

public class GameResultManager : MonoBehaviour
{
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.streamingAssetsPath, "game_results.json");
    }

    public void SaveGameResult(GameResult result)
    {
        if (result == null) return;

        GameResultData data = new GameResultData
        {
            username = result.username,
            raceTime = result.raceTime,
            base64Image = result.GetBase64Image() 
        };

        string json = JsonUtility.ToJson(data, true);
        
        // StreamingAssets klasörüne yaz
        File.WriteAllText(filePath, json);
        Debug.Log($"Game result saved to {filePath}");
    }
}

// JSON için özel veri modeli
[System.Serializable]
public class GameResultData
{
    public string username;
    public float raceTime;
    public string base64Image;
}