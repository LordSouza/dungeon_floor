using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public Slider healthSlider;
    public Slider xpSlider; // Optional XP bar for player HUD
    public TextMeshProUGUI xpText; // Optional XP text for player HUD

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        levelText.text = "Lvl " + unit.unitLevel;
        healthSlider.maxValue = unit.MaxHp;
        healthSlider.value = unit.currentHp;
        
        // Update XP display if components exist (player only)
        UpdateXP(unit);
    }

    public void setHP(int hp)
    {
        healthSlider.value = hp;
    }
    
    public void UpdateXP(Unit unit)
    {
        // Only update if XP UI elements exist (for player HUD)
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
