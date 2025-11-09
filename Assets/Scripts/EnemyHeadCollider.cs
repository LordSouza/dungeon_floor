using System;

using UnityEngine;

public class EnemyHeadCollider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.transform.parent.CompareTag("Player"))
        {
            GetComponentInParent<Enemy>().Destroy();
        }
    }
}
