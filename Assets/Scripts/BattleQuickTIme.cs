using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum QteType
{
    Attack,
    Heal
}
public class QteController : MonoBehaviour
{

    public TMP_Text sequenceText;
    public TMP_Text timerText;
    
    public int minLength = 3;
    public int maxLength = 5;
    public float timeLimit = 3f;

    private float _timer;
    private bool _qteActive;

    private List<char> _currentSequence = new List<char>();
    private int _playerIndex;

    private BattleSystem _battleSystem;
    private QteType _currentType;
    
    private Dictionary<char, KeyCode> _keyMap = new Dictionary<char, KeyCode>()
    {
        { 'A', KeyCode.UpArrow },
        { 'B', KeyCode.DownArrow  },
        { 'C', KeyCode.RightArrow },
        { 'D', KeyCode.LeftArrow }
    };
    

    void Start()
    {
        _battleSystem = UnityEngine.Object.FindFirstObjectByType<BattleSystem>();
        sequenceText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!_qteActive) return;
        
        _timer -= Time.deltaTime;
        timerText.text = _timer.ToString("0.0");

        if (_timer <= 0f)
        {
            FailQte();
            return;
        }
        
        if (_playerIndex < _currentSequence.Count)
        {
            char expected = _currentSequence[_playerIndex];

            if (Input.GetKeyDown(_keyMap[expected]))
            {
                _playerIndex++;
                UpdateSequenceVisual();

                if (_playerIndex >= _currentSequence.Count)
                {
                    CompleteQte();
                }
            }
        }
    }

    public void StartQte(QteType type)
    {
        _currentType = type;

        sequenceText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);

        _currentSequence.Clear();
        _playerIndex = 0;
        _qteActive = true;

        _timer = timeLimit;

        int length = Random.Range(minLength, maxLength + 1);
        char[] pool = { 'A', 'B', 'C', 'D' };

        for (int i = 0; i < length; i++)
            _currentSequence.Add(pool[Random.Range(0, pool.Length)]);

        UpdateSequenceVisual();
    }

    private void UpdateSequenceVisual()
    {
        string result = "";
        for (int i = 0; i < _currentSequence.Count; i++)
        {
            if (i < _playerIndex)
                result += $"<color=green>{_currentSequence[i]}</color> ";
            else
                result += $"{_currentSequence[i]} ";
        }

        sequenceText.text = result;
    }

    private void CompleteQte()
    {
        _qteActive = false;
        sequenceText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);

        int seqSize = _currentSequence.Count;
        int bonus = seqSize * 2; 

        if (_currentType == QteType.Attack)
        {
            _battleSystem.extraDamageFromQte = bonus;
            _battleSystem.StartCoroutine(_battleSystem.PlayerAttack());
        }
        else if (_currentType == QteType.Heal)
        {
            _battleSystem.extraHealingFromQte = bonus;
            _battleSystem.StartCoroutine(_battleSystem.PlayerHeal());
        }
    }

    private void FailQte()
    {
        _qteActive = false;
        sequenceText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);

        _battleSystem.QteFailed(); 
    }
}
