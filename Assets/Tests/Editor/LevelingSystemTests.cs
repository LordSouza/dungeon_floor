using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class LevelingSystemTests
    {
        [Test]
        public void CalculateXPRequirement_Level1_Returns10()
        {
            // Arrange
            GameObject go = new GameObject();
            Unit unit = go.AddComponent<Unit>();
            
            // Act
            int xpRequired = unit.CalculateXPRequirement(1);
            
            // Assert
            Assert.AreEqual(10, xpRequired, "Level 1->2 should require 10 XP");
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void CalculateXPRequirement_Level5_Returns67()
        {
            // Arrange
            GameObject go = new GameObject();
            Unit unit = go.AddComponent<Unit>();
            
            // Act
            int xpRequired = unit.CalculateXPRequirement(5);
            
            // Assert
            Assert.AreEqual(67, xpRequired, "Level 5->6 should require 67 XP (10 * 5^1.2)");
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void CalculateXPRequirement_Level10_Returns158()
        {
            // Arrange
            GameObject go = new GameObject();
            Unit unit = go.AddComponent<Unit>();
            
            // Act
            int xpRequired = unit.CalculateXPRequirement(10);
            
            // Assert
            Assert.AreEqual(158, xpRequired, "Level 10->11 should require 158 XP");
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void GainXP_EnoughForOneLevel_IncreasesLevel()
        {
            // Arrange
            GameObject go = new GameObject();
            Unit unit = go.AddComponent<Unit>();
            unit.unitLevel = 1;
            unit.xpToNextLevel = 10;
            unit.MaxHp = 20;
            unit.currentHp = 20;
            unit.damage = 5;
            
            // Act
            unit.GainXP(10);
            
            // Assert
            Assert.AreEqual(2, unit.unitLevel, "Unit should be level 2");
            Assert.AreEqual(0, unit.currentXP, "Current XP should be 0 after exact level up");
            Assert.Greater(unit.MaxHp, 20, "Max HP should increase after level up");
            Assert.Greater(unit.damage, 5, "Damage should increase after level up");
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void GainXP_MultipleLevels_LevelsUpMultipleTimes()
        {
            // Arrange
            GameObject go = new GameObject();
            Unit unit = go.AddComponent<Unit>();
            unit.unitLevel = 1;
            unit.xpToNextLevel = 10;
            unit.MaxHp = 20;
            unit.currentHp = 20;
            unit.damage = 5;
            
            // Act - Give enough XP for multiple levels (10 + 23 = 33 XP for level 1->3)
            unit.GainXP(50);
            
            // Assert
            Assert.GreaterOrEqual(unit.unitLevel, 3, "Unit should be at least level 3");
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void LevelUp_AtLevel5_AppliesMilestoneBonus()
        {
            // Arrange
            GameObject go = new GameObject();
            Unit unit = go.AddComponent<Unit>();
            unit.unitLevel = 4;
            unit.xpToNextLevel = 52; // Level 4->5 requires 52 XP
            unit.MaxHp = 40;
            unit.currentHp = 40;
            unit.damage = 13;
            
            int hpBefore = unit.MaxHp;
            int damageBefore = unit.damage;
            
            // Act
            unit.GainXP(52);
            
            // Assert
            Assert.AreEqual(5, unit.unitLevel, "Unit should be level 5");
            // Base gain: +5 HP, +2 damage
            // Scaling: +1 HP (level 5/5), +0 damage (level 5/10)
            // Milestone: +10 HP, +3 damage
            // Total: +16 HP, +5 damage
            Assert.AreEqual(hpBefore + 16, unit.MaxHp, "Should gain 16 HP at level 5 (5 base + 1 scaling + 10 milestone)");
            Assert.AreEqual(damageBefore + 5, unit.damage, "Should gain 5 damage at level 5 (2 base + 0 scaling + 3 milestone)");
            Assert.AreEqual(unit.MaxHp, unit.currentHp, "Should be fully healed after level up");
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void LevelUp_FullHealsPlayer()
        {
            // Arrange
            GameObject go = new GameObject();
            Unit unit = go.AddComponent<Unit>();
            unit.unitLevel = 1;
            unit.xpToNextLevel = 10;
            unit.MaxHp = 20;
            unit.currentHp = 10; // Damaged
            unit.damage = 5;
            
            // Act
            unit.GainXP(10);
            
            // Assert
            Assert.AreEqual(unit.MaxHp, unit.currentHp, "Player should be fully healed after level up");
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void InitializeEnemy_Level3_ScalesCorrectly()
        {
            // Arrange
            GameObject go = new GameObject();
            Unit unit = go.AddComponent<Unit>();
            
            // Act
            unit.InitializeEnemy(3);
            
            // Assert
            Assert.AreEqual(3, unit.unitLevel, "Enemy should be level 3");
            Assert.AreEqual(28, unit.MaxHp, "Enemy HP should be 10 + (3 * 6) = 28");
            Assert.AreEqual(9, unit.damage, "Enemy damage should be 3 + (3 * 2) = 9");
            Assert.AreEqual(unit.MaxHp, unit.currentHp, "Enemy should start at full HP");
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void TakeDamage_ReducesHP_ReturnsCorrectStatus()
        {
            // Arrange
            GameObject go = new GameObject();
            Unit unit = go.AddComponent<Unit>();
            unit.currentHp = 20;
            
            // Act
            bool isDead1 = unit.TakeDamage(10);
            bool isDead2 = unit.TakeDamage(15);
            
            // Assert
            Assert.IsFalse(isDead1, "Unit should not be dead after taking 10 damage from 20 HP");
            Assert.IsTrue(isDead2, "Unit should be dead after taking 15 more damage");
            Assert.LessOrEqual(unit.currentHp, 0, "HP should be 0 or negative");
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void Heal_IncreasesHP_DoesNotExceedMax()
        {
            // Arrange
            GameObject go = new GameObject();
            Unit unit = go.AddComponent<Unit>();
            unit.MaxHp = 20;
            unit.currentHp = 10;
            
            // Act
            unit.Heal(5);
            int hpAfterNormalHeal = unit.currentHp;
            
            unit.Heal(20); // Try to overheal
            
            // Assert
            Assert.AreEqual(15, hpAfterNormalHeal, "HP should increase by 5");
            Assert.AreEqual(20, unit.currentHp, "HP should not exceed MaxHp");
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
    }
}
