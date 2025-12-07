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
    
    private bool playerInRange = false;
    private bool isInteracting = false;

    void Start()
    {
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
        // Verificar se é o player (diretamente ou através do parent)
        bool isPlayer = other.CompareTag("Player");
        
        // Se não for, verificar se o parent é o Player
        if (!isPlayer && other.transform.parent != null)
        {
            isPlayer = other.transform.parent.CompareTag("Player");
        }
        
        // Garantir que NÃO é um inimigo
        bool isEnemy = other.GetComponent<Enemy>() != null || 
                       (other.transform.parent != null && other.transform.parent.GetComponent<Enemy>() != null);
        
        if (isPlayer && !isEnemy)
        {
            playerInRange = true;
            ShowPrompt();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Player saiu da área
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HidePrompt();
        }
    }

    void ShowPrompt()
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(true);
        }
    }

    void HidePrompt()
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
        }
    }

    void StartFishing()
    {
        isInteracting = true;
        HidePrompt();
        
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
