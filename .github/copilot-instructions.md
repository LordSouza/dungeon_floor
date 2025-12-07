# Dungeon Floor - AI Coding Agent Instructions

## Project Overview
A Unity 2D side-scrolling dungeon crawler with turn-based combat, level progression, and Quick Time Events (QTE). The game uses scene-based transitions between exploration and combat modes.

## Architecture & Data Flow

### Scene Structure
- **Init.unity** → Boot.cs → loads MainMenuScene
- **MainMenuScene.unity** → Main menu with start/reset options
- **MapScene.unity** → 2D platformer exploration with roaming enemies
- **GameScene.unity** → Turn-based combat system
- **DefeatScene.unity** → Game over state

### Persistent State Management
**GameManager.cs** is a singleton (`DontDestroyOnLoad`) that persists game state across scenes:
- Uses **SaveData.cs** class for JSON serialization to `Application.persistentDataPath + "/save.json"`
- Stores player stats (level, XP, HP, damage), position (playerX, playerY), and dead enemy IDs
- Always call `GameManager.Instance.Save()` after modifying `data` properties
- Load/Save pattern: Load on Awake, Save immediately after state changes

### Scene Transitions & State Handoff
1. **Map → Battle**: Player.cs detects collision with Enemy → saves player position and enemy data → loads GameScene
2. **Battle → Map**: BattleSystem.cs marks enemy as dead → saves player stats → loads MapScene
3. **MapSceneLoader.cs** restores player position and removes dead enemies on Map scene load

## Combat System

### Turn-Based Flow (BattleSystem.cs)
Uses **BattleState enum**: `START → PLAYERTURN → ENEMYTURN → (WON|LOST)`
- Player actions: Attack or Heal buttons trigger QTE system
- Enemy AI: 10% chance heal, 5% chance buff, otherwise attack
- Combat instantiates prefabs at battle stations, uses Animators for visual feedback

### Quick Time Event System (BattleQuickTIme.cs)
- Generates random arrow key sequences (A/B/C/D mapped to Up/Down/Right/Left)
- **Success**: `extraDamageFromQte` or `extraHealingFromQte` bonus = sequence length × 2
- **Partial/Fail**: Bonus = correct keys × 1
- QTE controller sets bonus on BattleSystem, then triggers coroutine (PlayerAttack/PlayerHeal)

### Unit Stats & Leveling (Unit.cs)
- **Player**: Stats persist via GameManager.Instance.data (playerLevel, playerXP, playerMaxHP, playerDamage)
- **Enemies**: Initialized with `InitializeEnemy(level)` → scales HP/damage based on level
- XP gain: `enemyLevel × 5`, level up adds `hpPerLevel=5` HP and `damagePerLevel=2` damage

## Key Patterns & Conventions

### Collision Detection Strategy
- **Player.cs**: `OnCollisionEnter2D` with Enemy → triggers battle transition
- **Enemy.cs**: `OnTriggerExit2D` with Foreground → flips sprite direction
- Enemies patrol using `Rigidbody2D.linearVelocityX` with direction flipping on triggers

### Component References
- Use `GetComponent<T>()` for same GameObject components (Rigidbody2D, Animator)
- Use `GetComponentInChildren<Animator>()` for child sprite objects
- Use `FindFirstObjectByType<T>()` for singleton-like managers (BattleSystem in QteController)
- Avoid `Find()` calls in Update - cache references in Awake/Start

### Input System
- Uses Unity's new Input System (InputSystem_Actions.inputactions)
- Player.cs implements `OnMove(InputValue)` and `OnJump(InputValue)` callbacks
- QTE uses old Input.GetKeyDown for arrow key detection

### Animator Integration
- Trigger parameters: "attack", "hit", "morre", "DoubleJump"
- Bool parameters: "IsRunning"
- Integer parameters: "random" (1-4 for attack variations)
- Always cache Animator references, set triggers before yield delays

## Development Workflows

### Adding New Enemy Types
1. Set unique `enemyId` string on Enemy.cs component (for persistence)
2. Set `enemyLevel` int (scales stats via Unit.ScaleByLevel)
3. Ensure Rigidbody2D + trigger colliders configured
4. Dead enemies tracked in `SaveData.deadEnemies` list

### Modifying Combat Mechanics
- Player stats in SaveData.cs (base values: level=1, XP=0, maxHP=20, damage=5)
- Enemy scaling formula in Unit.ScaleByLevel: `MaxHp = 10 + (level × 6)`, `damage = 3 + (level × 2)`
- QTE bonus multipliers in BattleQuickTIme.CompleteQte (currently `seqSize × 2`)

### Audio Integration
- BattleSystem.cs manages AudioSource with playerAttackSound, playerHealSound, enemyAttackSound clips
- Use `audioSource.PlayOneShot(clip)` before animation triggers

## Planned Features & Extensions

### Leveling System Improvements
- Current system: Linear XP gain (`enemyLevel × 5`), fixed stat increases per level
- Future enhancements should consider: skill trees, stat allocation choices, or level-gated abilities

### Fishing Minigame & Inventory
- **Fishing Minigame**: Will be a separate scene or UI overlay minigame
- **Fish Item**: Consumable item obtainable from fishing, usable in battle for healing
- **Inventory System**: Need to add inventory data structure to SaveData.cs
- **Battle Integration**: Add item usage button/mechanic in BattleSystem.cs alongside Attack/Heal
- Consider: Item slots in BattleHUD, inventory UI in MapScene

## Critical Implementation Notes

- **Never** modify GameManager.Instance.data without calling .Save() immediately after
- Scene loads are synchronous - no async operations expected
- Coroutines in BattleSystem use `yield return new WaitForSeconds()` for turn pacing
- Enemy destruction in MapScene handled by MapSceneLoader on scene load, not on kill
- Player position persists across scene transitions via SaveData (playerX, playerY)
- All file paths in Unity use forward slashes: `Assets/Scripts/`, not backslashes
- When adding new features (fishing, inventory), follow the existing save/load pattern in GameManager
