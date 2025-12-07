using TMPro;
using UnityEngine;

public class FishingSpot : MonoBehaviour
{
    public FishingMinigame fishingMinigame;
    public TextMeshProUGUI promptText; // Optional: "Press E to Fish"
    public KeyCode interactKey = KeyCode.E;
    
    private bool playerInRange = false;
    
    void Start()
    {
        if (fishingMinigame == null)
        {
            fishingMinigame = FindFirstObjectByType<FishingMinigame>();
        }
        
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            StartFishing();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (promptText != null)
            {
                promptText.gameObject.SetActive(true);
                promptText.text = $"Press {interactKey} to Fish";
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }
        }
    }
    
    void StartFishing()
    {
        if (fishingMinigame != null)
        {
            fishingMinigame.StartFishing();
        }
    }
}
