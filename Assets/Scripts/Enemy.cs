using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    
    Rigidbody2D _enemyPlayerRb;
    [SerializeField] GameObject enemyPoof;
    public string enemyId;
    float _enemyDir = 1;
    public int enemyLevel;
    public int enemyPrefabID;
    void Awake()
    {
        _enemyPlayerRb = GetComponent<Rigidbody2D>();
        if (GameManager.Instance.data.deadEnemies.Contains(enemyId))
          Destroy(gameObject);
    }

    void FixedUpdate()
    {
        _enemyPlayerRb.linearVelocityX = 2 * _enemyDir;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Foreground"))    
            FlipSprite();
        if(other.CompareTag("Player"))    
            KillEnemy();
    }

    public void KillEnemy()
    {
        Destroy(gameObject);
    }

    void FlipSprite()
    {
        transform.localScale = new Vector3(-Mathf.Sign(_enemyPlayerRb.linearVelocityX),1, 1);
        _enemyDir *= -1;
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        Enemy e = other.gameObject.GetComponent<Enemy>();
        
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log($"[Enemy] Player colidiu com: {enemyId}, prefabID = {enemyPrefabID}");

            GameManager.Instance.data.enemyPrefabToSpawn = enemyPrefabID;

            GameManager.Instance.data.playerX = transform.position.x;
            GameManager.Instance.data.playerY = transform.position.y;

            GameManager.Instance.data.lastEnemyID = enemyId;
            GameManager.Instance.data.currentEnemyLevel = enemyLevel;

            GameManager.Instance.Save();
            SceneManager.LoadScene("GameScene");
        }
    }
    
}
