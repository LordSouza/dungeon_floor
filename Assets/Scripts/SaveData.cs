using System.Collections.Generic;
[System.Serializable]
public class SaveData
{
    public float playerX;
    public float playerY;
    public string lastEnemyID;
    public bool hasSpawnedOnce = false;
    
    public List<string> deadEnemies = new List<string>();
}
