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

        // Verificar se UI está configurada
        if (promptPanel == null && showDebugMessages)
        {
            Debug.LogWarning("FishingBoat: Prompt Panel não configurado! Configure no Inspector ou ative 'Use Console Prompt'", this);
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
        // TESTE: Mostrar sempre no texto se está detectando
        if (promptText != null)
        {
            float distance = Vector3.Distance(transform.position, FindFirstObjectByType<Player>()?.transform.position ?? Vector3.zero);
            promptText.text = $"Distância: {distance:F2}m\nRange: {playerInRange}\nTag detectada: {(FindFirstObjectByType<Player>()?.tag ?? "null")}";
        }
        
        // Verificar input apenas se o player está perto e não está interagindo
        if (playerInRange && !isInteracting && Input.GetKeyDown(KeyCode.F))
        {
            StartFishing();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // LOG DETALHADO - sempre mostra o que colidiu
        Debug.LogWarning($"FishingBoat: Algo colidiu! Nome: '{other.gameObject.name}' | Tag: '{other.tag}' | Layer: {LayerMask.LayerToName(other.gameObject.layer)}");
        
        // Verificar se é o player
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowPrompt();
            Debug.Log("Player entrou na área do barco de pesca");
        }
        else
        {
            Debug.LogError($"FishingBoat: Colidiu mas NÃO é Player! Tag detectada: '{other.tag}' (esperado: 'Player')");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Player saiu da área
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HidePrompt();
            Debug.Log("Player saiu da área do barco de pesca");
        }
    }

    void ShowPrompt()
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(true);
            if (showDebugMessages)
            {
                Debug.Log("FishingBoat: Prompt mostrado (UI Panel)");
            }
        }
        else if (useConsolePrompt)
        {
            // Fallback: mostrar no Console se UI não configurada
            Debug.Log($"<color=cyan>★ {promptMessage} ★</color>");
        }
    }

    void HidePrompt()
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
            if (showDebugMessages)
            {
                Debug.Log("FishingBoat: Prompt escondido");
            }
        }
    }

    void StartFishing()
    {
        isInteracting = true;
        HidePrompt();
        
        Debug.Log("Iniciando pesca - mudando para " + fishingSceneName);
        
        // Salvar a posição atual do jogador antes de mudar de cena
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            GameManager.Instance.data.playerX = player.transform.position.x;
            GameManager.Instance.data.playerY = player.transform.position.y;
            GameManager.Instance.Save();
        }
        
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
}
