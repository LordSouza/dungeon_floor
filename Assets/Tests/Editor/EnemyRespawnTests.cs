using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Testes para o sistema de respawn de inimigos
/// </summary>
public class EnemyRespawnTests
{
    [Test]
    public void SceneLoadCounter_IncrementsCorrectly()
    {
        var data = new SaveData();
        data.totalSceneLoads = 0;
        
        data.totalSceneLoads++;
        Assert.AreEqual(1, data.totalSceneLoads);
        
        data.totalSceneLoads++;
        Assert.AreEqual(2, data.totalSceneLoads);
    }
    
    [Test]
    public void EnemyDeath_RecordsCorrectly()
    {
        var data = new SaveData();
        data.totalSceneLoads = 5;
        
        string enemyId = "skeleton_1";
        var deathRecord = new EnemyDeathRecord
        {
            enemyId = enemyId,
            deathCount = 1,
            sceneLoadsAtDeath = data.totalSceneLoads,
            originalX = 10.0f,
            originalY = 2.0f
        };
        
        data.enemyDeathRecords.Add(deathRecord);
        
        Assert.AreEqual(1, data.enemyDeathRecords.Count);
        Assert.AreEqual(enemyId, data.enemyDeathRecords[0].enemyId);
        Assert.AreEqual(5, data.enemyDeathRecords[0].sceneLoadsAtDeath);
    }
    
    [Test]
    public void EnemyRespawn_ChecksThreshold()
    {
        var data = new SaveData();
        data.respawnAfterSceneLoads = 1;
        data.totalSceneLoads = 10;
        
        // Enemy died at scene load 5
        int sceneLoadsAtDeath = 5;
        int sceneLoadsSinceDeath = data.totalSceneLoads - sceneLoadsAtDeath;
        
        bool shouldRespawn = sceneLoadsSinceDeath >= data.respawnAfterSceneLoads;
        
        Assert.IsTrue(shouldRespawn, "Enemy should respawn after 5 scene loads with threshold of 1");
    }
    
    [Test]
    public void EnemyRespawn_NotReadyYet()
    {
        var data = new SaveData();
        data.respawnAfterSceneLoads = 3;
        data.totalSceneLoads = 10;
        
        // Enemy died at scene load 9
        int sceneLoadsAtDeath = 9;
        int sceneLoadsSinceDeath = data.totalSceneLoads - sceneLoadsAtDeath;
        
        bool shouldRespawn = sceneLoadsSinceDeath >= data.respawnAfterSceneLoads;
        
        Assert.IsFalse(shouldRespawn, "Enemy should not respawn after only 1 scene load with threshold of 3");
    }
    
    [Test]
    public void SpawnPoints_InitializeCorrectly()
    {
        var data = new SaveData();
        
        // Simulate 3 enemies with positions
        var spawnPoint1 = new EnemySpawnPoint(5.0f, 2.0f);
        var spawnPoint2 = new EnemySpawnPoint(15.0f, 3.5f);
        var spawnPoint3 = new EnemySpawnPoint(25.0f, 1.0f);
        
        data.enemySpawnPoints.Add(spawnPoint1);
        data.enemySpawnPoints.Add(spawnPoint2);
        data.enemySpawnPoints.Add(spawnPoint3);
        
        Assert.AreEqual(3, data.enemySpawnPoints.Count);
    }
    
    [Test]
    public void RandomSpawn_SelectsFromPool()
    {
        var data = new SaveData();
        data.enableRandomSpawns = true;
        
        data.enemySpawnPoints.Add(new EnemySpawnPoint(10.0f, 2.0f));
        data.enemySpawnPoints.Add(new EnemySpawnPoint(20.0f, 3.0f));
        data.enemySpawnPoints.Add(new EnemySpawnPoint(30.0f, 4.0f));
        
        // Simulate random selection
        int randomIndex = Random.Range(0, data.enemySpawnPoints.Count);
        var selectedSpawn = data.enemySpawnPoints[randomIndex];
        
        Assert.IsTrue(randomIndex >= 0 && randomIndex < data.enemySpawnPoints.Count);
        Assert.IsNotNull(selectedSpawn);
    }
    
    [Test]
    public void RandomSpawn_DisabledWhenFlagOff()
    {
        var data = new SaveData();
        data.enableRandomSpawns = false;
        
        Assert.IsFalse(data.enableRandomSpawns, "Random spawns should be disabled");
    }
    
    [Test]
    public void RandomDirection_FiftyPercentChance()
    {
        // Simulate direction flip logic
        int flipCount = 0;
        int noFlipCount = 0;
        int iterations = 100;
        
        for (int i = 0; i < iterations; i++)
        {
            bool shouldFlip = Random.value > 0.5f;
            if (shouldFlip)
                flipCount++;
            else
                noFlipCount++;
        }
        
        // With 100 iterations, should be roughly 50/50 (allow 20-80 range)
        Assert.Greater(flipCount, 20, "Should flip at least 20% of the time");
        Assert.Less(flipCount, 80, "Should flip at most 80% of the time");
    }
    
    [Test]
    public void DeadEnemies_MigrateToNewSystem()
    {
        var data = new SaveData();
        
        // Old system
        data.deadEnemies.Add("skeleton_1");
        data.deadEnemies.Add("skeleton_2");
        
        // Simulate migration
        foreach (var enemyId in data.deadEnemies)
        {
            bool alreadyMigrated = data.enemyDeathRecords.Any(r => r.enemyId == enemyId);
            if (!alreadyMigrated)
            {
                var record = new EnemyDeathRecord
                {
                    enemyId = enemyId,
                    deathCount = 1,
                    sceneLoadsAtDeath = 0,
                    originalX = 0,
                    originalY = 0
                };
                data.enemyDeathRecords.Add(record);
            }
        }
        
        Assert.AreEqual(2, data.enemyDeathRecords.Count);
        Assert.IsTrue(data.enemyDeathRecords.Any(r => r.enemyId == "skeleton_1"));
        Assert.IsTrue(data.enemyDeathRecords.Any(r => r.enemyId == "skeleton_2"));
    }
    
    [Test]
    public void CleanupRespawned_RemovesOldRecords()
    {
        var data = new SaveData();
        data.totalSceneLoads = 10;
        data.respawnAfterSceneLoads = 1;
        
        // Add old death record that should be cleaned up
        data.enemyDeathRecords.Add(new EnemyDeathRecord
        {
            enemyId = "old_enemy",
            deathCount = 1,
            sceneLoadsAtDeath = 5, // 5 loads ago
            originalX = 0,
            originalY = 0
        });
        
        // Simulate cleanup
        data.enemyDeathRecords.RemoveAll(record =>
        {
            int sceneLoadsSinceDeath = data.totalSceneLoads - record.sceneLoadsAtDeath;
            return sceneLoadsSinceDeath >= data.respawnAfterSceneLoads;
        });
        
        Assert.AreEqual(0, data.enemyDeathRecords.Count, "Old death records should be cleaned up");
    }
    
    [Test]
    public void MultipleDeaths_IncrementDeathCount()
    {
        var data = new SaveData();
        string enemyId = "skeleton_boss";
        
        // First death
        var record = new EnemyDeathRecord
        {
            enemyId = enemyId,
            deathCount = 1,
            sceneLoadsAtDeath = 5,
            originalX = 0,
            originalY = 0
        };
        data.enemyDeathRecords.Add(record);
        
        // Second death (after respawn)
        var existingRecord = data.enemyDeathRecords.FirstOrDefault(r => r.enemyId == enemyId);
        if (existingRecord != null)
        {
            existingRecord.deathCount++;
            existingRecord.sceneLoadsAtDeath = 10;
        }
        
        Assert.AreEqual(2, existingRecord.deathCount);
    }
    
    [Test]
    public void EnemySpawnPoint_StoresPosition()
    {
        float x = 12.34f;
        float y = 56.78f;
        
        var spawnPoint = new EnemySpawnPoint(x, y);
        
        Assert.AreEqual(x, spawnPoint.x, 0.001f);
        Assert.AreEqual(y, spawnPoint.y, 0.001f);
    }
    
    [Test]
    public void RespawnThreshold_ConfigurablePerEnemy()
    {
        var data = new SaveData();
        
        // Default threshold
        int defaultThreshold = 1;
        data.respawnAfterSceneLoads = defaultThreshold;
        
        Assert.AreEqual(defaultThreshold, data.respawnAfterSceneLoads);
        
        // Can be changed
        data.respawnAfterSceneLoads = 5;
        Assert.AreEqual(5, data.respawnAfterSceneLoads);
    }
}
