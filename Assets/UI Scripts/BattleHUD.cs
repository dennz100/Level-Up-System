using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text levelText;
    public Text healthText;
    public Slider healthSlider;

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        levelText.text = "Lvl" + unit.unitLevel;
        // healthText.text = unit.currentHealth;
        healthSlider.maxValue = unit.maxHealth;
        healthSlider.value = unit.currentHealth;
    }

    public void SetHealth(int health)
    {
        healthSlider.value = health;
    }
}
