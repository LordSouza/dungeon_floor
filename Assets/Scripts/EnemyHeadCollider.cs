using System;

using UnityEngine;

public class EnemyHeadCollider : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar se é o Player diretamente
        if (other.CompareTag("Player"))
        {
            GetComponentInParent<Enemy>()?.Destroy();
            return;
        }
        
        // Verificar se o pai é o Player (para objetos filhos)
        if (other.transform.parent != null && other.transform.parent.CompareTag("Player"))
        {
            GetComponentInParent<Enemy>()?.Destroy();
        }
    }
}
