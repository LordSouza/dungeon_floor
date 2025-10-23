using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    private int _score;
    
    public void AddScore()
    { 
        int currentScore = int.Parse(scoreText.text.ToString());
        _score = currentScore + 10;
        scoreText.text = _score.ToString();
        PlayerPrefs.SetInt("Score", _score);
        PlayerPrefs.Save();
    }
    
}
