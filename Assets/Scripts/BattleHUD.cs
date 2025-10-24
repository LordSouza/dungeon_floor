using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text levelText;
    public Slider healthSlider;

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        levelText.text = "Lvl " + unit.unitLevel;
        healthSlider.maxValue = unit.MaxHp;
        healthSlider.value = unit.currentHp;
    }

    public void setHP(int hp)
    {
        healthSlider.value = hp;
    }
}
