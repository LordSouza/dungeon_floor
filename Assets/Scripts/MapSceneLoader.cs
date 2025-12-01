using UnityEngine;

public class MapSceneLoader : MonoBehaviour
{
    void Start()
    {
        GameData data = GameData.Load();
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy e in enemies)
        {
            if (data.defeatedEnemies.Contains(e.enemyId))
            {
                Destroy(e.gameObject);
            }
        }
    }
}