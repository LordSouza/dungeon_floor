# Leveling System - Quick Reference

## XP Requirements by Level

```
Level  │ XP Needed │ Total XP │ Time to Level
───────┼───────────┼──────────┼──────────────────
  1→2  │    10     │    10    │ 2 same-level enemies
  2→3  │    28     │    38    │ 5-6 enemies
  3→4  │    52     │    90    │ 10 enemies
  4→5  │    80     │   170    │ 16-17 enemies
  5→6  │   112     │   282    │ ★ MILESTONE ★
  6→7  │   149     │   431    │ 30 enemies
  7→8  │   191     │   622    │ 44 enemies
  8→9  │   237     │   859    │ 58 enemies
  9→10 │   287     │  1146    │ 73 enemies
 10→11 │   341     │  1487    │ ★ MILESTONE ★
```

## Stat Progression Table

```
Level │ HP   │ Damage │ Notes
──────┼──────┼────────┼─────────────────────────
  1   │  20  │   5    │ Starting stats
  2   │  25  │   7    │ 
  3   │  30  │   9    │ 
  4   │  35  │  11    │ 
  5   │  51  │  16    │ ★ +10 HP, +3 DMG bonus
  6   │  57  │  18    │ +1 HP scaling bonus
  7   │  62  │  20    │ 
  8   │  67  │  22    │ 
  9   │  72  │  24    │ 
 10   │  89  │  30    │ ★ +10 HP, +3 DMG bonus + +1 DMG scaling
```

## XP Reward Multipliers

Fighting enemies of different levels:

```
┌─────────────────────────────────────────────────────┐
│  Your Level: 5                                      │
├─────────────┬──────────────┬─────────────┬─────────┤
│ Enemy Level │  Multiplier  │  Base XP    │ Final   │
├─────────────┼──────────────┼─────────────┼─────────┤
│     10      │   200%+      │   50 XP     │ 100 XP  │ (VERY HARD)
│      8      │   160%       │   40 XP     │  64 XP  │ (HARD)
│      7      │   140%       │   35 XP     │  49 XP  │ (CHALLENGING)
│      6      │   130%       │   30 XP     │  39 XP  │ (TOUGH)
│      5      │   100%       │   25 XP     │  25 XP  │ (EQUAL)
│      4      │    90%       │   20 XP     │  18 XP  │ (EASY)
│      3      │    80%       │   15 XP     │  12 XP  │ (VERY EASY)
│      2      │    50%       │   10 XP     │   5 XP  │ (TRIVIAL)
│      1      │    50%       │    5 XP     │   3 XP  │ (TRIVIAL)
└─────────────┴──────────────┴─────────────┴─────────┘
```

## Milestone Rewards

```
Level 5:  ★★★  +10 HP, +3 Damage  ★★★
Level 10: ★★★  +10 HP, +3 Damage  ★★★
Level 15: ★★★  +10 HP, +3 Damage  ★★★
Level 20: ★★★  +10 HP, +3 Damage  ★★★
...and every 5 levels thereafter
```

## Combat Example

**Scenario**: Level 3 player fights Level 5 enemy

1. **Before Battle**:
   - Player: 30 HP, 9 Damage, 38/52 XP to Level 4
   
2. **Victory**:
   - Enemy gives 25 base XP (Level 5 × 5)
   - Level difference: +2, so 140% multiplier
   - Final XP: 35 XP
   
3. **After Battle**:
   - Player XP: 38 + 35 = 73 XP
   - **LEVEL UP!** (52 XP needed)
   - New stats: Level 4, 35 HP (full heal), 11 Damage
   - Remaining XP: 21/80 toward Level 5

## Design Philosophy

✓ **Early Levels**: Fast progression to hook players  
✓ **Mid Levels**: Balanced challenge and reward  
✓ **Late Levels**: Meaningful milestones  
✓ **Challenge Scaling**: Rewards risk-taking  
✓ **Anti-Grinding**: Diminishing returns on weak enemies  
