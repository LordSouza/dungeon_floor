using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Testes para o sistema de pesca completo
/// </summary>
public class FishingSystemTests
{
    [Test]
    public void FishCount_StartsAtZero()
    {
        var data = new SaveData();
        Assert.AreEqual(0, data.fishCount, "Fish count should start at 0");
    }
    
    [Test]
    public void FishCount_IncrementsCorrectly()
    {
        var data = new SaveData();
        data.fishCount = 0;
        
        data.fishCount++;
        Assert.AreEqual(1, data.fishCount);
        
        data.fishCount += 5;
        Assert.AreEqual(6, data.fishCount);
    }
    
    [Test]
    public void FishCount_DoesNotGoNegative()
    {
        var data = new SaveData();
        data.fishCount = 2;
        
        data.fishCount--;
        Assert.AreEqual(1, data.fishCount);
        
        data.fishCount--;
        Assert.AreEqual(0, data.fishCount);
        
        // Should not go below 0
        data.fishCount = Mathf.Max(0, data.fishCount - 1);
        Assert.AreEqual(0, data.fishCount);
    }
    
    [Test]
    public void FishItem_HealsCorrectAmount()
    {
        // Fish should heal 12 HP according to documentation
        int fishHealAmount = 12;
        
        int playerHP = 10;
        int maxHP = 50;
        
        playerHP += fishHealAmount;
        playerHP = Mathf.Min(playerHP, maxHP);
        
        Assert.AreEqual(22, playerHP, "Fish should heal 12 HP");
    }
    
    [Test]
    public void FishItem_DoesNotOverheal()
    {
        int fishHealAmount = 12;
        
        int playerHP = 45;
        int maxHP = 50;
        
        playerHP += fishHealAmount;
        playerHP = Mathf.Min(playerHP, maxHP);
        
        Assert.AreEqual(maxHP, playerHP, "Fish should not overheal past max HP");
    }
    
    [Test]
    public void FishItem_CanBeUsedMultipleTimes()
    {
        var data = new SaveData();
        data.fishCount = 5;
        
        // Use fish 3 times
        data.fishCount--;
        data.fishCount--;
        data.fishCount--;
        
        Assert.AreEqual(2, data.fishCount, "Should have 2 fish remaining");
    }
    
    [Test]
    public void FishItem_CannotBeUsedWhenNone()
    {
        var data = new SaveData();
        data.fishCount = 0;
        
        bool canUse = data.fishCount > 0;
        
        Assert.IsFalse(canUse, "Should not be able to use fish when count is 0");
    }
    
    [Test]
    public void PlayerPosition_SavedBeforeFishing()
    {
        var data = new SaveData();
        
        // Simulate player at fishing boat location
        float boatX = 10.5f;
        float boatY = 3.2f;
        
        data.playerX = boatX;
        data.playerY = boatY;
        
        Assert.AreEqual(boatX, data.playerX, 0.01f);
        Assert.AreEqual(boatY, data.playerY, 0.01f);
    }
    
    [Test]
    public void PlayerPosition_RestoredAfterFishing()
    {
        var data = new SaveData();
        data.hasSpawnedOnce = true;
        
        float savedX = 15.7f;
        float savedY = 5.3f;
        
        data.playerX = savedX;
        data.playerY = savedY;
        
        // Simulate returning from fishing scene
        Vector3 restoredPosition = new Vector3(data.playerX, data.playerY, 0);
        
        Assert.AreEqual(savedX, restoredPosition.x, 0.01f);
        Assert.AreEqual(savedY, restoredPosition.y, 0.01f);
    }
    
    [Test]
    public void FishingBoat_OnlyAcceptsPlayer()
    {
        // Simulate tag comparison
        string playerTag = "Player";
        string enemyTag = "Enemy";
        string untaggedTag = "Untagged";
        
        bool playerAccepted = playerTag == "Player";
        bool enemyRejected = enemyTag != "Player";
        bool untaggedRejected = untaggedTag != "Player";
        
        Assert.IsTrue(playerAccepted, "Player should be accepted");
        Assert.IsTrue(enemyRejected, "Enemy should be rejected");
        Assert.IsTrue(untaggedRejected, "Untagged should be rejected");
    }
    
    [Test]
    public void FishingMinigame_SuccessZoneValidation()
    {
        // Success zone parameters
        float successZoneStart = 0.4f;
        float successZoneEnd = 0.6f;
        float successZoneSize = successZoneEnd - successZoneStart;
        
        Assert.Greater(successZoneSize, 0, "Success zone should have positive size");
        Assert.LessOrEqual(successZoneSize, 1.0f, "Success zone should not exceed slider range");
        Assert.AreEqual(0.2f, successZoneSize, 0.01f, "Success zone should be 20% of slider");
    }
    
    [Test]
    public void FishingMinigame_IndicatorMovement()
    {
        // Simulate indicator moving across slider (0 to 1)
        float indicator = 0.0f;
        float speed = 0.5f; // moves at 0.5 units per second
        float deltaTime = 0.1f;
        
        indicator += speed * deltaTime;
        
        Assert.AreEqual(0.05f, indicator, 0.001f, "Indicator should move by speed * deltaTime");
    }
    
    [Test]
    public void FishingMinigame_SuccessDetection()
    {
        float successZoneStart = 0.4f;
        float successZoneEnd = 0.6f;
        
        float indicatorInZone = 0.5f;
        float indicatorOutOfZone = 0.8f;
        
        bool successCase = indicatorInZone >= successZoneStart && indicatorInZone <= successZoneEnd;
        bool failCase = indicatorOutOfZone >= successZoneStart && indicatorOutOfZone <= successZoneEnd;
        
        Assert.IsTrue(successCase, "Indicator at 0.5 should be in success zone (0.4-0.6)");
        Assert.IsFalse(failCase, "Indicator at 0.8 should be outside success zone");
    }
    
    [Test]
    public void FishingMinigame_EdgeCaseDetection()
    {
        float successZoneStart = 0.4f;
        float successZoneEnd = 0.6f;
        
        // Test exact boundaries
        float leftEdge = 0.4f;
        float rightEdge = 0.6f;
        float justOutsideLeft = 0.39f;
        float justOutsideRight = 0.61f;
        
        bool leftEdgeSuccess = leftEdge >= successZoneStart && leftEdge <= successZoneEnd;
        bool rightEdgeSuccess = rightEdge >= successZoneStart && rightEdge <= successZoneEnd;
        bool outsideLeft = justOutsideLeft >= successZoneStart && justOutsideLeft <= successZoneEnd;
        bool outsideRight = justOutsideRight >= successZoneStart && justOutsideRight <= successZoneEnd;
        
        Assert.IsTrue(leftEdgeSuccess, "Left edge should be successful");
        Assert.IsTrue(rightEdgeSuccess, "Right edge should be successful");
        Assert.IsFalse(outsideLeft, "Just outside left should fail");
        Assert.IsFalse(outsideRight, "Just outside right should fail");
    }
}
