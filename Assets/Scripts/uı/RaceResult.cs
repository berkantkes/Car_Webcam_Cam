using UnityEngine;
using System;

[Serializable]
public class RaceResult
{
    public string Username;  // Kullanıcı Adı
    public float RaceTime;   // Yarış Süresi (saniye)
    public string SpritePath; // Kaydedilen PNG dosya yolu

    public RaceResult(string username, float raceTime, string spritePath)
    {
        Username = username;
        RaceTime = raceTime;
        SpritePath = spritePath;
    }
}