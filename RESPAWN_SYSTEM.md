# Enemy Respawn System

## Overview
Enemies now respawn after a certain number of scene transitions, allowing players to farm XP and progress through the improved leveling system.

## How It Works

### Respawn Timing
- **Default**: Enemies respawn after **3 scene loads**
- Configurable via `SaveData.respawnAfterSceneLoads`
- Scene loads tracked in `SaveData.totalSceneLoads`

### Scene Load Counter
A "scene load" happens when:
- Returning to MapScene from battle
- Loading MapScene from menu (counts as 1)
- Each transition back to the map increments the counter

### Example Timeline

```
┌─────────────────────────────────────────────────────────┐
│ Action                    │ Scene Loads │ Enemy Status  │
├───────────────────────────┼─────────────┼───────────────┤
│ Start game                │      0      │ Alive         │
│ Enter MapScene            │      1      │ Alive         │
│ Fight & defeat Enemy A    │      1      │ Dead          │
│ Return to MapScene        │      2      │ Dead          │
│ Fight & defeat Enemy B    │      2      │ Dead          │
│ Return to MapScene        │      3      │ Dead          │
│ Fight & defeat Enemy C    │      3      │ Dead          │
│ Return to MapScene        │      4      │ RESPAWNED! ✨ │
│                           │             │ (Enemy A)     │
└───────────────────────────┴─────────────┴───────────────┘
```

**Enemy A** was defeated at scene load 1, respawns at scene load 4 (3 loads later)

## Implementation Details

### Data Structures

#### EnemyDeathRecord
```csharp
public class EnemyDeathRecord
{
    public string enemyId;           // Unique enemy identifier
    public int deathCount;           // Times defeated (for stats)
    public int sceneLoadsAtDeath;    // When enemy was last defeated
}
```

#### SaveData Fields
```csharp
public List<EnemyDeathRecord> enemyDeathRecords;  // New system
public int totalSceneLoads;                       // Scene load counter
public int respawnAfterSceneLoads = 3;            // Respawn interval
public List<string> deadEnemies;                  // Old system (deprecated)
```

### Respawn Logic (MapSceneLoader.cs)

1. **Increment Counter**: `totalSceneLoads++`
2. **Migrate Old Data**: Convert old `deadEnemies` list to new system
3. **Clean Up**: Remove expired death records
4. **Check Status**: Determine if each enemy should be dead or alive
5. **Apply State**: Destroy enemies that are still dead

### Recording Deaths (BattleSystem.cs)

When an enemy is defeated:
1. Find existing death record for enemy
2. If exists: Increment `deathCount`, update `sceneLoadsAtDeath`
3. If new: Create new `EnemyDeathRecord`
4. Save to `enemyDeathRecords` list

## Customization

### Change Respawn Rate

Edit in SaveData.cs or GameManager:
```csharp
// Fast respawn (every 2 scene loads)
data.respawnAfterSceneLoads = 2;

// Slow respawn (every 5 scene loads)
data.respawnAfterSceneLoads = 5;

// Instant respawn (every load)
data.respawnAfterSceneLoads = 1;

// No respawn
data.respawnAfterSceneLoads = 999999;
```

### Per-Enemy Respawn Times

You could extend `EnemyDeathRecord` to include:
```csharp
public int customRespawnTime = -1; // -1 = use global, else use custom
```

Then modify `IsEnemyDead()` in MapSceneLoader:
```csharp
int respawnTime = record.customRespawnTime > 0 
    ? record.customRespawnTime 
    : data.respawnAfterSceneLoads;
    
return scenesSinceDeath < respawnTime;
```

### Boss Enemies (No Respawn)

Mark bosses with very high respawn time:
```csharp
// In SaveData or add to EnemyDeathRecord
public bool isBoss = false;

// In MapSceneLoader.IsEnemyDead()
if (record.isBoss)
    return true; // Boss stays dead forever
```

## Backward Compatibility

### Migration System
Old save files using `deadEnemies` list automatically convert:
- On first MapScene load after update
- Old enemies transferred to new `enemyDeathRecords`
- All marked with current `totalSceneLoads` as death time
- Will respawn after 3 more scene loads

### No Data Loss
- Existing defeated enemies remain defeated initially
- Gradual respawn as player explores
- Old `deadEnemies` list cleared after migration

## Benefits

### For Gameplay
✅ **XP Farming**: Players can level up by revisiting areas  
✅ **Progression**: Required for new exponential leveling curve  
✅ **Exploration**: Encourages moving through the map  
✅ **Challenge**: Can return to grind when struggling  

### For Balance
✅ **Anti-Camping**: Must move to different enemies (scene transitions)  
✅ **Natural Pace**: Respawn rate controls progression speed  
✅ **Flexible**: Easy to adjust respawn timing  
✅ **Trackable**: Can see how many times player defeated each enemy  

## Future Enhancements

### Possible Features:
1. **Scaled Respawns**: Enemies respawn at player's current level
2. **Respawn Notifications**: "An enemy has respawned!" message
3. **Respawn Visualization**: Particle effect when enemy respawns
4. **Rare Spawns**: Some enemies have longer respawn times but better rewards
5. **Respawn Zones**: Different areas have different respawn rates
6. **Rest Mechanic**: Resting at checkpoint triggers immediate respawn

### Statistics Tracking:
Since we track `deathCount`, you could display:
- Total enemies defeated
- Times each enemy was defeated
- XP gained from respawned enemies
- Most fought enemy

## Testing Checklist

- [ ] Defeat an enemy, return to map - enemy stays dead
- [ ] After 3 scene loads, enemy respawns
- [ ] Multiple enemies respawn independently
- [ ] Old save files convert correctly
- [ ] Death count increments when re-fighting enemy
- [ ] XP gain works normally on respawned enemies
- [ ] Changing `respawnAfterSceneLoads` works as expected

---

**Version**: 1.0  
**Last Updated**: December 7, 2025  
**Related Systems**: Leveling System v2.0, SaveData, MapSceneLoader
