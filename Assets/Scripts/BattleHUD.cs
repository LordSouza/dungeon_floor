using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public Slider healthSlider;
    public Slider xpSlider; 
    public TextMeshProUGUI xpText; 

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        levelText.text = "Lvl " + unit.unitLevel;
        healthSlider.maxValue = unit.MaxHp;
        healthSlider.value = unit.currentHp;
        
        
        UpdateXP(unit);
    }

    public void setHP(int hp)
    {
        healthSlider.value = hp;
    }
    
    public void UpdateXP(Unit unit)
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = unit.xpToNextLevel;
            xpSlider.value = unit.currentXP;
        }
        
        if (xpText != null)
        {
            xpText.text = "XP: " + unit.currentXP + " / " + unit.xpToNextLevel;
        }
    }
}
