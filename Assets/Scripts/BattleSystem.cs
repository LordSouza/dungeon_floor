using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST}

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    private Unit playerUnit;
    private Unit enemyUnit;

    public TextMeshProUGUI dialogueText;
    
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
    
    public BattleState state;
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        dialogueText.text = "Voce encontrou um" + enemyUnit.unitName + ", mate-o."; 
        
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(1f);
        
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        
        enemyHUD.setHP(enemyUnit.currentHp);
        dialogueText.text = "ataque realizado com sucesso";
        
        yield return new WaitForSeconds(0.5f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        } else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.unitName + " te ataca!";
        
        yield return new WaitForSeconds(0.5f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        
        playerHUD.setHP(playerUnit.currentHp);
        
        yield return new WaitForSeconds(0.5f);

        if (isDead)
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
    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "Voce ganhou a batalha!";
        } else if (state == BattleState.LOST)
        {
            dialogueText.text = "Voce foi derrotado!";
        }
    }
    
    void PlayerTurn()
    {
        dialogueText.text = "Escolha uma ação";
        
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(4);
        
        playerHUD.setHP(playerUnit.currentHp);
        dialogueText.text = "Voce se sente melhor!";
        
        yield return new WaitForSeconds(0.5f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
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

