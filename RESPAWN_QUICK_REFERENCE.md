# Respawn System - Quick Reference

## How It Works in 30 Seconds

Enemies respawn after **3 scene transitions** back to the map.

```
Fight Enemy → Return to Map (1) → Fight → Return (2) → Fight → Return (3)
→ ENEMY RESPAWNS!
```

## For Players

### Farming XP
- Defeat enemies in an area
- Move to other areas (triggers scene transitions)
- Return after 3 transitions - enemies are back!
- Repeat to grind XP and level up

### Tips
- Don't stay in one area - enemies won't respawn
- Explore different parts of the map
- Each return to map counts as 1 transition
- Great for leveling up when stuck on a tough enemy

## For Developers

### Key Settings

**Respawn Rate** (in SaveData.cs):
```csharp
public int respawnAfterSceneLoads = 3; // Change this!
```

**Common Values:**
- `1` = Respawn immediately (very easy mode)
- `2` = Quick respawn (easy mode)
- `3` = Default (balanced)
- `5` = Slow respawn (hard mode)
- `999` = Essentially no respawn

### Data Structure

```csharp
// Each defeated enemy gets a record
public class EnemyDeathRecord {
    string enemyId;           // e.g., "goblin_01"
    int deathCount;           // How many times defeated
    int sceneLoadsAtDeath;    // When it died
}

// In SaveData
List<EnemyDeathRecord> enemyDeathRecords;
int totalSceneLoads;          // Incremented each map load
```

### Respawn Logic

```
Current Scene Loads - Death Scene Loads >= Respawn Threshold
    ↓
Enemy Respawns
```

Example:
- Enemy died at scene load 5
- Current scene load is 8
- Difference: 8 - 5 = 3
- Threshold: 3
- **3 >= 3 → RESPAWN!**

## Integration with Leveling

The respawn system works perfectly with the new leveling system:

| Feature | Purpose |
|---------|---------|
| Exponential XP | Requires more XP at higher levels |
| Respawning Enemies | Provides sustainable XP source |
| XP Multipliers | Rewards fighting stronger enemies |
| Scene Transition Requirement | Prevents stationary grinding |

### Progression Flow

```
Level 1-5  → Fast leveling, enemies respawn frequently
   ↓
Level 5-10 → Moderate pace, need to explore more
   ↓
Level 10+  → Grind required, utilize full respawn cycle
```

## Troubleshooting

**Enemies not respawning?**
- Check if 3 scene loads have passed
- Verify `enemyId` is set on Enemy prefab
- Look for console errors in MapSceneLoader

**Enemies respawning too fast/slow?**
- Adjust `respawnAfterSceneLoads` in SaveData.cs
- Default is 3, increase for harder, decrease for easier

**Old save files?**
- System auto-migrates old `deadEnemies` list
- First map load after update handles conversion
- No manual intervention needed

## Advanced: Custom Respawn Times

Want bosses that never respawn? Elite enemies that respawn faster?

### Option 1: Enemy Component
```csharp
// Add to Enemy.cs
public bool isBoss = false;
public int customRespawnTime = -1; // -1 = use default
```

### Option 2: Enemy ID Convention
```csharp
// In MapSceneLoader.cs
if (enemyId.StartsWith("BOSS_"))
    return true; // Never respawn
    
if (enemyId.StartsWith("ELITE_"))
    respawnTime = 5; // Longer respawn
```

## Statistics Tracking

Since we track `deathCount`, you can:
- Display "Times Defeated: X" for each enemy
- Award achievements for defeating same enemy multiple times
- Show total enemies defeated in player stats
- Create "Nemesis" system (enemy that killed you most)

---

**Quick Setup**: Everything works out of the box. Just fight enemies and explore!

**Default Setting**: 3 scene transitions for respawn

**Compatible With**: All existing save files, leveling system v2.0
