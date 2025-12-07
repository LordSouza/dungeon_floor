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
    }

    void FixedUpdate()
    {
        _enemyPlayerRb.linearVelocityX = 2 * _enemyDir;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Foreground"))    
            FlipSprite();
    }

    public void KillEnemy()
    {
        GameObject poof = Instantiate(enemyPoof, transform.position, Quaternion.identity);
        Destroy(poof, 0.8f);
        Destroy(gameObject);
    }

    void FlipSprite()
    {
        transform.localScale = new Vector3(-Mathf.Sign(_enemyPlayerRb.linearVelocityX),1, 1);
        _enemyDir *= -1;
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        // Colisão entre inimigos para evitar overlap
        if (other.gameObject.GetComponent<Enemy>() != null)
        {
            FlipSprite();
        }
    }
    
}
