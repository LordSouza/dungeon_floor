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
        // instanciar o jogador
        GameObject playerGo = Instantiate(playerPrefab, playerBattleStation);
        _playerUnit = playerGo.GetComponent<Unit>();
        playerAnimator = playerGo.GetComponentInChildren<Animator>();
        _playerUnit.unitLevel = GameManager.Instance.data.playerLevel;
        _playerUnit.currentXP = GameManager.Instance.data.playerXP;
        _playerUnit.xpToNextLevel = GameManager.Instance.data.playerXPToNextLevel;

        _playerUnit.MaxHp = GameManager.Instance.data.playerMaxHP;
        _playerUnit.currentHp = GameManager.Instance.data.playerCurrentHP;

        _playerUnit.damage = GameManager.Instance.data.playerDamage;
        
        
        // instanciar o mob
        GameObject enemyGo = Instantiate(enemyPrefab, enemyBattleStation);
        _enemyUnit = enemyGo.GetComponent<Unit>();
        _enemyUnit.InitializeEnemy(GameManager.Instance.data.currentEnemyLevel);
        
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
            
            // Improved XP calculation based on level difference
            int xpGanho = CalculateXPReward(_playerUnit.unitLevel, _enemyUnit.unitLevel);
            
            // Store levels before XP gain to detect level up
            int oldLevel = _playerUnit.unitLevel;
            _playerUnit.GainXP(xpGanho);
            int newLevel = _playerUnit.unitLevel;
            
            // Update player HUD with new stats
            playerHUD.SetHUD(_playerUnit);
            
            // Display XP gain message
            dialogueText.text = "Vitória! +" + xpGanho + " XP ganho";
            
            // If leveled up, show special message
            if (newLevel > oldLevel)
            {
                dialogueText.text += "\n★ LEVEL UP! " + oldLevel + " → " + newLevel + " ★";
            }
            
            // Salvar os dados
            SaveData data = GameManager.Instance.data;
            data.playerLevel = _playerUnit.unitLevel;
            data.playerXP = _playerUnit.currentXP;
            data.playerXPToNextLevel = _playerUnit.xpToNextLevel;
            data.playerMaxHP = _playerUnit.MaxHp;
            data.playerDamage = _playerUnit.damage;
            
            GameManager.Instance.data.playerCurrentHP = _playerUnit.currentHp;
            
            // Add enemy to death records (new respawn system)
            if (!string.IsNullOrEmpty(data.lastEnemyID))
            {
                RecordEnemyDeath(data, data.lastEnemyID);
                data.lastEnemyID = null;
            }
            
            GameManager.Instance.Save();
            
            
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

        float chanceHeal = 0.10f;  
        float chanceBuff = 0.05f;  

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
            GameManager.Instance.data.playerCurrentHP = _playerUnit.currentHp;
            GameManager.Instance.Save();
            
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
            GameManager.Instance.data.playerCurrentHP = GameManager.Instance.data.playerMaxHP;
            GameManager.Instance.Save();
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
        
        GameManager.Instance.data.playerCurrentHP = _playerUnit.currentHp;
        GameManager.Instance.Save();
        
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
    
    // Calculate XP reward based on level difference for more interesting progression
    int CalculateXPReward(int playerLevel, int enemyLevel)
    {
        // Base XP scales with enemy level
        int baseXP = enemyLevel * 5;
        
        // Level difference multiplier
        int levelDiff = enemyLevel - playerLevel;
        float multiplier = 1.0f;
        
        if (levelDiff >= 3)
        {
            // Fighting much stronger enemies gives bonus XP
            multiplier = 1.5f + (levelDiff * 0.1f);
        }
        else if (levelDiff >= 1)
        {
            // Fighting slightly stronger enemies gives small bonus
            multiplier = 1.2f + (levelDiff * 0.1f);
        }
        else if (levelDiff == 0)
        {
            // Same level = normal XP
            multiplier = 1.0f;
        }
        else if (levelDiff >= -2)
        {
            // Fighting slightly weaker enemies gives reduced XP
            multiplier = 0.8f + (levelDiff * 0.1f);
        }
        else
        {
            // Fighting much weaker enemies gives minimal XP
            multiplier = 0.5f;
        }
        
        int finalXP = Mathf.RoundToInt(baseXP * multiplier);
        
        // Minimum XP reward
        return Mathf.Max(finalXP, 1);
    }
    
    void RecordEnemyDeath(SaveData data, string enemyId)
    {
        // Find existing record or create new one
        var existingRecord = data.enemyDeathRecords.Find(r => r.enemyId == enemyId);
        
        if (existingRecord != null)
        {
            // Enemy defeated again - update death time and increment counter
            existingRecord.deathCount++;
            existingRecord.sceneLoadsAtDeath = data.totalSceneLoads;
        }
        else
        {
            // First time defeating this enemy
            data.enemyDeathRecords.Add(new EnemyDeathRecord(enemyId, data.totalSceneLoads));
        }
    }

}

