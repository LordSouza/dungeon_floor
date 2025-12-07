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
        // Verificar input apenas se o player está perto e não está interagindo
        if (playerInRange && !isInteracting && Input.GetKeyDown(KeyCode.F))
        {
            StartFishing();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar se é o player
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowPrompt();
            Debug.Log("Player entrou na área do barco de pesca");
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
        Player player = FindObjectOfType<Player>();
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
