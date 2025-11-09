using System;

using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D _enemyPlayerRb;
    [SerializeField] GameObject enemyPoof;
    float enemyDir = 1;
    void Awake()
    {
        _enemyPlayerRb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        _enemyPlayerRb.linearVelocityX = 2 * enemyDir;
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
        enemyDir *= -1;
    }
    
}
