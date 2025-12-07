using System.Linq;
using UnityEngine;

public class MapSceneLoader : MonoBehaviour
{
    void Start()
    {
        var data = GameManager.Instance.data;
        
        // Increment scene load counter for respawn system
        data.totalSceneLoads++;
        
        // Migrate old system to new system (one-time conversion)
        MigrateOldDeadEnemies(data);
        
        // Clean up respawned enemies from death records
        CleanupRespawnedEnemies(data);
        
        Player player = FindObjectOfType<Player>();
        
        if (player != null && (data.playerX != 0 || data.playerY != 0))
        {
            player.transform.position = new Vector3(data.playerX, data.playerY, 0);
        }
        
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        foreach (Enemy e in enemies)
        {
            if (!string.IsNullOrEmpty(e.enemyId))
            {
                // Check if enemy should still be dead
                if (IsEnemyDead(e.enemyId, data))
                {
                    Destroy(e.gameObject);
                }
            }
        }
        
        GameManager.Instance.Save();
    }
    
    void MigrateOldDeadEnemies(SaveData data)
    {
        // Migrate old deadEnemies list to new system
        if (data.deadEnemies.Count > 0)
        {
            foreach (string enemyId in data.deadEnemies)
            {
                // Check if not already in new system
                if (!data.enemyDeathRecords.Any(r => r.enemyId == enemyId))
                {
                    data.enemyDeathRecords.Add(new EnemyDeathRecord(enemyId, data.totalSceneLoads));
                }
            }
            // Clear old list after migration
            data.deadEnemies.Clear();
        }
    }
    
    void CleanupRespawnedEnemies(SaveData data)
    {
        // Remove enemies that should respawn
        data.enemyDeathRecords.RemoveAll(record => 
            data.totalSceneLoads - record.sceneLoadsAtDeath >= data.respawnAfterSceneLoads
        );
    }
    
    bool IsEnemyDead(string enemyId, SaveData data)
    {
        // Check new system
        var record = data.enemyDeathRecords.FirstOrDefault(r => r.enemyId == enemyId);
        if (record != null)
        {
            int scenesSinceDeath = data.totalSceneLoads - record.sceneLoadsAtDeath;
            return scenesSinceDeath < data.respawnAfterSceneLoads;
        }
        
        return false;
    }
}