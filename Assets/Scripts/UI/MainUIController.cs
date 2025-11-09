using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MapScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
    
}
