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
        
        // Verificar Player
        Debug.Log("\n--- VERIFICANDO PLAYER ---");
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            Debug.Log($"Player encontrado: {player.gameObject.name}");
            Debug.Log($"Player Layer: {LayerMask.LayerToName(player.gameObject.layer)} (index: {player.gameObject.layer})");
            Debug.Log($"Player Tag: {player.gameObject.tag}");
            Debug.Log($"Player Posição: {player.transform.position}");
            
            // Verificar colisores do player
            Collider2D[] playerColliders = player.GetComponents<Collider2D>();
            Debug.Log($"Player tem {playerColliders.Length} collider(s):");
            foreach (var col in playerColliders)
            {
                Debug.Log($"  - {col.GetType().Name}: IsTrigger={col.isTrigger}, Enabled={col.enabled}");
            }
            
            // Verificar Rigidbody2D do player
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Debug.Log($"Player Rigidbody2D: {playerRb.bodyType}");
            }
            else
            {
                Debug.LogError("✗ Player NÃO tem Rigidbody2D!");
            }
            
            // Verificar se layers podem colidir
            int boatLayer = gameObject.layer;
            int playerLayer = player.gameObject.layer;
            bool canCollide = !Physics2D.GetIgnoreLayerCollision(boatLayer, playerLayer);
            
            if (canCollide)
            {
                Debug.Log($"✓ Layers podem colidir: {LayerMask.LayerToName(boatLayer)} <-> {LayerMask.LayerToName(playerLayer)}");
            }
            else
            {
                Debug.LogError($"✗ PROBLEMA! Layers NÃO podem colidir: {LayerMask.LayerToName(boatLayer)} <-> {LayerMask.LayerToName(playerLayer)}");
                Debug.LogError("Vá em Edit -> Project Settings -> Physics 2D -> Layer Collision Matrix e marque a interseção!");
            }
        }
        else
        {
            Debug.LogWarning("Player não encontrado na cena!");
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
