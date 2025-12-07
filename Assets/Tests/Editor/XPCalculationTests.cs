using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class XPCalculationTests
    {
        private GameObject CreateBattleSystem()
        {
            GameObject go = new GameObject();
            BattleSystem battleSystem = go.AddComponent<BattleSystem>();
            
            // Initialize required components to avoid null references
            battleSystem.dialogueText = new GameObject().AddComponent<TMPro.TextMeshProUGUI>();
            battleSystem.audioSource = go.AddComponent<AudioSource>();
            battleSystem.playerButtonsPanel = new GameObject();
            
            return go;
        }
        
        [Test]
        public void CalculateXPReward_SameLevel_ReturnsBaseXP()
        {
            // Arrange
            GameObject go = CreateBattleSystem();
            BattleSystem battleSystem = go.GetComponent<BattleSystem>();
            
            // Use reflection to access private method
            var method = typeof(BattleSystem).GetMethod("CalculateXPReward", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Act
            int xp = (int)method.Invoke(battleSystem, new object[] { 5, 5 });
            
            // Assert
            Assert.AreEqual(25, xp, "Same level enemy should give base XP (5 * 5 = 25)");
            
            // Cleanup
            Object.DestroyImmediate(battleSystem.dialogueText.gameObject);
            Object.DestroyImmediate(battleSystem.playerButtonsPanel);
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void CalculateXPReward_EnemyHigherLevel_GivesBonusXP()
        {
            // Arrange
            GameObject go = CreateBattleSystem();
            BattleSystem battleSystem = go.GetComponent<BattleSystem>();
            
            var method = typeof(BattleSystem).GetMethod("CalculateXPReward", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Act - Player level 5, Enemy level 8 (difference: +3)
            int xp = (int)method.Invoke(battleSystem, new object[] { 5, 8 });
            
            // Assert
            // Base: 8 * 5 = 40
            // Multiplier: 1.5 + (3 * 0.1) = 1.8
            // Expected: 40 * 1.8 = 72
            Assert.AreEqual(72, xp, "Enemy 3 levels higher should give 1.8x XP bonus");
            
            // Cleanup
            Object.DestroyImmediate(battleSystem.dialogueText.gameObject);
            Object.DestroyImmediate(battleSystem.playerButtonsPanel);
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void CalculateXPReward_EnemyLowerLevel_GivesReducedXP()
        {
            // Arrange
            GameObject go = CreateBattleSystem();
            BattleSystem battleSystem = go.GetComponent<BattleSystem>();
            
            var method = typeof(BattleSystem).GetMethod("CalculateXPReward", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Act - Player level 8, Enemy level 7 (difference: -1)
            int xp = (int)method.Invoke(battleSystem, new object[] { 8, 7 });
            
            // Assert
            // Base: 7 * 5 = 35
            // Multiplier: 0.8 + (-1 * 0.1) = 0.7
            // Expected: 35 * 0.7 = 24.5 -> 24 (rounded)
            Assert.AreEqual(24, xp, "Enemy 1 level lower should give 0.7x XP reduction");
            
            // Cleanup
            Object.DestroyImmediate(battleSystem.dialogueText.gameObject);
            Object.DestroyImmediate(battleSystem.playerButtonsPanel);
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void CalculateXPReward_EnemyMuchWeaker_GivesMinimalXP()
        {
            // Arrange
            GameObject go = CreateBattleSystem();
            BattleSystem battleSystem = go.GetComponent<BattleSystem>();
            
            var method = typeof(BattleSystem).GetMethod("CalculateXPReward", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Act - Player level 10, Enemy level 1 (difference: -9)
            int xp = (int)method.Invoke(battleSystem, new object[] { 10, 1 });
            
            // Assert
            // Base: 1 * 5 = 5
            // Multiplier: 0.5 (minimum)
            // Expected: 5 * 0.5 = 2.5 -> 2 (rounded)
            Assert.GreaterOrEqual(xp, 1, "XP should never be less than 1");
            Assert.LessOrEqual(xp, 3, "Much weaker enemy should give minimal XP");
            
            // Cleanup
            Object.DestroyImmediate(battleSystem.dialogueText.gameObject);
            Object.DestroyImmediate(battleSystem.playerButtonsPanel);
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void CalculateXPReward_AlwaysReturnsPositiveValue()
        {
            // Arrange
            GameObject go = CreateBattleSystem();
            BattleSystem battleSystem = go.GetComponent<BattleSystem>();
            
            var method = typeof(BattleSystem).GetMethod("CalculateXPReward", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Act & Assert - Test multiple scenarios
            for (int playerLevel = 1; playerLevel <= 10; playerLevel++)
            {
                for (int enemyLevel = 1; enemyLevel <= 10; enemyLevel++)
                {
                    int xp = (int)method.Invoke(battleSystem, new object[] { playerLevel, enemyLevel });
                    Assert.Greater(xp, 0, $"XP should always be positive (Player: {playerLevel}, Enemy: {enemyLevel})");
                }
            }
            
            // Cleanup
            Object.DestroyImmediate(battleSystem.dialogueText.gameObject);
            Object.DestroyImmediate(battleSystem.playerButtonsPanel);
            Object.DestroyImmediate(go);
        }
    }
}
