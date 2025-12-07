using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform initialSpawnPoint; 
    public GameObject player;         

    void Start()
    {
        var data = GameManager.Instance.data;

        if (!data.hasSpawnedOnce)
        {
            player.transform.position = initialSpawnPoint.position;

            data.playerX = initialSpawnPoint.position.x;
            data.playerY = initialSpawnPoint.position.y;
            data.hasSpawnedOnce = true;

            GameManager.Instance.Save();
        }
        else
        {
            player.transform.position = new Vector2(data.playerX, data.playerY);
        }
    }

}