using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyDeathRecord
{
    public string enemyId;
    public int deathCount; // How many times defeated
    public int sceneLoadsAtDeath; // Scene loads when enemy died
    public float originalX; // Store original spawn position
    public float originalY;
    
    public EnemyDeathRecord(string id, int sceneLoads, float x = 0, float y = 0)
    {
        enemyId = id;
        deathCount = 1;
        sceneLoadsAtDeath = sceneLoads;
        originalX = x;
        originalY = y;
    }
}

[Serializable]
public class EnemySpawnPoint
{
    public float x;
    public float y;
    
    public EnemySpawnPoint(float posX, float posY)
    {
        x = posX;
        y = posY;
    }
}

[Serializable]
public class SaveData
{
    public float playerX;
    public float playerY;

    public string lastEnemyID;
    public bool hasSpawnedOnce;

    // Old system - kept for backward compatibility but will migrate to new system
    public List<string> deadEnemies = new List<string>();
    
    // New respawn system
    public List<EnemyDeathRecord> enemyDeathRecords = new List<EnemyDeathRecord>();
    public int totalSceneLoads = 0; // Track scene loads for respawn timing
    public int respawnAfterSceneLoads = 1; // Enemies respawn after X scene loads
    public List<EnemySpawnPoint> enemySpawnPoints = new List<EnemySpawnPoint>(); // Valid spawn positions
    public bool enableRandomSpawns = true; // Toggle random spawning

    // player stats
    public int playerLevel = 1;
    public int playerXP = 0;
    public int playerXPToNextLevel = 10; // Track XP requirement
    public int playerMaxHP = 20;
    public int playerDamage = 5;
    public int playerCurrentHP = 20;

    // inventory
    public int fishCount = 0; // Number of fish items
    
    // inimigo stats
    public int currentEnemyLevel = 1;
}