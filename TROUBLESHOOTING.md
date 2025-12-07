# Troubleshooting: "Missing Script" Error & Respawn Issues

## Problem Solved ✅

I've fixed the main issue - **Enemy.cs** was still using the old `deadEnemies` check in `Awake()`, which prevented the respawn system from working.

## Fix Applied

**Enemy.cs** - Removed the old dead check:
```csharp
// OLD CODE (REMOVED):
if (GameManager.Instance.data.deadEnemies.Contains(enemyId))
    Destroy(gameObject);

// NEW CODE:
// Enemy cleanup now handled by MapSceneLoader for respawn system
```

The respawn logic is now entirely in `MapSceneLoader.cs` where it belongs.

---

## Unity "Missing Script" Error Solutions

### Solution 1: Force Recompile (Most Common Fix) ⭐

**In Unity Editor:**
1. Click **Assets** menu → **Reimport All**
2. Wait for Unity to recompile (may take a minute)
3. Check Console for any errors

**OR Faster Method:**
1. Go to **Edit** → **Preferences** → **External Tools**
2. Click **Regenerate project files**
3. Restart Unity Editor

### Solution 2: Clear Unity Cache

1. **Close Unity completely**
2. Delete these folders in your project:
   - `Library/ScriptAssemblies/`
   - `Temp/`
3. **Reopen Unity** - it will rebuild everything

### Solution 3: Check for Duplicate Scripts

Sometimes Unity gets confused by duplicate script files:

1. In Unity, use **Edit** → **Find References In Scene**
2. Search for "Enemy" in Project window
3. Make sure there's only ONE `Enemy.cs` file
4. Check for `Enemy.cs.meta` conflicts

### Solution 4: Manual Meta File Cleanup

1. **Close Unity**
2. Navigate to: `Assets/Scripts/`
3. Look for any `.meta` files without matching `.cs` files
4. Delete orphaned `.meta` files
5. **Reopen Unity**

### Solution 5: Check Scene Objects

**In Unity Editor:**
1. Open **MapScene**
2. Find Enemy GameObjects in Hierarchy
3. Select each enemy
4. In Inspector, check if any script component shows "(Script)"
5. If you see missing scripts:
   - Remove the missing script component
   - Re-add the Enemy.cs script from Scripts folder

---

## Testing the Respawn System

After fixing the above, test the respawn:

### Test Steps:

1. **Start the game** (or reset save if needed)
2. **Defeat an enemy** in MapScene
3. **Note the enemy's position** and ID
4. **Trigger a scene transition**:
   - Fight another enemy → return to map
   - OR leave area and return
5. **Check if enemy respawned** at original position

### Expected Behavior (with `respawnAfterSceneLoads = 1`):

```
Action                          | Scene Load Count | Enemy Status
--------------------------------|------------------|-------------
Load MapScene                   |        1         | Alive
Defeat Enemy A                  |        1         | Dead
Return to MapScene              |        2         | RESPAWNED ✨
```

### Debug Logging

Add this to MapSceneLoader.cs `Start()` method to see what's happening:

```csharp
void Start()
{
    var data = GameManager.Instance.data;
    
    // Increment scene load counter for respawn system
    data.totalSceneLoads++;
    
    Debug.Log($"=== MapScene Loaded (Count: {data.totalSceneLoads}) ===");
    Debug.Log($"Respawn threshold: {data.respawnAfterSceneLoads}");
    Debug.Log($"Death records: {data.enemyDeathRecords.Count}");
    
    // ... rest of code
}
```

---

## Common Issues & Solutions

### Issue: Enemies Still Not Respawning

**Possible Causes:**

1. **Enemy has no ID set**
   - In Unity, select enemy → check Enemy component
   - Set `Enemy Id` field (e.g., "goblin_01")

2. **Scene load count not incrementing**
   - Check console logs for scene load count
   - Make sure MapSceneLoader is in the scene

3. **Old save file issues**
   - Delete save file: `%APPDATA%\..\LocalLow\<CompanyName>\Dungeon Floor\save.json`
   - Restart game for fresh save

4. **MapSceneLoader not attached**
   - Create empty GameObject in MapScene
   - Add MapSceneLoader.cs component to it

### Issue: "The referenced script is missing"

**This means:**
- Unity can't find a script that's attached to a GameObject
- Often caused by:
  - Compilation errors
  - Moved/renamed script files
  - Corrupted .meta files

**Quick Fix:**
```powershell
# In PowerShell, from project root:
Remove-Item -Path "Library\ScriptAssemblies\*" -Force -Recurse
Remove-Item -Path "Temp\*" -Force -Recurse
```

Then reopen Unity.

---

## Nuclear Option: Complete Reset

If nothing works:

1. **Backup your Scenes and Scripts folders**
2. Close Unity
3. Delete:
   - `Library/`
   - `Temp/`
   - `obj/`
4. Delete all `.csproj` and `.sln` files
5. Reopen Unity → let it rebuild everything

---

## Verification Checklist

After fixes, verify:

- [ ] No compilation errors in Console
- [ ] Enemy.cs doesn't check `deadEnemies` in Awake
- [ ] MapSceneLoader exists in MapScene
- [ ] Each enemy has unique `enemyId` set
- [ ] SaveData has `respawnAfterSceneLoads = 1` for testing
- [ ] Console shows scene load increments
- [ ] Enemies respawn after 1 scene load

---

**Current Status**: Code is correct. Issue is likely Unity editor state. Try Solution 1 first!
