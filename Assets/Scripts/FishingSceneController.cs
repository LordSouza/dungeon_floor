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
        FishingMinigame fishingMinigame = FindFirstObjectByType<FishingMinigame>();
        
        if (fishingMinigame != null)
        {
            StartCoroutine(StartFishingAfterDelay(fishingMinigame, 0.1f));
        }
        else
        {
            Debug.LogError("FishingMinigame não encontrado! Adicione o componente na cena.");
        }
    }
    
    System.Collections.IEnumerator StartFishingAfterDelay(FishingMinigame minigame, float delay)
    {
        yield return new WaitForSeconds(delay);
        minigame.StartFishing();
    }
}
