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
    
    [SerializeField] float speedX;
    [SerializeField] float jumpIntensity;
    
    void Awake()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _playerSpriteAnimator = GetComponentInChildren<Animator>();
    }

    void OnJump(InputValue inputValue)
    {

        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Foreground")))
        {
            _playerRb.linearVelocityY = jumpIntensity;
            bool isJumping = Mathf.Abs(_playerRb.linearVelocityX) > Mathf.Epsilon;
            _playerSpriteAnimator.SetBool("IsJumping",isJumping);
            _canDoubleJump = true;
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
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Enemy e = other.gameObject.GetComponent<Enemy>();

        if (e != null)
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