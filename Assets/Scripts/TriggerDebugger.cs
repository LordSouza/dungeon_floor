using UnityEngine;

/// <summary>
/// Script de DEBUG para testar detecção de triggers
/// Adicione este script ao FishingBoat TEMPORARIAMENTE para diagnosticar
/// </summary>
public class TriggerDebugger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"<color=green>✓ TRIGGER DETECTADO!</color> GameObject: {other.gameObject.name}, Tag: {other.tag}");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"<color=yellow>✗ TRIGGER SAIU!</color> GameObject: {other.gameObject.name}");
    }

    void OnDrawGizmos()
    {
        // Desenhar área do trigger em verde
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + (Vector3)box.offset, box.size);
        }
    }
}
