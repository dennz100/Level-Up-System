using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum BattleState {START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public UnityEvent<float> EnemyKilledEvent;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;

    public Text dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public WaitForSeconds WaitTwoSeconds = new WaitForSeconds(2f);
    public WaitForSeconds WaitOneSecond = new WaitForSeconds(1f);

    public BattleState state;
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        SetupBattle();
    }

    public void UpdateHealthUI()
    {
        playerHUD.SetHealth(playerUnit.HealthUI);
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();
        LevelSystem levelSystem = GameObject.Find("XPBar").GetComponent<LevelSystem>();
        levelSystem.LevelUpEvent.AddListener(playerUnit.LevelUp);
        
        levelSystem.UpdateHealthEvent.AddListener(UpdateHealthUI);

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        dialogueText.text = "A " + enemyUnit.unitName + " appears.";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return WaitTwoSeconds;

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        int playerDamageAmount = Random.Range(playerUnit.unitLevel + 3, playerUnit.unitLevel + 5);

        bool isDead = enemyUnit.TakeDamage(playerDamageAmount);

        enemyHUD.SetHealth(enemyUnit.HealthUI);
        dialogueText.text = "Enemy took " + playerDamageAmount + " DMG.";
        enemyHUD.healthText.text = enemyUnit.currentHealth.ToString();

        if (enemyUnit.currentHealth < 0)
        {
            enemyUnit.currentHealth = 0;
            enemyHUD.healthText.text = enemyUnit.currentHealth.ToString();
        }

        yield return WaitOneSecond;

        if (isDead)
        {
            dialogueText.text = "Enemy Defeated!";
            Destroy(enemyUnit.gameObject);
            EnemyKilledEvent.Invoke(enemyUnit.Experience);

            yield return WaitTwoSeconds;

            dialogueText.text = "A " + enemyUnit.unitName + " appears.";
            GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
            enemyUnit = enemyGO.GetComponent<Unit>();
            // enemyHUD.SetHealth(enemyUnit.currentHealth);
            enemyUnit.unitLevel = Random.Range(playerUnit.unitLevel, playerUnit.unitLevel + 2);
            
            if (enemyUnit.unitLevel > 1)
            {
                enemyUnit.maxHealth += enemyUnit.maxHealth / 4;
            }
           
            enemyUnit.currentHealth = enemyUnit.maxHealth;
            
            enemyHUD.SetHUD(enemyUnit);
            // state = BattleState.WON;
            // EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        yield return WaitTwoSeconds;
    }

    void EndBattle()
    {
        if(state == BattleState.WON)
        {
            dialogueText.text = "You won!";
        }
        else if (state == BattleState.LOST)
        {
            Destroy(playerUnit.gameObject);
            dialogueText.text = "Defeat.";
        }
    }

    IEnumerator EnemyTurn()
    {
        int enemyDamageAmount = Random.Range(enemyUnit.unitLevel + 1, enemyUnit.unitLevel + 3);

        dialogueText.text = enemyUnit.unitName + " attacks!";

        yield return WaitOneSecond;

       bool isDead = playerUnit.TakeDamage(enemyDamageAmount);

        playerHUD.SetHealth(playerUnit.HealthUI);
        dialogueText.text = "Knight took " + enemyDamageAmount + " DMG.";
        playerHUD.healthText.text = playerUnit.currentHealth.ToString();
        
        if (playerUnit.currentHealth < 0)
        {
            playerUnit.currentHealth = 0;
            playerHUD.healthText.text = playerUnit.currentHealth.ToString();
        }

        // print(playerUnit.HealthUI);
        // print(playerUnit.currentHealth);
        // print(playerUnit.maxHealth);

        yield return WaitOneSecond;

        if(isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }

    IEnumerator PlayerHeal()
    {
        int healAmount = Random.Range(playerUnit.unitLevel + 1, playerUnit.unitLevel + 7);

        playerUnit.Heal(healAmount);
        playerHUD.healthText.text = playerUnit.currentHealth.ToString();

        playerHUD.SetHealth(playerUnit.HealthUI);
        dialogueText.text = "You healed for " + healAmount + " HP.";

        yield return WaitOneSecond;

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());

        yield return WaitTwoSeconds;
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }
    
    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }
}
