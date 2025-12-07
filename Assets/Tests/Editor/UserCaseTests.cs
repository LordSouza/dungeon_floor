using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    /// <summary>
    /// Testes focados em cenários reais de uso do jogador
    /// </summary>
    public class UserCaseTests
    {
        private string testSavePath;
        
        [SetUp]
        public void Setup()
        {
            testSavePath = Application.persistentDataPath + "/test_usercase.json";
            if (System.IO.File.Exists(testSavePath))
                System.IO.File.Delete(testSavePath);
        }
        
        [TearDown]
        public void Teardown()
        {
            if (System.IO.File.Exists(testSavePath))
                System.IO.File.Delete(testSavePath);
        }
        
        #region Cenários de Combate
        
        [Test]
        public void UserCase_FirstBattle_PlayerWins()
        {
            // Cenário: Jogador nível 1 enfrenta inimigo nível 1 pela primeira vez
            // Arrange
            GameObject playerGo = new GameObject();
            Unit player = playerGo.AddComponent<Unit>();
            player.unitLevel = 1;
            player.MaxHp = 20;
            player.currentHp = 20;
            player.damage = 5;
            player.xpToNextLevel = 10;
            player.currentXP = 0;
            
            GameObject enemyGo = new GameObject();
            Unit enemy = enemyGo.AddComponent<Unit>();
            enemy.InitializeEnemy(1); // Level 1: 16 HP, 5 damage
            
            // Act - Simular combate
            bool playerTurn = true;
            while (player.currentHp > 0 && enemy.currentHp > 0)
            {
                if (playerTurn)
                {
                    bool enemyDied = enemy.TakeDamage(player.damage);
                    if (enemyDied) break;
                }
                else
                {
                    player.TakeDamage(enemy.damage);
                }
                playerTurn = !playerTurn;
            }
            
            // Assert
            Assert.Greater(player.currentHp, 0, "Player should survive first battle");
            Assert.LessOrEqual(enemy.currentHp, 0, "Enemy should be defeated");
            
            // Cleanup
            Object.DestroyImmediate(playerGo);
            Object.DestroyImmediate(enemyGo);
        }
        
        [Test]
        public void UserCase_PlayerLevelUp_DuringBattle()
        {
            // Cenário: Jogador está no limite do level up e derrota inimigo
            // Arrange
            GameObject playerGo = new GameObject();
            Unit player = playerGo.AddComponent<Unit>();
            player.unitLevel = 1;
            player.MaxHp = 20;
            player.currentHp = 15; // Danificado
            player.damage = 5;
            player.xpToNextLevel = 10;
            player.currentXP = 8; // Precisa de 2 XP para upar
            
            int hpBeforeLevelUp = player.MaxHp;
            
            // Act - Ganhar 5 XP (de inimigo level 1)
            player.GainXP(5);
            
            // Assert
            Assert.AreEqual(2, player.unitLevel, "Player should reach level 2");
            Assert.AreEqual(3, player.currentXP, "Should have 3 XP remaining (8+5-10)");
            Assert.Greater(player.MaxHp, hpBeforeLevelUp, "Max HP should increase");
            Assert.AreEqual(player.MaxHp, player.currentHp, "Should be fully healed after level up");
            
            // Cleanup
            Object.DestroyImmediate(playerGo);
        }
        
        [Test]
        public void UserCase_PlayerDies_HPGoesToZero()
        {
            // Cenário: Jogador toma dano fatal
            // Arrange
            GameObject playerGo = new GameObject();
            Unit player = playerGo.AddComponent<Unit>();
            player.currentHp = 5;
            
            // Act
            bool died = player.TakeDamage(10);
            
            // Assert
            Assert.IsTrue(died, "Player should die from fatal damage");
            Assert.LessOrEqual(player.currentHp, 0, "HP should be 0 or negative");
            
            // Cleanup
            Object.DestroyImmediate(playerGo);
        }
        
        [Test]
        public void UserCase_HealingDoesNotOverheal()
        {
            // Cenário: Jogador tenta se curar além do HP máximo
            // Arrange
            GameObject playerGo = new GameObject();
            Unit player = playerGo.AddComponent<Unit>();
            player.MaxHp = 20;
            player.currentHp = 18;
            
            // Act
            player.Heal(10); // Tentar curar 10 HP quando faltam apenas 2
            
            // Assert
            Assert.AreEqual(20, player.currentHp, "HP should cap at MaxHp");
            
            // Cleanup
            Object.DestroyImmediate(playerGo);
        }
        
        [Test]
        public void UserCase_FightingStrongerEnemy_MoreXP()
        {
            // Cenário: Jogador nível 3 derrota inimigo nível 5
            // Arrange
            GameObject battleGo = new GameObject();
            BattleSystem battle = battleGo.AddComponent<BattleSystem>();
            battle.dialogueText = new GameObject().AddComponent<TMPro.TextMeshProUGUI>();
            battle.audioSource = battleGo.AddComponent<AudioSource>();
            battle.playerButtonsPanel = new GameObject();
            
            var method = typeof(BattleSystem).GetMethod("CalculateXPReward", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Act
            int xpFromSameLevel = (int)method.Invoke(battle, new object[] { 3, 3 });
            int xpFromStronger = (int)method.Invoke(battle, new object[] { 3, 5 });
            
            // Assert
            Assert.Greater(xpFromStronger, xpFromSameLevel, "Stronger enemy should give more XP");
            Assert.GreaterOrEqual(xpFromStronger, xpFromSameLevel * 1.2f, "Should get at least 20% bonus");
            
            // Cleanup
            Object.DestroyImmediate(battle.dialogueText.gameObject);
            Object.DestroyImmediate(battle.playerButtonsPanel);
            Object.DestroyImmediate(battleGo);
        }
        
        #endregion
        
        #region Cenários de Progressão
        
        [Test]
        public void UserCase_ReachLevel5_GetsMilestoneBonus()
        {
            // Cenário: Jogador sobe do nível 4 para 5 e recebe bônus especial
            // Arrange
            GameObject playerGo = new GameObject();
            Unit player = playerGo.AddComponent<Unit>();
            player.unitLevel = 4;
            player.xpToNextLevel = 55;
            player.MaxHp = 40;
            player.currentHp = 40;
            player.damage = 13;
            
            int hpBefore = player.MaxHp;
            int damageBefore = player.damage;
            
            // Act
            player.GainXP(55); // Upar para level 5
            
            // Assert
            Assert.AreEqual(5, player.unitLevel);
            // Base: +5 HP, +2 damage
            // Scaling: +1 HP (5/5), +0 damage (5/10)
            // Milestone: +10 HP, +3 damage
            Assert.AreEqual(hpBefore + 16, player.MaxHp, "Should get 16 HP (5+1+10)");
            Assert.AreEqual(damageBefore + 5, player.damage, "Should get 5 damage (2+0+3)");
            
            // Cleanup
            Object.DestroyImmediate(playerGo);
        }
        
        [Test]
        public void UserCase_MultipleKills_GraduallLevelUp()
        {
            // Cenário: Jogador mata 5 inimigos consecutivos
            // Arrange
            GameObject playerGo = new GameObject();
            Unit player = playerGo.AddComponent<Unit>();
            player.unitLevel = 1;
            player.xpToNextLevel = 10;
            player.currentXP = 0;
            player.MaxHp = 20;
            player.currentHp = 20;
            player.damage = 5;
            
            // Act - Matar 5 inimigos (5 XP cada)
            for (int i = 0; i < 5; i++)
            {
                player.GainXP(5);
                player.currentHp = player.MaxHp; // Simular cura
            }
            
            // Assert
            Assert.GreaterOrEqual(player.unitLevel, 2, "Should level up at least once");
            Assert.Greater(player.MaxHp, 20, "Stats should have improved");
            
            // Cleanup
            Object.DestroyImmediate(playerGo);
        }
        
        [Test]
        public void UserCase_LowLevelGrinding_ReducedXP()
        {
            // Cenário: Jogador nível alto matando inimigos fracos (evitar grinding)
            // Arrange
            GameObject battleGo = new GameObject();
            BattleSystem battle = battleGo.AddComponent<BattleSystem>();
            battle.dialogueText = new GameObject().AddComponent<TMPro.TextMeshProUGUI>();
            battle.audioSource = battleGo.AddComponent<AudioSource>();
            battle.playerButtonsPanel = new GameObject();
            
            var method = typeof(BattleSystem).GetMethod("CalculateXPReward", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Act
            int xpFromWeakEnemy = (int)method.Invoke(battle, new object[] { 10, 1 });
            int xpFromSameLevel = (int)method.Invoke(battle, new object[] { 10, 10 });
            
            // Assert
            Assert.Less(xpFromWeakEnemy, xpFromSameLevel, "Weak enemy should give less XP");
            Assert.LessOrEqual(xpFromWeakEnemy, xpFromSameLevel * 0.6f, "Should get 60% or less XP");
            Assert.GreaterOrEqual(xpFromWeakEnemy, 1, "Should always get at least 1 XP");
            
            // Cleanup
            Object.DestroyImmediate(battle.dialogueText.gameObject);
            Object.DestroyImmediate(battle.playerButtonsPanel);
            Object.DestroyImmediate(battleGo);
        }
        
        #endregion
        
        #region Cenários de Save/Load
        
        [Test]
        public void UserCase_SaveGameAfterBattle_LoadLater()
        {
            // Cenário: Jogador salva o jogo após uma batalha e carrega depois
            // Arrange
            GameObject gmGo = new GameObject();
            GameManager gm = gmGo.AddComponent<GameManager>();
            var savePathField = typeof(GameManager).GetField("_savePath", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            savePathField.SetValue(gm, testSavePath);
            
            // Act - Salvar dados após batalha
            gm.data = new SaveData();
            gm.data.playerLevel = 3;
            gm.data.playerXP = 15;
            gm.data.playerMaxHP = 30;
            gm.data.playerCurrentHP = 25;
            gm.data.playerDamage = 9;
            gm.data.playerX = 42.5f;
            gm.data.playerY = 18.3f;
            gm.data.hasSpawnedOnce = true;
            gm.Save();
            
            // Simular fechar e reabrir o jogo
            gm.data = null;
            gm.Load();
            
            // Assert
            Assert.AreEqual(3, gm.data.playerLevel);
            Assert.AreEqual(15, gm.data.playerXP);
            Assert.AreEqual(30, gm.data.playerMaxHP);
            Assert.AreEqual(25, gm.data.playerCurrentHP);
            Assert.AreEqual(9, gm.data.playerDamage);
            Assert.AreEqual(42.5f, gm.data.playerX);
            Assert.AreEqual(18.3f, gm.data.playerY);
            Assert.IsTrue(gm.data.hasSpawnedOnce);
            
            // Cleanup
            Object.DestroyImmediate(gmGo);
        }
        
        [Test]
        public void UserCase_StartNewGame_ResetsProgress()
        {
            // Cenário: Jogador reinicia o jogo do zero
            // Arrange
            GameObject gmGo = new GameObject();
            GameManager gm = gmGo.AddComponent<GameManager>();
            var savePathField = typeof(GameManager).GetField("_savePath", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            savePathField.SetValue(gm, testSavePath);
            
            // Simular jogo antigo
            gm.data = new SaveData();
            gm.data.playerLevel = 10;
            gm.data.playerXP = 500;
            gm.data.deadEnemies.Add("enemy_1");
            gm.Save();
            
            // Act - Resetar save
            gm.ResetSave();
            
            // Assert
            Assert.AreEqual(1, gm.data.playerLevel, "Should reset to level 1");
            Assert.AreEqual(0, gm.data.playerXP, "Should reset XP");
            Assert.AreEqual(20, gm.data.playerMaxHP, "Should reset HP");
            Assert.AreEqual(5, gm.data.playerDamage, "Should reset damage");
            Assert.AreEqual(0, gm.data.deadEnemies.Count, "Should reset dead enemies list");
            
            // Cleanup
            Object.DestroyImmediate(gmGo);
        }
        
        #endregion
        
        #region Cenários de Respawn de Inimigos
        
        [Test]
        public void UserCase_KillEnemy_ThenRespawns()
        {
            // Cenário: Jogador mata inimigo, ele reaparece após 1 scene load
            // Arrange
            SaveData data = new SaveData();
            data.totalSceneLoads = 0;
            data.respawnAfterSceneLoads = 1;
            
            // Act - Matar inimigo
            var record = new EnemyDeathRecord("goblin_1", data.totalSceneLoads, 10f, 5f);
            data.enemyDeathRecords.Add(record);
            
            // Carregar cena (incrementa contador)
            data.totalSceneLoads++;
            
            // Verificar se deve respawnar
            int scenesSinceDeath = data.totalSceneLoads - record.sceneLoadsAtDeath;
            bool shouldRespawn = scenesSinceDeath >= data.respawnAfterSceneLoads;
            
            // Assert
            Assert.IsTrue(shouldRespawn, "Enemy should respawn after 1 scene load");
            Assert.AreEqual(1, scenesSinceDeath);
            
            // Cleanup after respawn
            data.enemyDeathRecords.Remove(record);
            Assert.AreEqual(0, data.enemyDeathRecords.Count, "Record should be removed after respawn");
        }
        
        [Test]
        public void UserCase_MultipleDeaths_TracksCorrectly()
        {
            // Cenário: Jogador mata o mesmo inimigo múltiplas vezes
            // Arrange
            SaveData data = new SaveData();
            data.totalSceneLoads = 0;
            
            // Act - Primeira morte
            var record = new EnemyDeathRecord("boss_1", data.totalSceneLoads);
            data.enemyDeathRecords.Add(record);
            Assert.AreEqual(1, record.deathCount);
            
            // Respawn acontece
            data.totalSceneLoads += 2;
            data.enemyDeathRecords.Clear();
            
            // Segunda morte do mesmo inimigo
            var existingRecord = data.enemyDeathRecords.Find(r => r.enemyId == "boss_1");
            if (existingRecord != null)
            {
                existingRecord.deathCount++;
                existingRecord.sceneLoadsAtDeath = data.totalSceneLoads;
            }
            else
            {
                var newRecord = new EnemyDeathRecord("boss_1", data.totalSceneLoads);
                data.enemyDeathRecords.Add(newRecord);
            }
            
            // Assert
            Assert.AreEqual(1, data.enemyDeathRecords.Count);
            Assert.AreEqual(1, data.enemyDeathRecords[0].deathCount, "New record starts at 1");
        }
        
        #endregion
        
        #region Cenários de Itens (Fishing)
        
        [Test]
        public void UserCase_CatchFish_AddsToInventory()
        {
            // Cenário: Jogador pesca e ganha um peixe
            // Arrange
            GameObject gmGo = new GameObject();
            GameManager gm = gmGo.AddComponent<GameManager>();
            GameManager.Instance = gm;
            gm.data = new SaveData();
            gm.data.fishCount = 0;
            
            var savePathField = typeof(GameManager).GetField("_savePath", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            savePathField.SetValue(gm, testSavePath);
            
            // Act - Pescar
            gm.data.fishCount++;
            gm.Save();
            
            // Assert
            Assert.AreEqual(1, gm.data.fishCount);
            
            // Cleanup
            Object.DestroyImmediate(gmGo);
        }
        
        [Test]
        public void UserCase_UseFishInBattle_HealsAndConsumes()
        {
            // Cenário: Jogador usa peixe durante batalha para se curar
            // Arrange
            GameObject playerGo = new GameObject();
            Unit player = playerGo.AddComponent<Unit>();
            player.MaxHp = 50;
            player.currentHp = 20; // Baixo HP
            
            GameObject gmGo = new GameObject();
            GameManager gm = gmGo.AddComponent<GameManager>();
            GameManager.Instance = gm;
            gm.data = new SaveData();
            gm.data.fishCount = 3; // Tem 3 peixes
            
            var savePathField = typeof(GameManager).GetField("_savePath", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            savePathField.SetValue(gm, testSavePath);
            
            // Act - Usar peixe (cura 12 HP)
            player.Heal(12);
            gm.data.fishCount--;
            gm.Save();
            
            // Assert
            Assert.AreEqual(32, player.currentHp, "Should heal 12 HP (20+12)");
            Assert.AreEqual(2, gm.data.fishCount, "Should consume 1 fish");
            
            // Cleanup
            Object.DestroyImmediate(playerGo);
            Object.DestroyImmediate(gmGo);
        }
        
        [Test]
        public void UserCase_TryUseItem_WhenNoItems()
        {
            // Cenário: Jogador tenta usar item mas não tem nenhum
            // Arrange
            SaveData data = new SaveData();
            data.fishCount = 0;
            
            // Act & Assert
            Assert.AreEqual(0, data.fishCount);
            
            // Simular tentativa de usar (deveria falhar silenciosamente)
            if (data.fishCount > 0)
            {
                data.fishCount--;
            }
            
            Assert.AreEqual(0, data.fishCount, "Count should remain 0");
        }
        
        [Test]
        public void UserCase_CollectMultipleFish_Accumulates()
        {
            // Cenário: Jogador pesca várias vezes
            // Arrange
            SaveData data = new SaveData();
            data.fishCount = 0;
            
            // Act - Pescar 5 vezes
            for (int i = 0; i < 5; i++)
            {
                data.fishCount++;
            }
            
            // Assert
            Assert.AreEqual(5, data.fishCount);
            
            // Usar 2
            data.fishCount -= 2;
            Assert.AreEqual(3, data.fishCount);
        }
        
        #endregion
        
        #region Cenários de Edge Cases
        
        [Test]
        public void UserCase_LevelUpMultipleTimes_SingleXPGain()
        {
            // Cenário: Jogador ganha XP suficiente para subir múltiplos níveis de uma vez
            // Arrange
            GameObject playerGo = new GameObject();
            Unit player = playerGo.AddComponent<Unit>();
            player.unitLevel = 1;
            player.xpToNextLevel = 10;
            player.currentXP = 0;
            player.MaxHp = 20;
            player.currentHp = 20;
            player.damage = 5;
            
            // Act - Ganhar 100 XP de uma vez (vários levels)
            player.GainXP(100);
            
            // Assert
            Assert.GreaterOrEqual(player.unitLevel, 4, "Should level up multiple times");
            Assert.AreEqual(player.MaxHp, player.currentHp, "Should be fully healed");
            
            // Cleanup
            Object.DestroyImmediate(playerGo);
        }
        
        [Test]
        public void UserCase_DamageExceedsCurrentHP_Overkill()
        {
            // Cenário: Jogador toma dano muito maior que HP atual
            // Arrange
            GameObject playerGo = new GameObject();
            Unit player = playerGo.AddComponent<Unit>();
            player.currentHp = 5;
            
            // Act
            bool died = player.TakeDamage(999);
            
            // Assert
            Assert.IsTrue(died);
            Assert.Less(player.currentHp, 0, "HP can go negative");
            
            // Cleanup
            Object.DestroyImmediate(playerGo);
        }
        
        [Test]
        public void UserCase_EnemyScaling_HighLevel()
        {
            // Cenário: Verificar que inimigos de alto nível têm stats apropriados
            // Arrange
            GameObject enemyGo = new GameObject();
            Unit enemy = enemyGo.AddComponent<Unit>();
            
            // Act
            enemy.InitializeEnemy(20); // Level 20 enemy
            
            // Assert
            // Formula: MaxHp = 10 + (level * 6), damage = 3 + (level * 2)
            Assert.AreEqual(130, enemy.MaxHp, "Level 20 should have 130 HP");
            Assert.AreEqual(43, enemy.damage, "Level 20 should have 43 damage");
            
            // Cleanup
            Object.DestroyImmediate(enemyGo);
        }
        
        [Test]
        public void UserCase_ZeroHPExactly_StillDies()
        {
            // Cenário: HP chega exatamente a zero
            // Arrange
            GameObject playerGo = new GameObject();
            Unit player = playerGo.AddComponent<Unit>();
            player.currentHp = 10;
            
            // Act
            bool died = player.TakeDamage(10); // Exactly 10 damage
            
            // Assert
            Assert.IsTrue(died, "Should die when HP reaches exactly 0");
            Assert.AreEqual(0, player.currentHp);
            
            // Cleanup
            Object.DestroyImmediate(playerGo);
        }
        
        #endregion
    }
}
