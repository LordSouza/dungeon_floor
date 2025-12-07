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
        
        ResetSave();
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
        
        // Recalculate XP requirement if it's invalid (for old saves or corrupted data)
        if (data.playerXPToNextLevel <= 0)
        {
            data.playerXPToNextLevel = CalculateXPRequirement(data.playerLevel);
            Save();
        }
    }

    public void MarkEnemyAsDead(string enemyID)
    {
        if (!data.deadEnemies.Contains(enemyID))
            data.deadEnemies.Add(enemyID);

        Save();
    }
    
    public void ResetSave()
    {
        data = new SaveData();
        // Calculate correct XP requirement for starting level
        data.playerXPToNextLevel = CalculateXPRequirement(data.playerLevel);
        Save();
    }
    
    // Helper method to calculate XP requirement (matches Unit.cs formula)
    private int CalculateXPRequirement(int level)
    {
        return Mathf.RoundToInt(10f * Mathf.Pow(level, 1.2f));
    }


}