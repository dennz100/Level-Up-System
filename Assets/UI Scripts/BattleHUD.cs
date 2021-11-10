using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{

    public TMP_Text nameText;
    public TMP_Text levelText;
    public TMP_Text healthText;
    public Slider healthSlider;

    public Image frontXpBar;
    public Image backXpBar;
    public TextMeshProUGUI xpText;

    private float lerpTimer;
    private float delayTimer;

    private void Start()
    {
        if (frontXpBar)
        {
            frontXpBar.fillAmount = 0;
        }
        
    }

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        levelText.text = "Lvl " + unit.unitLevel;
        // healthText.text = unit.currentHealth;
        healthSlider.maxValue = unit.maxHealth;
        healthSlider.value = unit.currentHealth;
    }

    public void SetHealth(int health)
    {
        healthSlider.value = health;
    }

    public void SetLevel(int level)
    {
        levelText.text = "Lvl " + level.ToString();
        frontXpBar.fillAmount = 0;
        backXpBar.fillAmount = 0;
    }

    public void UpdateXpUI(float currentXp, float requiredXp)
    {
        this.currentXp = currentXp;
        this.requiredXp = requiredXp;

        float xpFraction = currentXp / requiredXp;
        float FXP = frontXpBar.fillAmount;
        // frontXpBar.fillAmount = currentXp / requiredXp;
        backXpBar.fillAmount = currentXp / requiredXp;
        if (FXP < xpFraction)
        {
            ShouldAnimateXpBar = true;
            StopCoroutine(AnimateXpBar(currentXp, requiredXp));
            StartCoroutine(AnimateXpBar(currentXp, requiredXp));
        }
        xpText.text = currentXp + "/" + requiredXp;
    }

    public bool ShouldAnimateXpBar;
    float currentXp;
    float requiredXp;

    private void Update()
    {
        //// if (ShouldAnimateXpBar)
        //{
        //    if (frontXpBar && backXpBar)
        //    {
        //        float xpFraction = this.currentXp / requiredXp;
        //        float FXP = frontXpBar.fillAmount;
        //        print(xpFraction);
        //        // print(requiredXp);
        //        print(frontXpBar.fillAmount);
        //        print(FXP < xpFraction);
        //        if (FXP < xpFraction)
        //        {
        //            delayTimer += Time.deltaTime;
        //            print(delayTimer);
        //            backXpBar.fillAmount = xpFraction;
        //            if (delayTimer > 0.3)
        //            {
        //                lerpTimer += Time.deltaTime;
        //                float percentComplete = lerpTimer / 4;
                        
        //                frontXpBar.fillAmount = Mathf.Lerp(FXP, backXpBar.fillAmount, percentComplete);
        //            }
        //        }
        //    }

        //}

    }
    [Range(0.0001f, 1)]
    public float LerpPercent = 0.5f;

    private IEnumerator AnimateXpBar(float currentXp, float requiredXp)
    {
        // if (ShouldAnimateXpBar)
        {
            if (frontXpBar && backXpBar)
            {
                yield return new WaitForSeconds(0.3f);
                float xpFraction = currentXp / requiredXp;
                float FXP = frontXpBar.fillAmount;
                // print(requiredXp);
                if (FXP < xpFraction)
                {
                    // delayTimer += Time.deltaTime;
                    backXpBar.fillAmount = xpFraction;
                    // if (delayTimer > 0.3)
                    while (frontXpBar.fillAmount < backXpBar.fillAmount)
                    {
                        lerpTimer += Time.deltaTime;
                        float percentComplete = lerpTimer / LerpPercent;

                        frontXpBar.fillAmount = Mathf.Lerp(FXP, backXpBar.fillAmount, percentComplete);
                        yield return null;
                    }
                }
                lerpTimer = 0;
            }

        }

        //float lerpDuration = 1;
        //float startValue = frontXpBar.fillAmount;
        //float endValue = currentXp / requiredXp;
        //float valueToLerp;
        //{
        //    float timeElapsed = 0;

        //    while (timeElapsed < lerpDuration)
        //    {
        //        timeElapsed += Time.deltaTime;
        //        valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
        //        frontXpBar.fillAmount = valueToLerp;

        //        yield return null;
        //    }

        //    valueToLerp = endValue;
        //}
    }
}
