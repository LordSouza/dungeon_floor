using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Testes de integração entre sistemas
/// </summary>
public class IntegrationTests
{
    [Test]
    public void FullGameLoop_NewGame()
    {
        var data = new SaveData();
        
        // Initial state
        Assert.AreEqual(1, data.playerLevel);
        Assert.AreEqual(0, data.playerXP);
        Assert.AreEqual(20, data.playerMaxHP);
        Assert.AreEqual(5, data.playerDamage);
        Assert.AreEqual(0, data.fishCount);
        Assert.AreEqual(0, data.totalSceneLoads);
    }
    
    [Test]
    public void FullGameLoop_FirstBattle()
    {
        var data = new SaveData();
        
        // Player defeats level 1 enemy
        int enemyLevel = 1;
        int xpGained = enemyLevel * 5;
        
        data.playerXP += xpGained;
        
        Assert.AreEqual(5, data.playerXP, "Should gain 5 XP from level 1 enemy");
    }
    
    [Test]
    public void FullGameLoop_LevelUpAndContinue()
    {
        var data = new SaveData();
        
        // Gain enough XP to level up (10 XP for level 1->2)
        data.playerXP = 10;
        data.playerXPToNextLevel = 10;
        
        // Level up
        if (data.playerXP >= data.playerXPToNextLevel)
        {
            data.playerLevel++;
            data.playerXP -= data.playerXPToNextLevel;
            data.playerMaxHP += 5;
            data.playerDamage += 2;
            data.playerXPToNextLevel = Mathf.RoundToInt(10f * Mathf.Pow(data.playerLevel, 1.2f));
        }
        
        Assert.AreEqual(2, data.playerLevel);
        Assert.AreEqual(25, data.playerMaxHP);
        Assert.AreEqual(7, data.playerDamage);
    }
    
    [Test]
    public void FullGameLoop_FishingAndBattle()
    {
        var data = new SaveData();
        data.playerLevel = 3;
        data.playerMaxHP = 30;
        
        // Go fishing
        data.fishCount++;
        Assert.AreEqual(1, data.fishCount);
        
        // Enter battle at low HP
        int playerHP = 8;
        
        // Use fish
        if (data.fishCount > 0)
        {
            data.fishCount--;
            playerHP += 12;
            playerHP = Mathf.Min(playerHP, data.playerMaxHP);
        }
        
        Assert.AreEqual(20, playerHP);
        Assert.AreEqual(0, data.fishCount);
    }
    
    [Test]
    public void FullGameLoop_EnemyRespawnCycle()
    {
        var data = new SaveData();
        data.respawnAfterSceneLoads = 1;
        
        // Scene 1: Enemy alive, player defeats it
        data.totalSceneLoads = 1;
        string enemyId = "skeleton_1";
        
        var deathRecord = new EnemyDeathRecord(enemyId, data.totalSceneLoads, 10, 2);
        data.enemyDeathRecords.Add(deathRecord);
        
        // Scene 2: Enemy still dead
        data.totalSceneLoads = 2;
        int sceneLoadsSinceDeath = data.totalSceneLoads - deathRecord.sceneLoadsAtDeath;
        bool shouldRespawn = sceneLoadsSinceDeath >= data.respawnAfterSceneLoads;
        
        Assert.IsTrue(shouldRespawn, "Enemy should respawn after 1 scene load");
    }
    
    [Test]
    public void FullGameLoop_ProgressionToLevel10()
    {
        var data = new SaveData();
        
        // Simulate leveling from 1 to 10
        for (int level = 1; level < 10; level++)
        {
            int xpRequired = Mathf.RoundToInt(10f * Mathf.Pow(level, 1.2f));
            data.playerXP = xpRequired;
            data.playerLevel = level;
            
            // Level up
            data.playerLevel++;
            data.playerXP = 0;
            
            // Base growth
            data.playerMaxHP += 5;
            data.playerDamage += 2;
            
            // Milestone at level 5 and 10
            if (data.playerLevel % 5 == 0)
            {
                data.playerMaxHP += 10;
                data.playerDamage += 3;
            }
        }
        
        Assert.AreEqual(10, data.playerLevel);
        // Base: 20 + (9*5) + 2 milestones (10+10) = 20 + 45 + 20 = 85
        Assert.AreEqual(85, data.playerMaxHP);
        // Base: 5 + (9*2) + 2 milestones (3+3) = 5 + 18 + 6 = 29
        Assert.AreEqual(29, data.playerDamage);
    }
    
    [Test]
    public void FullGameLoop_MultipleItemTypes()
    {
        var data = new SaveData();
        
        // Currently only fish, but test framework for multiple items
        data.fishCount = 5;
        
        int totalHealingPotential = data.fishCount * 12;
        
        Assert.AreEqual(60, totalHealingPotential, "5 fish should heal 60 HP total");
    }
    
    [Test]
    public void FullGameLoop_SaveLoadCycle()
    {
        var data = new SaveData();
        
        // Modify game state
        data.playerLevel = 5;
        data.playerXP = 25;
        data.playerMaxHP = 50;
        data.playerDamage = 15;
        data.fishCount = 3;
        data.totalSceneLoads = 10;
        data.playerX = 15.5f;
        data.playerY = 3.2f;
        
        // Simulate save (would use JsonUtility.ToJson)
        string json = JsonUtility.ToJson(data);
        
        // Simulate load (would use JsonUtility.FromJson)
        var loadedData = JsonUtility.FromJson<SaveData>(json);
        
        Assert.AreEqual(5, loadedData.playerLevel);
        Assert.AreEqual(25, loadedData.playerXP);
        Assert.AreEqual(50, loadedData.playerMaxHP);
        Assert.AreEqual(15, loadedData.playerDamage);
        Assert.AreEqual(3, loadedData.fishCount);
        Assert.AreEqual(10, loadedData.totalSceneLoads);
        Assert.AreEqual(15.5f, loadedData.playerX, 0.01f);
        Assert.AreEqual(3.2f, loadedData.playerY, 0.01f);
    }
    
    [Test]
    public void FullGameLoop_DeathAndReset()
    {
        var data = new SaveData();
        
        // Player progressed to level 5
        data.playerLevel = 5;
        data.playerXP = 50;
        data.playerMaxHP = 50;
        data.fishCount = 10;
        
        // Player dies - game resets to new game state
        var newData = new SaveData();
        
        Assert.AreEqual(1, newData.playerLevel);
        Assert.AreEqual(0, newData.playerXP);
        Assert.AreEqual(20, newData.playerMaxHP);
        Assert.AreEqual(0, newData.fishCount);
    }
    
    [Test]
    public void FullGameLoop_BattleToFishingToMap()
    {
        var data = new SaveData();
        data.hasSpawnedOnce = true;
        
        // Start in map
        float mapX = 10.0f;
        float mapY = 5.0f;
        
        // Enter battle
        data.playerX = mapX;
        data.playerY = mapY;
        
        // Return to map
        Assert.AreEqual(mapX, data.playerX);
        Assert.AreEqual(mapY, data.playerY);
        
        // Go to fishing boat
        float boatX = 20.0f;
        float boatY = 3.0f;
        data.playerX = boatX;
        data.playerY = boatY;
        
        // Catch fish
        data.fishCount++;
        
        // Return to map at boat position
        Assert.AreEqual(boatX, data.playerX);
        Assert.AreEqual(boatY, data.playerY);
        Assert.AreEqual(1, data.fishCount);
    }
}
