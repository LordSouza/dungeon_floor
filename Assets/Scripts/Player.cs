using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    Rigidbody2D _playerRb;
    Animator _playerSpriteAnimator;
    BoxCollider2D _feetCollider;
    
    float _xDir;
    bool _canDoubleJump;
    bool takingDmg;
    
    [SerializeField] float speedX;
    [SerializeField] float jumpIntensity;
    
    void Awake()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _playerSpriteAnimator = GetComponentInChildren<Animator>();
        _feetCollider = GetComponentInChildren<BoxCollider2D>();
    }

    void OnJump(InputValue inputValue)
    {
        if (_feetCollider.IsTouchingLayers(LayerMask.GetMask("Foreground")))
        {
            _playerRb.linearVelocityY = jumpIntensity;
            _canDoubleJump = true;
            return;
        }

        if (_canDoubleJump)
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
        if (takingDmg)
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
    }

    void OnCollisionEnter2D(Collision2D other)
    {

        Enemy e = other.gameObject.GetComponent<Enemy>();
        
        if (e != null)
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}