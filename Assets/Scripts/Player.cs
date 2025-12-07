using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    Rigidbody2D _playerRb;
    Animator _playerSpriteAnimator;
    [FormerlySerializedAs("_feetCollider")] [SerializeField] BoxCollider2D feetCollider;
    
    float _xDir;
    bool _canDoubleJump;
    bool _takingDmg;
    bool _isGrounded = false;
    
    [SerializeField] float speedX;
    [SerializeField] float jumpIntensity;
    
    // Battle immunity system
    private bool _isBattleImmune = false;
    [SerializeField] private float battleImmunityDuration = 2f; // 2 seconds of immunity
    
    void Awake()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _playerSpriteAnimator = GetComponentInChildren<Animator>();
    }
    
    void Start()
    {
        // Check if player just returned from battle
        if (GameManager.Instance.data.justReturnedFromBattle)
        {
            StartBattleImmunity();
            GameManager.Instance.data.justReturnedFromBattle = false;
            GameManager.Instance.Save();
        }
    }
    
    void StartBattleImmunity()
    {
        _isBattleImmune = true;
        Invoke(nameof(EndBattleImmunity), battleImmunityDuration);
    }
    
    void EndBattleImmunity()
    {
        _isBattleImmune = false;
    }
    
    void OnDestroy()
    {
        // Cancelar Invoke pendente ao destruir
        CancelInvoke(nameof(EndBattleImmunity));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Foreground"))){
            _isGrounded = true;
            _playerSpriteAnimator.SetBool("IsJumping", !_isGrounded);
            _canDoubleJump = false;
        }
    }

    void OnJump(InputValue inputValue)
    {
        if (_isGrounded)
        {
            _playerRb.linearVelocityY = jumpIntensity;
            _isGrounded = false;
            _playerSpriteAnimator.SetBool("IsJumping", !_isGrounded);
            _canDoubleJump = true;
        }
        else if (_canDoubleJump)
        {
            _playerRb.linearVelocityY = jumpIntensity;
            _canDoubleJump = false;
            _playerSpriteAnimator.SetTrigger("DoubleJump");
        }
    }
    
    void OnMove(InputValue inputValue)
    {
        _xDir = inputValue.Get<Vector2>().x;
    }
    
    void MovePlayer()
    {
        if (_takingDmg)
            return;
        _playerRb.linearVelocityX = _xDir * speedX;
        bool isRunning = Mathf.Abs(_playerRb.linearVelocityX) > Mathf.Epsilon;

        _playerSpriteAnimator.SetBool("IsRunning",isRunning);
        if(isRunning)
            FlipSprite();
    }

    void FlipSprite()
    {
        transform.localScale = new Vector3(Mathf.Sign(_playerRb.linearVelocityX),1, 1);
    }

    void FixedUpdate()
    {
        MovePlayer();
        _playerSpriteAnimator.SetFloat("xVelocity", Math.Abs(_playerRb.linearVelocity.x));
        _playerSpriteAnimator.SetFloat("yVelocity", _playerRb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Enemy e = other.gameObject.GetComponent<Enemy>();

        if (e != null && !_isBattleImmune)
        {
            GameManager.Instance.data.playerX = transform.position.x;
            GameManager.Instance.data.playerY = transform.position.y;
            GameManager.Instance.data.lastEnemyID = e.enemyId;
            GameManager.Instance.data.currentEnemyLevel = e.enemyLevel;
            GameManager.Instance.Save();
            
            SceneManager.LoadScene("GameScene");
        }
    }

}