using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LevelSystem : MonoBehaviour
{
    public UnityEvent<int> LevelUpEvent;
    public UnityEvent<float, float> UpdateXpEvent;
    public UnityEvent UpdateHealthEvent;
    public int level;
    public float currentXp;
    public float requiredXp;

    // [Header("UI")]
    // public Image frontXpBar;
    // public Image backXpBar;
    // public TextMeshProUGUI levelText;
    // public TextMeshProUGUI xpText;
    [Header("Multipliers")]
    [Range(1f, 300f)]
    public float additionMultiplier = 300;
    [Range(2f, 4f)]
    public float powerMultiplier = 2;
    [Range(7f, 14f)]
    public float divisionMultiplier = 7;
    // Start is called before the first frame update
    void Start()
    {
        // frontXpBar.fillAmount = currentXp / requiredXp;
        // backXpBar.fillAmount = currentXp / requiredXp;
        level = 1;
        requiredXp = CalculateRequiredXp();
        UpdateXpEvent.Invoke(currentXp, requiredXp);
    }

    // Update is called once per frame
    void Update()
    {
        // UpdateXpUI();
        if (Input.GetKeyDown(KeyCode.X))
            GainExperienceFlatRate(20);
        if (currentXp > requiredXp)
            LevelUp();
    }

    public void GainExperienceFlatRate(float xpGained)
    {
        print(xpGained);
        currentXp += xpGained;
        UpdateXpEvent.Invoke(currentXp, requiredXp);
        // lerpTimer = 0f;
        // delayTimer = 0f;
    }

    public void GainExperienceScalable(float xpGained, int passedLevel)
    {
        if(passedLevel < level)
        {
            float multiplier = 1 + (level - passedLevel) * 0.1f;
            currentXp += xpGained * multiplier;
        }
        else
        {
            currentXp += xpGained;
        }
        // lerpTimer = 0f;
        // delayTimer = 0f;
    }

    public void LevelUp()
    {
        level++;
        // frontXpBar.fillAmount = 0f;
        // backXpBar.fillAmount = 0f;
        currentXp = Mathf.RoundToInt(currentXp - requiredXp);
        // GetComponent<PlayerHealth>().IncreaseHealth(level);
        requiredXp = CalculateRequiredXp();
        // levelText.text = "Lvl " + level;
        LevelUpEvent.Invoke(level);
        UpdateXpEvent.Invoke(currentXp, requiredXp);
        UpdateHealthEvent.Invoke();
    }

    private int CalculateRequiredXp()
    {
        int solveForRequiredXp = 0;
        for(int levelCycle = 1; levelCycle <= level; levelCycle++)
        {
            solveForRequiredXp += (int)Mathf.Floor(levelCycle + additionMultiplier * Mathf.Pow(powerMultiplier, levelCycle / divisionMultiplier));
        }
        return solveForRequiredXp / 4;
    }

}