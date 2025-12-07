using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FishingMinigame : MonoBehaviour
{
    [Header("UI References")]
    public GameObject fishingUI;
    public Slider progressSlider;
    public RectTransform successZone;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI fishCountText; // Contador de peixes
    public Button exitButton; // Botão para sair
    
    [Header("Settings")]
    public float indicatorSpeed = 2f;
    public float successZoneSize = 0.2f; // 20% of bar
    public int fishHealAmount = 12; // How much HP fish heals
    
    private bool isPlaying = false;
    private bool canCatch = false;
    private float currentProgress = 0f;
    private bool movingRight = true;
    private float successZoneStart;
    private float successZoneEnd;
    
    void Start()
    {
        if (fishingUI != null)
        {
            fishingUI.SetActive(false);
        }
        else
        {
            Debug.LogError("fishingUI é NULL! Configure no Inspector!");
        }
        
        if (progressSlider == null)
            Debug.LogError("progressSlider é NULL! Configure no Inspector!");
    }
    
    void Update()
    {
        if (!isPlaying)
            return;
        
        // Move indicator
        if (movingRight)
        {
            currentProgress += indicatorSpeed * Time.deltaTime;
            if (currentProgress >= 1f)
            {
                currentProgress = 1f;
                movingRight = false;
            }
        }
        else
        {
            currentProgress -= indicatorSpeed * Time.deltaTime;
            if (currentProgress <= 0f)
            {
                currentProgress = 0f;
                movingRight = true;
            }
        }
        
        progressSlider.value = currentProgress;
        
        // Check if in success zone
        canCatch = currentProgress >= successZoneStart && currentProgress <= successZoneEnd;
        
        // Listen for catch attempt
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            AttemptCatch();
        }
        
        // Listen for exit (ESC key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelFishing();
        }
    }
    
    public void StartFishing()
    {
        if (isPlaying)
            return;
        
        if (fishingUI == null || progressSlider == null)
        {
            Debug.LogError("FishingMinigame: Referências não configuradas no Inspector!");
            return;
        }
        
        // Setup exit button
        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(CancelFishing);
        }
        
        UpdateFishCountDisplay();
        
        // Setup success zone (random position)
        successZoneStart = Random.Range(0.1f, 0.9f - successZoneSize);
        successZoneEnd = successZoneStart + successZoneSize;
        
        if (successZone != null)
        {
            successZone.anchorMin = new Vector2(successZoneStart, 0);
            successZone.anchorMax = new Vector2(successZoneEnd, 1);
        }
        
        // Reset state
        currentProgress = 0f;
        movingRight = true;
        isPlaying = true;
        
        // Show UI
        fishingUI.SetActive(true);
        
        if (instructionText != null)
            instructionText.text = "Press SPACE or E when indicator is in GREEN zone!";
        
        if (resultText != null)
            resultText.text = "";
        
        // Pause player movement
        Time.timeScale = 1f; // Keep running for smooth animation
    }
    
    void AttemptCatch()
    {
        isPlaying = false;
        
        if (canCatch)
        {
            // Success!
            GameManager.Instance.data.fishCount++;
            GameManager.Instance.Save();
            
            if (resultText != null)
                resultText.text = $"SUCCESS! You caught a fish!";
            
            UpdateFishCountDisplay();
        }
        else
        {
            // Failed
            if (resultText != null)
                resultText.text = "The fish got away... Try again!";
        }
        
        // Reiniciar após delay (não fechar mais)
        StartCoroutine(RestartFishingAfterDelay(2f));
    }
    
    IEnumerator RestartFishingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Limpar texto de resultado
        if (resultText != null)
            resultText.text = "";
        
        // Reposicionar success zone aleatoriamente
        successZoneStart = Random.Range(0.1f, 0.9f - successZoneSize);
        successZoneEnd = successZoneStart + successZoneSize;
        
        if (successZone != null)
        {
            successZone.anchorMin = new Vector2(successZoneStart, 0);
            successZone.anchorMax = new Vector2(successZoneEnd, 1);
        }
        
        // Reset state
        currentProgress = 0f;
        movingRight = true;
        isPlaying = true;
    }
    
    public void ReturnToMap()
    {
        // Restaurar posição do player antes de voltar
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            GameManager.Instance.data.playerX = player.transform.position.x;
            GameManager.Instance.data.playerY = player.transform.position.y;
            GameManager.Instance.Save();
        }
        
        // Carregar MapScene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MapScene");
    }
    
    public void CancelFishing()
    {
        isPlaying = false;
        ReturnToMap();
    }
    
    void UpdateFishCountDisplay()
    {
        if (fishCountText != null)
        {
            fishCountText.text = $"Fish: {GameManager.Instance.data.fishCount}";
        }
    }
}
