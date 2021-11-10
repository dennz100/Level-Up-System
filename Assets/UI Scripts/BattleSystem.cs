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
    
    public BattleState state;
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        SetupBattle();
    }

    public void UpdateHealthUI(int health)
    {
        playerHUD.SetHealth(playerUnit.currentHealth / playerUnit.maxHealth);
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

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        enemyHUD.SetHealth(enemyUnit.currentHealth);

        if(isDead)
        {
            EnemyKilledEvent.Invoke(enemyUnit.Experience);
            GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
            enemyUnit = enemyGO.GetComponent<Unit>();
            enemyHUD.SetHealth(enemyUnit.currentHealth);
            // state = BattleState.WON;
            // EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        yield return new WaitForSeconds(2f);
    }

    void EndBattle()
    {
        if(state == BattleState.WON)
        {
            dialogueText.text = "You won!";
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "Defeat.";
        }
    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.unitName + " attacks!";

        yield return new WaitForSeconds(1f);

       bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        playerHUD.SetHealth(playerUnit.currentHealth / playerUnit.maxHealth);
        print(playerUnit.currentHealth / playerUnit.maxHealth);
        print(playerUnit.currentHealth);
        print(playerUnit.maxHealth);

        yield return new WaitForSeconds(1f);

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
        playerUnit.Heal(5);

        playerHUD.SetHealth(playerUnit.currentHealth);
        dialogueText.text = "You healed yourself.";

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());

        yield return new WaitForSeconds(2f);
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
