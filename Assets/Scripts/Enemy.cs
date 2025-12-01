using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    Rigidbody2D _enemyPlayerRb;
    [SerializeField] GameObject enemyPoof;
    public string enemyId;
    float _enemyDir = 1;
    
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
            Destroy();
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
