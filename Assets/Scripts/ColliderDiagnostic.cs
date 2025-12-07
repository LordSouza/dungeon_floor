using UnityEngine;

/// <summary>
/// Script de diagnóstico para debugar problemas de colisão/trigger
/// Adicione este script no FishingBoat para ver informações detalhadas
/// </summary>
public class ColliderDiagnostic : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== DIAGNÓSTICO DE COLLIDER ===");
        Debug.Log($"GameObject: {gameObject.name}");
        Debug.Log($"Layer: {LayerMask.LayerToName(gameObject.layer)} (index: {gameObject.layer})");
        Debug.Log($"Tag: {gameObject.tag}");
        Debug.Log($"Posição: {transform.position}");
        
        // Verificar BoxCollider2D
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Debug.Log($"✓ BoxCollider2D encontrado!");
            Debug.Log($"  - Is Trigger: {boxCollider.isTrigger}");
            Debug.Log($"  - Enabled: {boxCollider.enabled}");
            Debug.Log($"  - Size: {boxCollider.size}");
            Debug.Log($"  - Offset: {boxCollider.offset}");
            
            if (!boxCollider.isTrigger)
            {
                Debug.LogError("⚠️ PROBLEMA! Is Trigger está FALSE! Marque como TRUE no Inspector!");
            }
        }
        else
        {
            Debug.LogError("✗ BoxCollider2D NÃO encontrado!");
        }
        
        // Verificar Rigidbody2D (não necessário mas útil saber)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Debug.Log($"Rigidbody2D: {rb.bodyType}");
        }
        
        Debug.Log("=== FIM DO DIAGNÓSTICO ===\n");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[ColliderDiagnostic] TRIGGER ENTER: {other.gameObject.name}");
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[ColliderDiagnostic] COLLISION ENTER: {collision.gameObject.name}");
    }
}
