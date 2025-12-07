# Save File Migration Guide

## Overview
The leveling system update (v2.0) adds a new field to the save data. Existing save files will work but need initialization.

## New SaveData Field
```csharp
public int playerXPToNextLevel = 10; // New field added
```

## Automatic Migration

The system automatically handles old save files:

1. **Loading Old Saves**: 
   - `playerXPToNextLevel` defaults to 10 if not in save file
   - BattleSystem recalculates correct value on first battle

2. **First Battle After Update**:
   - Player unit loads with `xpToNextLevel = 10` (default)
   - On victory, system saves correct `playerXPToNextLevel`
   - All future loads will have correct value

## Manual Fix (Optional)

If you want to fix an existing save immediately:

1. **Locate save file**: 
   - Windows: `%USERPROFILE%\AppData\LocalLow\<CompanyName>\Dungeon Floor\save.json`
   - Mac: `~/Library/Application Support/<CompanyName>/Dungeon Floor/save.json`

2. **Edit save.json** to add the field:

```json
{
  "playerX": 0.0,
  "playerY": 0.0,
  "playerLevel": 3,
  "playerXP": 25,
  "playerXPToNextLevel": 52,  // ← Add this line (calculate using formula)
  "playerMaxHP": 30,
  "playerDamage": 9,
  "playerCurrentHP": 30,
  "deadEnemies": ["enemy1", "enemy2"]
}
```

3. **Calculate correct XPToNextLevel**:
   - Formula: `10 × level^1.5` (rounded)
   - Level 1: 10
   - Level 2: 28
   - Level 3: 52
   - Level 4: 80
   - Level 5: 112

## Verification

After loading a save, check console logs for level-up messages to verify system is working correctly.

## No Action Required

**For most users**: No action needed. The system handles migration automatically.

---

**Version**: 2.0  
**Last Updated**: December 7, 2025
