using System.Linq;
using UnityEngine;

public class MapSceneLoader : MonoBehaviour
{
    void Start()
    {
        var data = GameManager.Instance.data;
        
        // Increment scene load counter for respawn system
        data.totalSceneLoads++;
        
        Debug.Log($"=== MAP SCENE LOADED (Scene Load #{data.totalSceneLoads}) ===");
        Debug.Log($"Respawn threshold: {data.respawnAfterSceneLoads} scene loads");
        
        // Migrate old system to new system (one-time conversion)
        MigrateOldDeadEnemies(data);
        
        // Clean up respawned enemies from death records
        CleanupRespawnedEnemies(data);
        
        Debug.Log($"Active death records: {data.enemyDeathRecords.Count}");
        
        Player player = FindFirstObjectByType<Player>();
        
        // Restaura a posição do jogador se já tiver spawned antes
        if (player != null && data.hasSpawnedOnce)
        {
            player.transform.position = new Vector3(data.playerX, data.playerY, 0);
            Debug.Log($"Player position restored to: ({data.playerX}, {data.playerY})");
        }
        
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Debug.Log($"Total enemies in scene: {enemies.Length}");
        
        // Store original spawn positions if not already stored
        InitializeSpawnPoints(data, enemies);

        int destroyedCount = 0;
        foreach (Enemy e in enemies)
        {
            if (!string.IsNullOrEmpty(e.enemyId))
            {
                // Check if enemy should still be dead
                if (IsEnemyDead(e.enemyId, data))
                {
                    Debug.Log($"Destroying enemy: {e.enemyId} (still dead)");
                    Destroy(e.gameObject);
                    destroyedCount++;
                }
                else
                {
                    // Enemy is alive - apply random spawn if respawned
                    ApplyRandomSpawnIfNeeded(e, data);
                    Debug.Log($"Enemy {e.enemyId} is alive at position ({e.transform.position.x}, {e.transform.position.y})");
                }
            }
        }
        
        Debug.Log($"Enemies destroyed: {destroyedCount}, Enemies alive: {enemies.Length - destroyedCount}");
        
        GameManager.Instance.Save();
    }
    
    void InitializeSpawnPoints(SaveData data, Enemy[] enemies)
    {
        // Collect all original enemy positions as valid spawn points
        if (data.enemySpawnPoints.Count == 0 && enemies.Length > 0)
        {
            Debug.Log("Initializing spawn points...");
            foreach (Enemy e in enemies)
            {
                if (!string.IsNullOrEmpty(e.enemyId))
                {
                    var spawnPoint = new EnemySpawnPoint(e.transform.position.x, e.transform.position.y);
                    data.enemySpawnPoints.Add(spawnPoint);
                    Debug.Log($"Added spawn point: ({spawnPoint.x}, {spawnPoint.y})");
                }
            }
        }
    }
    
    void ApplyRandomSpawnIfNeeded(Enemy enemy, SaveData data)
    {
        // Only apply random spawn if enabled and there are spawn points
        if (!data.enableRandomSpawns || data.enemySpawnPoints.Count == 0)
            return;
        
        // ALWAYS randomize position - every map load!
        var randomSpawn = data.enemySpawnPoints[Random.Range(0, data.enemySpawnPoints.Count)];
        enemy.transform.position = new Vector3(randomSpawn.x, randomSpawn.y, enemy.transform.position.z);
        
        // Randomly flip direction (50% chance)
        bool shouldFlip = Random.value > 0.5f;
        if (shouldFlip)
        {
            //enemy.FlipDirection();
            Debug.Log($"Enemy {enemy.enemyId} spawned at random position: ({randomSpawn.x}, {randomSpawn.y}) - Direction FLIPPED");
        }
        else
        {
            Debug.Log($"Enemy {enemy.enemyId} spawned at random position: ({randomSpawn.x}, {randomSpawn.y}) - Original direction");
        }
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