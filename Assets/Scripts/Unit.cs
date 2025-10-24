using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    
    public int damage;
    
    public int MaxHp;
    public int currentHp;

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
    
}
