using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class GameManagerTests
    {
        private string testSavePath;
        
        [SetUp]
        public void Setup()
        {
            // Create a test save path
            testSavePath = Application.persistentDataPath + "/test_save.json";
            
            // Clean up any existing test save
            if (System.IO.File.Exists(testSavePath))
            {
                System.IO.File.Delete(testSavePath);
            }
        }
        
        [TearDown]
        public void Teardown()
        {
            // Clean up test save file
            if (System.IO.File.Exists(testSavePath))
            {
                System.IO.File.Delete(testSavePath);
            }
        }
        
        [Test]
        public void ResetSave_InitializesDefaultValues()
        {
            // Arrange
            GameObject go = new GameObject();
            GameManager gm = go.AddComponent<GameManager>();
            
            // Act
            gm.ResetSave();
            
            // Assert
            Assert.IsNotNull(gm.data);
            Assert.AreEqual(1, gm.data.playerLevel);
            Assert.AreEqual(10, gm.data.playerXPToNextLevel, "XP requirement should be calculated correctly");
            Assert.AreEqual(20, gm.data.playerMaxHP);
            Assert.AreEqual(5, gm.data.playerDamage);
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void CalculateXPRequirement_MatchesUnitCalculation()
        {
            // Arrange
            GameObject gmGo = new GameObject();
            GameManager gm = gmGo.AddComponent<GameManager>();
            
            GameObject unitGo = new GameObject();
            Unit unit = unitGo.AddComponent<Unit>();
            
            // Use reflection to access private method in GameManager
            var gmMethod = typeof(GameManager).GetMethod("CalculateXPRequirement", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Act & Assert
            for (int level = 1; level <= 10; level++)
            {
                int gmXP = (int)gmMethod.Invoke(gm, new object[] { level });
                int unitXP = unit.CalculateXPRequirement(level);
                
                Assert.AreEqual(unitXP, gmXP, $"GameManager and Unit XP calculation should match for level {level}");
            }
            
            // Cleanup
            Object.DestroyImmediate(unitGo);
            Object.DestroyImmediate(gmGo);
        }
        
        [Test]
        public void MarkEnemyAsDead_AddsToDeadEnemiesList()
        {
            // Arrange
            GameObject go = new GameObject();
            GameManager gm = go.AddComponent<GameManager>();
            gm.data = new SaveData();
            
            // Act
            gm.MarkEnemyAsDead("enemy_1");
            gm.MarkEnemyAsDead("enemy_2");
            
            // Assert
            Assert.AreEqual(2, gm.data.deadEnemies.Count);
            Assert.Contains("enemy_1", gm.data.deadEnemies);
            Assert.Contains("enemy_2", gm.data.deadEnemies);
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void MarkEnemyAsDead_DoesNotAddDuplicates()
        {
            // Arrange
            GameObject go = new GameObject();
            GameManager gm = go.AddComponent<GameManager>();
            gm.data = new SaveData();
            
            // Act
            gm.MarkEnemyAsDead("enemy_1");
            gm.MarkEnemyAsDead("enemy_1"); // Try to add duplicate
            
            // Assert
            Assert.AreEqual(1, gm.data.deadEnemies.Count, "Should not add duplicate enemy IDs");
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
    }
}
