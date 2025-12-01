using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public List<string> defeatedEnemies = new List<string>();
    public string currentEnemyId;

    private const string KEY = "GAME_DATA";

    public static GameData Load()
    {
        if (!PlayerPrefs.HasKey(KEY))
            return new GameData();

        string json = PlayerPrefs.GetString(KEY);
        return JsonUtility.FromJson<GameData>(json);
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(KEY, json);
    }
}