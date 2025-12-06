using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public float playerX;
    public float playerY;

    public string lastEnemyID;
    public bool hasSpawnedOnce;

    public List<string> deadEnemies = new List<string>();

    // player stats
    public int playerLevel = 1;
    public int playerXP = 0;
    public int playerMaxHP = 20;
    public int playerDamage = 5;
    
    // inimigo stats
    public int currentEnemyLevel = 1;
}