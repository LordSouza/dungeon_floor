using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI finalScore;
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void Start()
    {
        if (finalScore != null)
        {
            finalScore.text = "Score: " + PlayerPrefs.GetInt("Score");
        }
    }
}
