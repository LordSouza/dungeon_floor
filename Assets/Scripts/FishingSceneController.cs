using UnityEngine;

/// <summary>
/// Script para iniciar automaticamente o minigame de pesca quando a FishingScene carrega
/// Adicione este script em um GameObject na FishingScene
/// </summary>
public class FishingSceneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FishingMinigame fishingMinigame;
    
    void Start()
    {
        Debug.Log("=== FishingSceneController Start() ===");
        
        // Iniciar o minigame automaticamente
        if (fishingMinigame != null)
        {
            Debug.Log($"Iniciando minigame em: {fishingMinigame.gameObject.name}");
            fishingMinigame.StartFishing();
        }
        else
        {
            Debug.LogError("FishingMinigame não está atribuído no FishingSceneController! Arraste o componente no Inspector!");
        }
    }
}
