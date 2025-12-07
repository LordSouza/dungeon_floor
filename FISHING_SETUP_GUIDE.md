


# Fishing Minigame - Complete Setup Guide

## ✅ Scripts Created!

All code is complete. Now you need to set up the UI and scene objects in Unity.

---

## 📋 Part 1: Create Fishing UI (In GameScene or MapScene)

### Step 1: Create Fishing UI Canvas

**In Unity:**
1. **Right-click in Hierarchy** → UI → Canvas
2. Rename to "**FishingCanvas**"
3. Set **Canvas Scaler** → UI Scale Mode: **Scale With Screen Size**
4. Reference Resolution: **1920 x 1080**

### Step 2: Create Fishing Panel

1. **Right-click FishingCanvas** → UI → Panel
2. Rename to "**FishingPanel**"
3. Set color to semi-transparent black (#00000080)
4. Covers full screen (stretches to fill)

### Step 3: Create Progress Bar

**Create Slider:**
1. **Right-click FishingPanel** → UI → Slider
2. Rename to "**ProgressSlider**"
3. Position: Center of screen
4. Width: 600, Height: 40

**Configure Slider:**
- **Interactable**: UNCHECKED (read-only)
- **Min Value**: 0
- **Max Value**: 1
- **Value**: 0

**Style Background:**
- Select **ProgressSlider → Background**
- Color: Dark gray (#333333)

**Style Fill (Indicator):**
- Select **ProgressSlider → Fill Area → Fill**
- Color: White or cyan (#00BCD4)

**Hide Handle:**
- Select **ProgressSlider → Handle Slide Area**
- Disable or set width to 0

### Step 4: Create Success Zone (Green Zone)

1. **Right-click ProgressSlider** → UI → Image
2. Rename to "**SuccessZone**"
3. Color: Green (#4CAF50) with 50% opacity
4. Set **Anchor**: Stretch horizontally
5. **Initial values** (will be set by code):
   - Anchor Min X: 0.3, Y: 0
   - Anchor Max X: 0.5, Y: 1

### Step 5: Create Instruction Text

1. **Right-click FishingPanel** → UI → Text - TextMeshPro
2. Rename to "**InstructionText**"
3. Position: Top center
4. Text: "Press SPACE or E when indicator is in GREEN zone!"
5. Font Size: 24
6. Color: White
7. Alignment: Center

### Step 6: Create Result Text

1. **Right-click FishingPanel** → UI → Text - TextMeshPro
2. Rename to "**ResultText**"
3. Position: Bottom center
4. Text: "" (empty at start)
5. Font Size: 28
6. Color: Yellow (#FFEB3B)
7. Alignment: Center

### Step 6.5: Create Fish Counter Text

1. **Right-click FishingPanel** → UI → Text - TextMeshPro
2. Rename to "**FishCountText**"
3. Position: Top-right corner
4. Text: "Fish: 0"
5. Font Size: 20
6. Color: White
7. Alignment: Right

### Step 6.6: Create Exit Button

1. **Right-click FishingPanel** → UI → Button - TextMeshPro
2. Rename to "**ExitButton**"
3. Position: Bottom-right corner
4. Button text: "Exit (ESC)"
5. Font Size: 18
6. Color: Red or gray
7. Size: 120x40

### Step 7: Attach FishingMinigame Script

1. **Right-click in Hierarchy** → Create Empty
2. Rename to "**FishingManager**"
3. **Add Component** → **FishingMinigame** (the script we created)

**Assign References in Inspector:**
- **Fishing UI**: Drag FishingPanel
- **Progress Slider**: Drag ProgressSlider
- **Success Zone**: Drag SuccessZone (the green image)
- **Instruction Text**: Drag InstructionText
- **Result Text**: Drag ResultText
- **Fish Count Text**: Drag FishCountText (new!)
- **Exit Button**: Drag ExitButton (new!)

**Configure Settings:**
- **Indicator Speed**: 2 (adjust for difficulty)
- **Success Zone Size**: 0.2 (20% of bar - smaller = harder)
- **Fish Heal Amount**: 12

---

## 📋 Part 2: Add Fishing Spot to MapScene

### Step 8: Create Fishing Spot GameObject

**In MapScene:**
1. **Right-click in Hierarchy** → 2D Object → Sprite
2. Rename to "**FishingSpot**"
3. Position it where you want (e.g., near water)
4. **Sprite**: Use a water/pond sprite (or simple blue square for testing)

**Add Collider:**
1. **Add Component** → Box Collider 2D
2. Check **Is Trigger**: ✓
3. Size: Large enough for player to walk into (e.g., 2x2)

**Add Script:**
1. **Add Component** → **FishingSpot** (the script we created)

### Step 9: Create Prompt Text (Optional)

1. **Right-click FishingSpot** → UI → Text - TextMeshPro
2. Rename to "**FishingPrompt**"
3. Position above fishing spot
4. Text: "Press E to Fish"
5. Font Size: 16
6. **Initially disabled** (will show when player is near)

### Step 10: Configure FishingSpot Script

**In Inspector (FishingSpot component):**
- **Fishing Minigame**: Drag FishingManager from Hierarchy
- **Prompt Text**: Drag FishingPrompt
- **Interact Key**: E (default)

---

## 📋 Part 3: Add Item Button to Battle

### Step 11: Add "Use Item" Button in GameScene

**In GameScene:**
1. Find your **Player Buttons Panel** (has Attack and Heal buttons)
2. **Duplicate** the Heal button
3. Rename to "**UseItemButton**"
4. Change text to "**Use Fish**" or "**Item**"

**Configure Button:**
1. Select UseItemButton
2. **On Click ()** event:
   - Click **+** to add event
   - Drag **BattleSystem** object to field
   - Select: **BattleSystem** → **OnUseItemButton()**

### Step 12: (Optional) Add Fish Count Display

**In Battle HUD:**
1. Add TextMeshPro text
2. Name: "FishCountText"
3. Text: "Fish: 0"
4. Position near player HUD

**To update dynamically**, add to BattleHUD.cs:
```csharp
public TextMeshProUGUI fishCountText;

public void UpdateFishCount()
{
    if (fishCountText != null)
        fishCountText.text = $"Fish: {GameManager.Instance.data.fishCount}";
}
```

---

## 🎮 Testing the System

### Test Fishing Minigame:

1. **Play MapScene**
2. **Walk to fishing spot**
3. **See prompt**: "Press E to Fish"
4. **Press E** → Fishing UI appears
5. **Watch indicator** move across bar
6. **Press SPACE or E** when indicator is in green zone
7. **Success**: "SUCCESS! You caught a fish!"
8. **Counter updates**: "Fish: 1" (top-right)
9. **After 2 seconds** → Minigame **restarts automatically**
10. **Fish again** as many times as you want!
11. **Press ESC or click Exit** when done → Returns to MapScene

### Test Battle Item Usage:

1. **Start a battle** (make sure you caught at least 1 fish)
2. **Your turn** → See "Use Fish" button
3. **Click "Use Fish"**
4. **Result**: Player heals 12 HP, fish count decreases
5. **If no fish**: "Você não tem nenhum peixe!"

---

## 🎨 Visual Customization

### Make it Look Better:

**Fishing UI:**
- Add background image (pond/water texture)
- Add fish sprites bouncing
- Add particle effects on success
- Add sound effects (splash, success chime)

**Fishing Spot:**
- Use water sprite or animation
- Add ripple effect
- Add floating indicator (arrow or icon)

**Button:**
- Add fish icon to Use Item button
- Color code (blue for fish item)

---

## 🔄 New Feature: Continuous Fishing!

### How It Works:

**The fishing minigame now loops automatically:**
- After catching (success or failure), wait 2 seconds
- Minigame **restarts automatically** with new random green zone
- Player can fish **as many times as they want**
- Fish counter updates in real-time
- **Exit anytime** with ESC key or Exit button

**Controls:**
- **SPACE or E**: Attempt to catch fish
- **ESC**: Exit fishing and return to map
- **Exit Button**: Same as ESC

**Benefits:**
- No need to re-enter fishing spot each time
- Farm fish efficiently
- Counter shows total fish collected
- Leave when you're satisfied

---

## ⚙️ Configuration Options

### Adjust Difficulty:

**In FishingMinigame component:**
- **Indicator Speed**: 
  - 1 = Easy (slow)
  - 2 = Medium (default)
  - 3-4 = Hard (fast)
  
- **Success Zone Size**:
  - 0.3 = Easy (30% of bar)
  - 0.2 = Medium (20%, default)
  - 0.1 = Hard (10%)

### Adjust Healing:

**In FishingMinigame:**
- **Fish Heal Amount**: 12 (default)
- Change to 10 (weaker) or 15 (stronger)

**In BattleSystem.UseItem():**
```csharp
int healAmount = 12; // Change this value
```

---

## 📊 System Overview

### Data Flow:

```
1. Player walks to FishingSpot
   ↓
2. Presses E → FishingMinigame.StartFishing()
   ↓
3. Success → data.fishCount++
   ↓
4. Save to file
   ↓
5. In battle → OnUseItemButton()
   ↓
6. Heal player, fishCount--
   ↓
7. Save to file
```

### Files Modified:
- ✅ **SaveData.cs** - Added fishCount
- ✅ **BattleSystem.cs** - Added OnUseItemButton() and UseItem()

### Files Created:
- ✅ **FishingMinigame.cs** - Minigame logic
- ✅ **FishingSpot.cs** - Trigger zone

---

## 🐛 Troubleshooting

### "Fishing doesn't start":
- ✅ Check FishingSpot has BoxCollider2D with Is Trigger checked
- ✅ Check FishingManager is assigned in FishingSpot component
- ✅ Check Player has "Player" tag

### "Indicator doesn't move":
- ✅ Check Fishing UI is active when testing
- ✅ Check Slider reference is assigned
- ✅ Check Indicator Speed > 0

### "Can't use fish in battle":
- ✅ Check button On Click event calls OnUseItemButton()
- ✅ Check you actually caught a fish (fishCount > 0)
- ✅ Check BattleSystem reference

### "Success zone in wrong position":
- ✅ SuccessZone should be child of ProgressSlider
- ✅ Anchors should be set to stretch
- ✅ Script will position it automatically

---

## 🎯 Quick Setup Checklist

**Scripts:** (Already done!)
- [x] SaveData.cs modified
- [x] FishingMinigame.cs created
- [x] FishingSpot.cs created
- [x] BattleSystem.cs modified

**Unity Setup:** (You need to do)
- [ ] Create Fishing UI in Canvas
- [ ] Create FishingManager with script
- [ ] Assign all UI references
- [ ] Add FishingSpot to MapScene
- [ ] Configure FishingSpot references
- [ ] Add Use Item button to battle
- [ ] Connect button to OnUseItemButton()

**Testing:**
- [ ] Test fishing minigame
- [ ] Test item usage in battle
- [ ] Test save/load persistence

---

## 🚀 You're Ready!

Follow the steps above in Unity and you'll have a fully functional fishing minigame!

The code is complete - just need the Unity scene setup! 🎣✨
