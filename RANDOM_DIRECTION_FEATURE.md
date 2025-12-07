# Random Enemy Direction on Respawn

## 🔄 New Feature: Randomized Enemy Direction!

### What Changed?

**Before:**
- Enemies always faced/moved in their original direction
- Predictable patrol patterns

**After:**
- **50% chance to flip direction** on each spawn
- Enemy could be moving left OR right
- Completely unpredictable!

---

## 🎮 How It Works

### On Every Map Load:

1. **Random Position:** Enemy spawns at random location ✓
2. **Random Direction:** 50% chance enemy faces opposite way ✓

### Example Scenario:

**Original Enemy:**
```
[Goblin] → moving RIGHT at (5, 2)
```

**After Respawn - Possibility 1 (50% chance):**
```
[Goblin] → moving RIGHT at (10, 3)  ← Same direction, new position
```

**After Respawn - Possibility 2 (50% chance):**
```
[Goblin] ← moving LEFT at (15, 5)  ← FLIPPED direction, new position
```

---

## 💡 Why This Is Great

### Gameplay Benefits:

✅ **Even More Unpredictable** - Can't anticipate enemy movement  
✅ **Varied Encounters** - Same enemy feels different each time  
✅ **Strategic Depth** - Must adapt to enemy facing direction  
✅ **No Pattern Memorization** - Truly random behavior  
✅ **Replayability** - Never exactly the same twice  

### Combat Impact:

- **Enemy facing you?** They might see you first!
- **Enemy facing away?** Sneak attack opportunity!
- **Can't predict** which way they'll go
- **Must react** to each situation

---

## 🔧 Technical Details

### New Methods in Enemy.cs:

```csharp
// Set specific direction (-1 = left, 1 = right)
public void SetDirection(float direction)

// Flip current direction (reverses movement)
public void FlipDirection()
```

### MapSceneLoader Logic:

```csharp
// 50% chance to flip
bool shouldFlip = Random.value > 0.5f;
if (shouldFlip) {
    enemy.FlipDirection();
}
```

### What Gets Flipped:

1. **Movement direction** (`_enemyDir` variable)
2. **Sprite facing** (visual representation)
3. **Patrol direction** (which way they move)

---

## 📊 Statistics

### Spawn Variations Per Enemy:

With 3 spawn points:

**Old System:**
- 3 possible positions
- Total: **3 variations**

**New System:**
- 3 possible positions
- 2 possible directions per position
- Total: **6 variations!**

**With 5 spawn points:**
- 5 positions × 2 directions = **10 variations!**

---

## 🧪 Testing

### How To Test:

1. **Play game** and note enemy position & direction
2. **Look at enemy:** Which way is it moving?
3. **Fight and return** to map
4. **Check enemy again:** Different position AND possibly different direction!

### Console Logs:

You'll see one of:
```
Enemy goblin_01 spawned at random position: (10.0, 3.0) - Direction FLIPPED
```
or
```
Enemy goblin_01 spawned at random position: (5.0, 2.0) - Original direction
```

### Visual Test:

**Before:**
```
  → → →  Goblin always moves right
```

**After Load 1:**
```
  ← ← ←  Goblin now moves left!
```

**After Load 2:**
```
  → → →  Goblin moves right again!
```

---

## ⚙️ Configuration

### Already Active!

The feature is **automatically enabled** with random spawns:
- `enableRandomSpawns = true` → direction randomization ON
- `enableRandomSpawns = false` → direction randomization OFF

### Adjust Flip Chance:

In `MapSceneLoader.cs`, modify:

```csharp
// Current: 50% chance
bool shouldFlip = Random.value > 0.5f;

// Always flip:
bool shouldFlip = true;

// 75% chance to flip:
bool shouldFlip = Random.value > 0.25f;

// 25% chance to flip:
bool shouldFlip = Random.value > 0.75f;

// Never flip:
bool shouldFlip = false;
```

---

## 🎯 Example Scenarios

### Scenario 1: Stealth Approach
```
Enemy spawns facing away from you
→ Sneak attack opportunity!
```

### Scenario 2: Head-On Collision
```
Enemy spawns facing toward you
→ Quick reaction needed!
```

### Scenario 3: Unpredictable Patrol
```
Same enemy, same spot, different direction
→ Must adapt strategy each time
```

---

## 🎲 Randomization Summary

### What's Random Now:

1. ✅ **Spawn Position** - Any point from pool
2. ✅ **Movement Direction** - Left or right (50/50)
3. ✅ **Sprite Facing** - Matches movement direction

### Total Randomization:

```
Position: Random
Direction: Random
Result: MAXIMUM VARIETY! 🎲✨
```

---

## 🔍 Behind The Scenes

### What Happens On Spawn:

```
1. Pick random spawn point        → (x, y)
2. Move enemy to that position    → enemy.position = (x, y)
3. Roll for direction flip        → 50% chance
4. If flip: enemy.FlipDirection() → Reverses movement & sprite
5. Enemy starts moving            → In new random direction!
```

### Preserved Behavior:

- ✅ Enemy still patrols normally
- ✅ Still flips at walls/edges
- ✅ Still triggers battles on collision
- ✅ All original mechanics work

---

## 💪 Gameplay Impact

### Player Experience:

**Before:**
```
"That goblin is always at (5,2) moving right.
I'll just wait here for it..."
```

**After:**
```
"Where's the goblin?
Oh there! Wait, it's coming toward me!
Last time it was facing the other way!"
```

### Difficulty Impact:

- **Slightly harder** - Can't predict approach
- **More engaging** - Must stay alert
- **More realistic** - Feels more alive
- **Better gameplay** - Rewards adaptability

---

## 📝 Summary

### Complete Randomization System:

| Feature | Status |
|---------|--------|
| Random spawn positions | ✅ ACTIVE |
| Random spawn directions | ✅ ACTIVE |
| Fast leveling | ✅ ACTIVE |
| Instant respawns | ✅ ACTIVE |

### What You Get:

- 🎲 **Random positions** every load
- 🔄 **Random directions** every load
- ⚡ **Fast progression** with new XP curve
- 🔁 **Instant respawns** after 1 scene load

**Result:** Maximum unpredictability and replayability! 🎮✨

---

## ✅ Active Now!

**No configuration needed** - works automatically with random spawns!

Every enemy spawn is now truly unique:
- Different position ✓
- Different direction ✓
- Every single time ✓

**Enjoy the enhanced dynamic gameplay!** 🚀
