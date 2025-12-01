using System.Collections;
using TMPro;
using UnityEngine;
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
    
    private Unit _playerUnit;
    private Unit _enemyUnit;
    
    public TextMeshProUGUI dialogueText;
    
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
    
    public BattleState state;
    public QteController qteController;
    
    public AudioSource audioSource;
    public AudioClip playerAttackSound;
    public AudioClip playerHealSound;
    public AudioClip enemyAttackSound;
    
    public GameObject playerButtonsPanel;
    
    public int extraDamageFromQte;
    public int extraHealingFromQte;
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGo = Instantiate(playerPrefab, playerBattleStation);
        _playerUnit = playerGo.GetComponent<Unit>();
        playerAnimator = playerGo.GetComponentInChildren<Animator>();

        GameObject enemyGo = Instantiate(enemyPrefab, enemyBattleStation);
        _enemyUnit = enemyGo.GetComponent<Unit>();
        enemyAnimator = enemyGo.GetComponentInChildren<Animator>();

        dialogueText.text = "Voce encontrou um " + _enemyUnit.unitName + ", mate-o."; 
        
        playerHUD.SetHUD(_playerUnit);
        enemyHUD.SetHUD(_enemyUnit);

        yield return new WaitForSeconds(1f);
        
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    public IEnumerator PlayerAttack()
    {
        audioSource.PlayOneShot(playerAttackSound);
        playerAnimator.SetTrigger("attack");
        playerAnimator.SetInteger("random", Random.Range(1, 4));
        enemyAnimator.SetTrigger("hit");
        
        int totalDamage = _playerUnit.damage + extraDamageFromQte;
        extraDamageFromQte = 0;
        bool isDead = _enemyUnit.TakeDamage(totalDamage);
        enemyHUD.setHP(_enemyUnit.currentHp);
        dialogueText.text = "ataque realizado com sucesso";
        
        yield return new WaitForSeconds(1f);
        
        if (isDead)
        {
            state = BattleState.WON;
            enemyAnimator.SetTrigger("morre");
            EndBattle();
            
            GameData data = GameData.Load();
            data.defeatedEnemies.Add(data.currentEnemyId);
            data.currentEnemyId = null;
            data.Save();
            
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

        dialogueText.text = _enemyUnit.unitName + " está pensando...";
        yield return new WaitForSeconds(0.8f);

        float rnd = Random.value; 

        float chanceHeal = 0.25f;  
        float chanceBuff = 0.15f;  

        bool didAction = false;
        
        if (rnd < chanceHeal)
        {
            int healAmount = Random.Range(2, 5);
            _enemyUnit.Heal(healAmount);
            enemyHUD.setHP(_enemyUnit.currentHp);

            dialogueText.text = _enemyUnit.unitName + " recuperou " + healAmount + " de vida!";

            yield return new WaitForSeconds(1.2f);

            didAction = true;
        }
        else if (rnd < chanceHeal + chanceBuff)
        {
            int bonus = Random.Range(3, 7);
            _enemyUnit.damage += bonus;
            dialogueText.text = _enemyUnit.unitName + " ficou mais forte! (+ " + bonus + " dano)";

            yield return new WaitForSeconds(1.2f);

            didAction = true;
        }
        
        if (!didAction)
        {
            audioSource.PlayOneShot(enemyAttackSound);

            enemyAnimator.SetTrigger("attack");
            dialogueText.text = _enemyUnit.unitName + " te ataca!";

            yield return new WaitForSeconds(0.5f);

            playerAnimator.SetTrigger("hit");

            bool isDead = _playerUnit.TakeDamage(_enemyUnit.damage);
            playerHUD.setHP(_playerUnit.currentHp);

            yield return new WaitForSeconds(1f);

            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
                yield return new WaitForSeconds(1f);
                SceneManager.LoadScene("DefeatScene");
                yield break;
            }
        }
        
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }
    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            GameManager.Instance.MarkEnemyAsDead(GameManager.Instance.data.lastEnemyID);
            SceneManager.LoadScene("MapScene");
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

    public IEnumerator PlayerHeal()
    {
        audioSource.PlayOneShot(playerHealSound);

        int totalHeal = 4 + extraHealingFromQte;
        extraHealingFromQte = 0;

        _playerUnit.Heal(totalHeal);

        playerHUD.setHP(_playerUnit.currentHp);
        dialogueText.text = "Você recuperou vida!";

        yield return new WaitForSeconds(1f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    
    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        playerButtonsPanel.SetActive(false);

        if (qteController != null)
        {
            qteController.StartQte(QteType.Attack);
            dialogueText.text = "Repita a sequência para atacar!";
        }
        else
        {
            StartCoroutine(PlayerAttack());
        }
        
    }
    
    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        playerButtonsPanel.SetActive(false);

        if (qteController != null)
        {
            qteController.StartQte(QteType.Heal);
            dialogueText.text = "Repita a sequência para curar!";
        }
        else
        {
            StartCoroutine(PlayerHeal());
        }
    }
    
    public void QteFailed()
    {
        dialogueText.text = "Falhou no evento";
        StartCoroutine(EnemyTurn());
    }

}

