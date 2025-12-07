using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public class SaveDataTests
    {
        [Test]
        public void EnemyDeathRecord_Constructor_InitializesCorrectly()
        {
            // Arrange & Act
            var record = new EnemyDeathRecord("enemy_1", 5, 10f, 20f);
            
            // Assert
            Assert.AreEqual("enemy_1", record.enemyId);
            Assert.AreEqual(1, record.deathCount);
            Assert.AreEqual(5, record.sceneLoadsAtDeath);
            Assert.AreEqual(10f, record.originalX);
            Assert.AreEqual(20f, record.originalY);
        }
        
        [Test]
        public void EnemySpawnPoint_Constructor_InitializesCorrectly()
        {
            // Arrange & Act
            var spawnPoint = new EnemySpawnPoint(15f, 25f);
            
            // Assert
            Assert.AreEqual(15f, spawnPoint.x);
            Assert.AreEqual(25f, spawnPoint.y);
        }
        
        [Test]
        public void SaveData_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var data = new SaveData();
            
            // Assert
            Assert.AreEqual(1, data.playerLevel, "Default player level should be 1");
            Assert.AreEqual(0, data.playerXP, "Default XP should be 0");
            Assert.AreEqual(10, data.playerXPToNextLevel, "Default XP to next level should be 10");
            Assert.AreEqual(20, data.playerMaxHP, "Default max HP should be 20");
            Assert.AreEqual(20, data.playerCurrentHP, "Default current HP should be 20");
            Assert.AreEqual(5, data.playerDamage, "Default damage should be 5");
            Assert.AreEqual(0, data.fishCount, "Default fish count should be 0");
            Assert.AreEqual(1, data.respawnAfterSceneLoads, "Default respawn threshold should be 1");
            Assert.IsTrue(data.enableRandomSpawns, "Random spawns should be enabled by default");
            Assert.IsNotNull(data.deadEnemies, "Dead enemies list should be initialized");
            Assert.IsNotNull(data.enemyDeathRecords, "Enemy death records should be initialized");
            Assert.IsNotNull(data.enemySpawnPoints, "Enemy spawn points should be initialized");
        }
        
        [Test]
        public void SaveData_EnemyDeathRecords_CanAddAndFind()
        {
            // Arrange
            var data = new SaveData();
            var record1 = new EnemyDeathRecord("enemy_1", 5);
            var record2 = new EnemyDeathRecord("enemy_2", 10);
            
            // Act
            data.enemyDeathRecords.Add(record1);
            data.enemyDeathRecords.Add(record2);
            
            var found = data.enemyDeathRecords.Find(r => r.enemyId == "enemy_1");
            
            // Assert
            Assert.AreEqual(2, data.enemyDeathRecords.Count);
            Assert.IsNotNull(found);
            Assert.AreEqual("enemy_1", found.enemyId);
        }
        
        [Test]
        public void SaveData_SpawnPoints_CanAddMultiple()
        {
            // Arrange
            var data = new SaveData();
            
            // Act
            data.enemySpawnPoints.Add(new EnemySpawnPoint(10f, 20f));
            data.enemySpawnPoints.Add(new EnemySpawnPoint(30f, 40f));
            data.enemySpawnPoints.Add(new EnemySpawnPoint(50f, 60f));
            
            // Assert
            Assert.AreEqual(3, data.enemySpawnPoints.Count);
            Assert.AreEqual(10f, data.enemySpawnPoints[0].x);
            Assert.AreEqual(60f, data.enemySpawnPoints[2].y);
        }
    }
}
