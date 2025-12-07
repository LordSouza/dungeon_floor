# Fishing System - Quick Reference

## 🎣 What You Get

**Fishing Minigame:**
- Timing-based mechanic
- Moving indicator on progress bar
- Press button when in green zone
- Success = get fish item!

**Fish Item:**
- Stored in inventory (persistent)
- Usable in battle
- Heals 12 HP
- Consumable (one-time use)

---

## 🎮 How to Play

### Fishing:
```
1. Walk to fishing spot in MapScene
2. See "Press E to Fish"
3. Press E
4. Watch moving indicator ━━━━→
5. Press SPACE/E when in [GREEN ZONE]
6. Success! +1 Fish
```

### Using Fish in Battle:
```
1. In battle, on your turn
2. Click "Use Fish" button
3. Player heals 12 HP
4. Fish count -1
5. Enemy's turn
```

---

## 🛠️ Unity Setup (Quick Steps)

### 1. Fishing UI (Canvas)
```
FishingCanvas
└── FishingPanel
    ├── ProgressSlider (Slider with Fill)
    │   └── SuccessZone (Green Image)
    ├── InstructionText (TMP)
    └── ResultText (TMP)

FishingManager (Empty GameObject)
└── FishingMinigame script
    ├── Assign: FishingPanel
    ├── Assign: ProgressSlider
    ├── Assign: SuccessZone
    ├── Assign: InstructionText
    └── Assign: ResultText
```

### 2. Fishing Spot (MapScene)
```
FishingSpot (Sprite with BoxCollider2D Trigger)
├── FishingSpot script
│   ├── Assign: FishingManager
│   └── Assign: PromptText
└── FishingPrompt (TMP, initially hidden)
```

### 3. Battle Button (GameScene)
```
PlayerButtonsPanel
├── AttackButton ✓
├── HealButton ✓
└── UseItemButton ← Add this!
    └── On Click: BattleSystem.OnUseItemButton()
```

---

## 📊 Visual Layout

### Fishing UI:
```
┌─────────────────────────────────────────┐
│ Press SPACE when indicator is in GREEN! │  ← Instruction
│                                         │
│  ░░░░░░░░[██]░░░░░░░░░░░░░░░░░░░░      │  ← Progress Bar
│         ↑ Indicator                     │
│  ░░░░░░░░[▓▓▓▓▓▓]░░░░░░░░░░░░░░        │  ← Success Zone (Green)
│                                         │
│        SUCCESS! You caught a fish!      │  ← Result
│             (Total: 3)                  │
└─────────────────────────────────────────┘
```

### Battle UI:
```
┌─────────────────────────────────┐
│ Player HP: [████████░░] 20/25   │
│ Fish: 3                         │  ← Fish count
│                                 │
│ [Attack] [Heal] [Use Fish]      │  ← Buttons
└─────────────────────────────────┘
```

---

## ⚙️ Settings

**Difficulty (in FishingMinigame):**
- Indicator Speed: 2 (1=easy, 4=hard)
- Success Zone Size: 0.2 (0.3=easy, 0.1=hard)

**Balance (in BattleSystem):**
- Fish Heal Amount: 12 HP

---

## 🎯 Code Summary

### SaveData.cs:
```csharp
public int fishCount = 0; // Inventory
```

### FishingMinigame.cs:
```csharp
StartFishing()     // Begin minigame
AttemptCatch()     // Check if successful
→ fishCount++      // Add fish on success
```

### FishingSpot.cs:
```csharp
OnTriggerEnter2D() // Player enters
Press E            // Trigger fishing
→ StartFishing()   // Call minigame
```

### BattleSystem.cs:
```csharp
OnUseItemButton()  // Button clicked
UseItem()          // Consume fish
→ Heal player      // +12 HP
→ fishCount--      // Remove fish
```

---

## ✅ Testing Checklist

**Fishing:**
- [ ] Can walk to fishing spot
- [ ] Prompt appears when near
- [ ] Press E starts minigame
- [ ] Indicator moves smoothly
- [ ] Success zone visible (green)
- [ ] Catching fish increases count
- [ ] Missing decreases nothing
- [ ] UI closes after 2 seconds

**Battle:**
- [ ] Use Fish button visible
- [ ] Button disabled if no fish
- [ ] Using fish heals player
- [ ] Fish count decreases
- [ ] Enemy turn after using

**Persistence:**
- [ ] Fish count saves
- [ ] Fish count loads on restart

---

## 🎨 Enhancement Ideas

**Easy Additions:**
- Different fish types (healing amounts)
- Fishing level/skill system
- Multiple fishing spots
- Rare fish with bonus effects

**Visual Polish:**
- Animated fishing spot
- Particle effects on catch
- Sound effects
- Fish sprites bouncing

**Gameplay:**
- Fishing cooldown timer
- Bait system
- Fishing competition
- Sell fish for gold

---

## 📁 Files

**Modified:**
- SaveData.cs
- BattleSystem.cs

**Created:**
- FishingMinigame.cs
- FishingSpot.cs
- FISHING_SETUP_GUIDE.md
- FISHING_IMPLEMENTATION_PLAN.md

**Unity Setup:**
- FishingCanvas (UI)
- FishingManager (GameObject)
- FishingSpot (MapScene)
- UseItemButton (GameScene)

---

**TL;DR:** Code is done! Set up UI in Unity following the guide! 🎣✨
