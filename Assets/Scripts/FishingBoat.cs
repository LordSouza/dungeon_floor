using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FishingBoat : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private string promptMessage = "Pressione F para pescar";
    
    [Header("Scene Settings")]
    [SerializeField] private string fishingSceneName = "FishingScene";
    
    [Header("Debug")]
    [SerializeField] private bool showDebugMessages = true;
    [SerializeField] private bool useConsolePrompt = true; // Usar Console se UI não configurada
    
    private bool playerInRange = false;
    private bool isInteracting = false;

    void Start()
    {
        Debug.Log("=== FISHING BOAT START ===");
        
        // Verificar se UI está configurada
        if (promptPanel == null && showDebugMessages)
        {
            Debug.LogWarning("FishingBoat: Prompt Panel não configurado! Configure no Inspector ou ative 'Use Console Prompt'", this);
        }
        else if (promptPanel != null)
        {
            Debug.Log("Prompt Panel configurado corretamente!");
        }
        
        if (promptText != null)
        {
            Debug.Log("Prompt Text configurado corretamente!");
        }
        else if (showDebugMessages)
        {
            Debug.LogWarning("Prompt Text NÃO configurado!");
        }
        
        if (promptText == null && showDebugMessages)
        {
            Debug.LogWarning("FishingBoat: Prompt Text não configurado!", this);
        }
        
        // Esconder o prompt no início
        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
        }
        
        if (promptText != null)
        {
            promptText.text = promptMessage;
        }
    }

    void Update()
    {
        // Debug: Mostrar distância até o player
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            Debug.Log($"Distância até Player: {distance:F2} | Player em Range: {playerInRange}");
        }
        
        // Verificar input apenas se o player está perto e não está interagindo
        if (playerInRange && !isInteracting && Input.GetKeyDown(KeyCode.F))
        {
            StartFishing();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($">>> TRIGGER ENTER! GameObject: {other.gameObject.name} | Tag: '{other.tag}' | Layer: {other.gameObject.layer}");
        
        // Verificar se é o player
        if (other.CompareTag("Player"))
        {
            Debug.Log("✓ Tag 'Player' detectada! Mostrando prompt...");
            playerInRange = true;
            ShowPrompt();
        }
        else
        {
            Debug.LogError($"✗ Tag INCORRETA! Detectou: '{other.tag}' mas esperava: 'Player'");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"<<< TRIGGER EXIT! GameObject: {other.gameObject.name}");
        
        // Player saiu da área
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player saiu da área do barco");
            playerInRange = false;
            HidePrompt();
        }
    }

    void ShowPrompt()
    {
        Debug.Log("ShowPrompt() chamado");
        if (promptPanel != null)
        {
            promptPanel.SetActive(true);
            Debug.Log("Prompt Panel ATIVADO");
        }
        else
        {
            Debug.LogWarning("ShowPrompt: Panel é NULL!");
        }
    }

    void HidePrompt()
    {
        Debug.Log("HidePrompt() chamado");
        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
            Debug.Log("Prompt Panel DESATIVADO");
        }
    }

    void StartFishing()
    {
        Debug.Log(">>> INICIANDO PESCA! <<<");
        isInteracting = true;
        HidePrompt();
        
        // Salvar a posição atual do jogador antes de mudar de cena
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            Debug.Log($"Salvando posição do player: ({player.transform.position.x}, {player.transform.position.y})");
            GameManager.Instance.data.playerX = player.transform.position.x;
            GameManager.Instance.data.playerY = player.transform.position.y;
            GameManager.Instance.Save();
        }
        else
        {
            Debug.LogWarning("Player não encontrado ao tentar salvar posição!");
        }
        
        Debug.Log($"Carregando cena: {fishingSceneName}");
        // Mudar para a cena de pesca
        SceneManager.LoadScene(fishingSceneName);
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar a área de interação no editor
        Gizmos.color = Color.cyan;
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
        }
    }
    
    void OnDrawGizmos()
    {
        // Sempre mostrar a área do trigger (não precisa selecionar)
        Gizmos.color = new Color(0, 1, 1, 0.3f); // Cyan transparente
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Vector3 center = transform.position + (Vector3)boxCollider.offset;
            Gizmos.DrawCube(center, boxCollider.size);
            
            // Desenhar borda
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(center, boxCollider.size);
        }
    }
}
