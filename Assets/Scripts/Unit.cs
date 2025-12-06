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

    public int damagePerLevel = 2;
    public int hpPerLevel = 5;
    
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

        MaxHp += hpPerLevel;
        damage += damagePerLevel;

        currentHp = MaxHp;

        Debug.Log(unitName + " subiu para o nível " + unitLevel);
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
