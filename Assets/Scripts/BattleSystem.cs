using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST}

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public Animator playerAnimator;
    public Animator enemyAnimator;
    
    private Unit playerUnit;
    private Unit enemyUnit;

    public TextMeshProUGUI dialogueText;
    
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
    
    public BattleState state;
    
    public AudioSource audioSource;
    public AudioClip playerAttackSound;
    public AudioClip playerHealSound;
    public AudioClip enemyAttackSound;
    
    public GameObject playerButtonsPanel;
    
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();
        playerAnimator = playerGO.GetComponentInChildren<Animator>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();
        enemyAnimator = enemyGO.GetComponentInChildren<Animator>();

        dialogueText.text = "Voce encontrou um " + enemyUnit.unitName + ", mate-o."; 
        
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(1f);
        
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        audioSource.PlayOneShot(playerAttackSound);
        playerAnimator.SetTrigger("attack");
        playerAnimator.SetInteger("random", Random.Range(1, 4));
        enemyAnimator.SetTrigger("hit");
        
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        enemyHUD.setHP(enemyUnit.currentHp);
        dialogueText.text = "ataque realizado com sucesso";
        
        yield return new WaitForSeconds(1f);
        
        if (isDead)
        {
            state = BattleState.WON;
            enemyAnimator.SetTrigger("morre");
            EndBattle();
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("MapScene");
        } else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        playerButtonsPanel.SetActive(false);
        audioSource.PlayOneShot(enemyAttackSound);
        enemyAnimator.SetTrigger("attack");

        dialogueText.text = enemyUnit.unitName + " te ataca!";
       
        
        yield return new WaitForSeconds(0.5f);
        
        playerAnimator.SetTrigger("hit");

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        
        playerHUD.setHP(playerUnit.currentHp);
        
        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("DefeatScene");
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
        playerButtonsPanel.SetActive(true);
        
    }

    IEnumerator PlayerHeal()
    {
        audioSource.PlayOneShot(playerHealSound);
        playerUnit.Heal(4);
        
        playerHUD.setHP(playerUnit.currentHp);
        dialogueText.text = "Voce se sente melhor!";
        
        yield return new WaitForSeconds(0.05f);

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

