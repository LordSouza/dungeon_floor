using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIController : MonoBehaviour
{
    public SaveData data = new SaveData();
    string _savePath;
    
    private void Awake()
    {
        _savePath = Application.persistentDataPath + "/save.json";
    }
    public void StartGame()
    {
        SceneManager.LoadScene("MapScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
    
    public void Resetar()
    {
        GameManager.Instance.ResetSave();
        GameManager.Instance.Save();
    }
    
    public void GotoMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
