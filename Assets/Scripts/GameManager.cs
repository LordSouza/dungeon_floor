using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SaveData data = new SaveData();
    string _savePath;

    private void Awake()
    {
        
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        _savePath = Application.persistentDataPath + "/save.json";
        Load();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_savePath, json);
    }

    public void Load()
    {
        if (!File.Exists(_savePath))
            return;
        string json = File.ReadAllText(_savePath);
        data = JsonUtility.FromJson<SaveData>(json);
    }

    public void MarkEnemyAsDead(string enemyID)
    {
        if (!data.deadEnemies.Contains(enemyID))
            data.deadEnemies.Add(enemyID);

        Save();
    }

}