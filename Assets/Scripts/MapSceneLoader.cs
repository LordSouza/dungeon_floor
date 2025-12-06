using UnityEngine;

public class MapSceneLoader : MonoBehaviour
{
    void Start()
    {
        var data = GameManager.Instance.data;
        Player player = FindObjectOfType<Player>();
        
        if (player != null && (data.playerX != 0 || data.playerY != 0))
        {
            player.transform.position = new Vector3(data.playerX, data.playerY, 0);
        }
        
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        foreach (Enemy e in enemies)
        {
            if (!string.IsNullOrEmpty(e.enemyId) && data.deadEnemies.Contains(e.enemyId))
            {
                Destroy(e.gameObject);
            }
        }
    }
}