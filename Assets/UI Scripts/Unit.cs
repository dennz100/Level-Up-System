using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;

    public int damage;

    public int maxHealth;
    public int currentHealth;

    public float Experience = 20;

    public bool TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
    public void LevelUp(int level)
    {
        unitLevel = level;
        print(level);
        maxHealth += maxHealth / 4;
        currentHealth = maxHealth;
        
        print(currentHealth);
        print(maxHealth);
    }
}
