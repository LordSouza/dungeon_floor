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
    }

    void FixedUpdate()
    {
        _enemyPlayerRb.linearVelocityX = 2 * _enemyDir;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Foreground"))    
            FlipSprite();
        // Não destruir ao colidir com Player - a batalha é iniciada pelo Player.cs
    }

    public void Destroy()
    {
        GameObject poof = Instantiate(enemyPoof, transform.position, Quaternion.identity);
        // Garantir que o efeito seja destruído mesmo se a cena mudar
        Destroy(poof, 0.8f);
        Destroy(gameObject);
    }

    void FlipSprite()
    {
        transform.localScale = new Vector3(-Mathf.Sign(_enemyPlayerRb.linearVelocityX),1, 1);
        _enemyDir *= -1;
    }
    
}
