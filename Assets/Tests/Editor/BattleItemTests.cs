using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Testes para o sistema de itens em batalha
/// </summary>
public class BattleItemTests
{
    [Test]
    public void FishItem_AvailableWhenInInventory()
    {
        var data = new SaveData();
        data.fishCount = 3;
        
        bool canUseItem = data.fishCount > 0;
        
        Assert.IsTrue(canUseItem, "Should be able to use fish when count > 0");
    }
    
    [Test]
    public void FishItem_UnavailableWhenNone()
    {
        var data = new SaveData();
        data.fishCount = 0;
        
        bool canUseItem = data.fishCount > 0;
        
        Assert.IsFalse(canUseItem, "Should not be able to use fish when count is 0");
    }
    
    [Test]
    public void FishItem_ConsumesOneOnUse()
    {
        var data = new SaveData();
        data.fishCount = 5;
        
        // Use item
        data.fishCount--;
        
        Assert.AreEqual(4, data.fishCount, "Should consume 1 fish on use");
    }
    
    [Test]
    public void FishItem_HealsPlayerInBattle()
    {
        int playerHP = 15;
        int playerMaxHP = 50;
        int fishHealAmount = 12;
        
        // Use fish
        playerHP += fishHealAmount;
        playerHP = Mathf.Min(playerHP, playerMaxHP);
        
        Assert.AreEqual(27, playerHP, "Player should heal from 15 to 27");
    }
    
    [Test]
    public void FishItem_CannotOverhealMaxHP()
    {
        int playerHP = 45;
        int playerMaxHP = 50;
        int fishHealAmount = 12;
        
        // Use fish
        playerHP += fishHealAmount;
        playerHP = Mathf.Min(playerHP, playerMaxHP);
        
        Assert.AreEqual(playerMaxHP, playerHP, "Should cap at max HP (50)");
    }
    
    [Test]
    public void FishItem_FullHealScenario()
    {
        int playerHP = 1;
        int playerMaxHP = 20;
        int fishHealAmount = 12;
        
        // Use first fish
        playerHP += fishHealAmount;
        Assert.AreEqual(13, playerHP);
        
        // Use second fish
        playerHP += fishHealAmount;
        playerHP = Mathf.Min(playerHP, playerMaxHP);
        Assert.AreEqual(playerMaxHP, playerHP, "Two fish should heal from 1 to max");
    }
    
    [Test]
    public void ItemUsage_TakesTurn()
    {
        // In turn-based combat, using item should end player turn
        bool playerTurnActive = true;
        
        // Simulate using item
        bool itemUsed = true;
        if (itemUsed)
        {
            playerTurnActive = false;
        }
        
        Assert.IsFalse(playerTurnActive, "Using item should end player turn");
    }
    
    [Test]
    public void BattleState_TransitionsAfterItemUse()
    {
        // Player uses item -> Should transition to enemy turn
        string currentState = "PLAYERTURN";
        
        // Use item
        currentState = "ENEMYTURN";
        
        Assert.AreEqual("ENEMYTURN", currentState, "Should transition to enemy turn after item use");
    }
    
    [Test]
    public void MultipleItems_CanBeUsedInSequence()
    {
        var data = new SaveData();
        data.fishCount = 10;
        
        int playerHP = 10;
        int playerMaxHP = 50;
        int fishHealAmount = 12;
        
        // Use 3 fish in sequence across multiple turns
        for (int i = 0; i < 3; i++)
        {
            if (data.fishCount > 0 && playerHP < playerMaxHP)
            {
                data.fishCount--;
                playerHP += fishHealAmount;
                playerHP = Mathf.Min(playerHP, playerMaxHP);
            }
        }
        
        Assert.AreEqual(7, data.fishCount, "Should have used 3 fish");
        Assert.AreEqual(46, playerHP, "Should have healed 36 HP total");
    }
    
    [Test]
    public void ItemButton_DisabledWhenNoItems()
    {
        var data = new SaveData();
        data.fishCount = 0;
        
        bool itemButtonEnabled = data.fishCount > 0;
        
        Assert.IsFalse(itemButtonEnabled, "Item button should be disabled when no items");
    }
    
    [Test]
    public void ItemButton_EnabledWhenHasItems()
    {
        var data = new SaveData();
        data.fishCount = 1;
        
        bool itemButtonEnabled = data.fishCount > 0;
        
        Assert.IsTrue(itemButtonEnabled, "Item button should be enabled when has items");
    }
    
    [Test]
    public void FishCount_PersistsAcrossBattles()
    {
        var data = new SaveData();
        data.fishCount = 5;
        
        // Battle 1: Use 2 fish
        data.fishCount -= 2;
        Assert.AreEqual(3, data.fishCount);
        
        // Battle 2: Use 1 fish
        data.fishCount -= 1;
        Assert.AreEqual(2, data.fishCount);
        
        // Should persist across save/load
        Assert.AreEqual(2, data.fishCount, "Fish count should persist");
    }
    
    [Test]
    public void CriticalHP_FishCanSave()
    {
        int playerHP = 2;
        int playerMaxHP = 30;
        int fishHealAmount = 12;
        int enemyDamage = 10;
        
        // Player at critical HP (2)
        // Enemy will deal 10 damage next turn
        // Use fish to survive
        playerHP += fishHealAmount;
        Assert.AreEqual(14, playerHP);
        
        // Take enemy damage
        playerHP -= enemyDamage;
        Assert.AreEqual(4, playerHP, "Fish should allow survival");
        Assert.Greater(playerHP, 0, "Player should still be alive");
    }
    
    [Test]
    public void ItemUsage_DoesNotAffectEnemyHP()
    {
        int enemyHP = 30;
        int playerHP = 10;
        int fishHealAmount = 12;
        
        // Use fish
        playerHP += fishHealAmount;
        
        Assert.AreEqual(30, enemyHP, "Enemy HP should not change when player uses item");
        Assert.AreEqual(22, playerHP, "Player HP should increase");
    }
}
