# Fishing Minigame Implementation Guide

## Overview
A simple timing-based fishing minigame that rewards players with fish items that can be used in battle for healing.

## System Architecture

### 1. Inventory System
- Add inventory to SaveData.cs
- Track fish count (consumable item)
- Persist across save/load

### 2. Fishing Minigame
- Trigger from fishing spot in MapScene
- Timing-based mechanic (press button at right time)
- Difficulty scales with player level
- Success = gain fish item

### 3. Battle Integration
- New "Use Item" button in battle
- Fish heals 10-15 HP
- Consumes one fish from inventory

## Implementation Steps

### Step 1: Add Inventory to SaveData ✓
```csharp
// In SaveData.cs
public int fishCount = 0; // Number of fish in inventory
```

### Step 2: Create Fishing Minigame Script
```csharp
// FishingMinigame.cs
- Shows UI with moving indicator
- Player presses button when indicator is in "success zone"
- Success = add fish to inventory
- Can be triggered from fishing spot
```

### Step 3: Create Fishing Spot in Map
```csharp
// FishingSpot.cs
- Collider trigger in MapScene
- Player presses 'E' or 'F' to start fishing
- Loads fishing minigame UI
```

### Step 4: Battle Item Usage
```csharp
// BattleSystem.cs
- Add UseItem button
- Check if player has fish
- Heal player
- Decrement fish count
```

### Step 5: UI Elements
- Fishing minigame overlay (Canvas)
- Inventory display in MapScene
- Item button in battle HUD

## Design Decisions

### Fishing Mechanic Options:

**Option A: Timing-based (Recommended)** ⭐
- Moving indicator slides across bar
- Press button when in green zone
- Simple, intuitive, skill-based

**Option B: Button mashing**
- Press button rapidly
- Fill progress bar
- Energy-based

**Option C: Sequence matching**
- Show sequence of buttons
- Player repeats sequence
- Similar to QTE system

**Recommendation:** Option A for simplicity and accessibility

### Fish Item Properties:
- **Healing Amount:** 10-15 HP (configurable)
- **Stack Limit:** Unlimited (or 99 max)
- **Usage:** Battle only (not in map)
- **Cost:** Time investment in minigame

## File Structure

```
Assets/Scripts/
├── SaveData.cs (modify)
├── FishingMinigame.cs (new)
├── FishingSpot.cs (new)
├── BattleSystem.cs (modify)
└── BattleHUD.cs (modify)

Assets/Scenes/
└── MapScene.unity (add fishing spot)

Assets/Prefabs/ (optional)
└── FishingUI.prefab
```

## Next Steps

I'll now implement each component. The system will:
1. ✅ Be simple and fun
2. ✅ Follow existing code patterns
3. ✅ Integrate with save system
4. ✅ Work with battle system
5. ✅ Require minimal Unity setup

Ready to proceed? I'll create the scripts next!
