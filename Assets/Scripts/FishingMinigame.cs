using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    [Header("UI References")]
    public GameObject fishingUI;
    public Slider progressSlider;
    public RectTransform successZone;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI resultText;
    
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
            fishingUI.SetActive(false);
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
    }
    
    public void StartFishing()
    {
        if (isPlaying)
            return;
        
        // Setup success zone (random position)
        successZoneStart = Random.Range(0.1f, 0.9f - successZoneSize);
        successZoneEnd = successZoneStart + successZoneSize;
        
        // Position success zone visual
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
        if (fishingUI != null)
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
                resultText.text = $"SUCCESS! You caught a fish! (Total: {GameManager.Instance.data.fishCount})";
            
            Debug.Log($"Fishing success! Player now has {GameManager.Instance.data.fishCount} fish");
        }
        else
        {
            // Failed
            if (resultText != null)
                resultText.text = "The fish got away... Try again!";
            
            Debug.Log("Fishing failed!");
        }
        
        // Close UI after delay
        StartCoroutine(CloseAfterDelay(2f));
    }
    
    IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (fishingUI != null)
            fishingUI.SetActive(false);
        
        isPlaying = false;
    }
    
    public void CancelFishing()
    {
        isPlaying = false;
        if (fishingUI != null)
            fishingUI.SetActive(false);
    }
}
