using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    Rigidbody2D _enemyPlayerRb;
    [SerializeField] GameObject enemyPoof;
    public string enemyId;
    float _enemyDir = 1;
    public int enemyLevel;
    void Awake()
    {
        _enemyPlayerRb = GetComponent<Rigidbody2D>();
        // Note: Enemy cleanup now handled by MapSceneLoader for respawn system
        // Old deadEnemies check removed to support respawning
    }

    void FixedUpdate()
    {
        _enemyPlayerRb.linearVelocityX = 2 * _enemyDir;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        
        if(other.CompareTag("Foreground"))    
            FlipSprite();
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.data.currentEnemyLevel = enemyLevel;
            GameManager.Instance.data.lastEnemyID = enemyId;
            Destroy();
        }
    }

    public void Destroy()
    {
        GameObject poof = Instantiate(enemyPoof, transform.position, Quaternion.identity);
        Destroy(poof,0.8f);
        Destroy(gameObject);
    }

    void FlipSprite()
    {
        transform.localScale = new Vector3(-Mathf.Sign(_enemyPlayerRb.linearVelocityX),1, 1);
        _enemyDir *= -1;
    }
    
}
