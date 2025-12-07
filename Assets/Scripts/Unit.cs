using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    
    public int damage;
    
    public int MaxHp;
    public int currentHp;
    
    public int currentXP;
    public int xpToNextLevel = 10;

    // Base stat increases per level
    public int damagePerLevel = 2;
    public int hpPerLevel = 5;
    
    // Calculates XP required for a specific level using exponential curve
    public int CalculateXPRequirement(int level)
    {
        // Faster progression formula: 10 * level^1.2 (reduced from 1.5)
        // Level 1->2: 10, Level 2->3: 23, Level 3->4: 36, Level 4->5: 52, etc.
        // Much faster than before - about 30% less XP needed per level!
        return Mathf.RoundToInt(10f * Mathf.Pow(level, 1.2f));
    }
    
    public void GainXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
    }
    
    void LevelUp()
    {
        unitLevel++;

        // Calculate stat increases with slight scaling
        int hpGain = hpPerLevel + (unitLevel / 5); // +1 HP every 5 levels
        int damageGain = damagePerLevel + (unitLevel / 10); // +1 damage every 10 levels

        MaxHp += hpGain;
        damage += damageGain;

        // Full heal on level up
        currentHp = MaxHp;

        // Update XP requirement for next level
        xpToNextLevel = CalculateXPRequirement(unitLevel);

        // Milestone bonuses every 5 levels
        if (unitLevel % 5 == 0)
        {
            int bonusHP = 10;
            int bonusDamage = 3;
            MaxHp += bonusHP;
            damage += bonusDamage;
            currentHp = MaxHp;
            Debug.Log(unitName + " atingiu o nível marco " + unitLevel + "! Bônus: +" + bonusHP + " HP, +" + bonusDamage + " Dano");
        }
        else
        {
            Debug.Log(unitName + " subiu para o nível " + unitLevel + " (+" + hpGain + " HP, +" + damageGain + " Dano)");
        }
    }
    
    public bool TakeDamage(int dmg)
    {
        currentHp -= dmg;

        if (currentHp <= 0)
            return true;
        else
            return false;
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        if (currentHp > MaxHp)
            currentHp = MaxHp;
    }
    
    public void ScaleByLevel()
    {
        // um escalador para o nivel dos mobs
        MaxHp = 10 + (unitLevel * 6);  
        damage = 3 + (unitLevel * 2);   
        currentHp = MaxHp;
    }
    public void InitializeEnemy(int level)
    {
        unitLevel = level;
        ScaleByLevel();
    }
}
