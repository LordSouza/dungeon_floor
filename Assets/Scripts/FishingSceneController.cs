using UnityEngine;

/// <summary>
/// Script para iniciar automaticamente o minigame de pesca quando a FishingScene carrega
/// Adicione este script em QUALQUER GameObject na FishingScene (pode ser vazio)
/// Não precisa configurar nada - busca automaticamente!
/// </summary>
public class FishingSceneController : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== FishingSceneController Start() ===");
        
        // Buscar FishingMinigame automaticamente na cena
        FishingMinigame fishingMinigame = FindFirstObjectByType<FishingMinigame>();
        
        if (fishingMinigame != null)
        {
            Debug.Log($"FishingMinigame encontrado em: {fishingMinigame.gameObject.name}");
            
            // Pequeno delay para garantir que tudo inicializou
            StartCoroutine(StartFishingAfterDelay(fishingMinigame, 0.1f));
        }
        else
        {
            Debug.LogError("FishingMinigame NÃO encontrado na cena! Adicione o componente FishingMinigame em algum GameObject!");
        }
    }
    
    System.Collections.IEnumerator StartFishingAfterDelay(FishingMinigame minigame, float delay)
    {
        yield return new UnityEngine.WaitForSeconds(delay);
        
        Debug.Log("Iniciando pesca...");
        minigame.StartFishing();
    }
}
