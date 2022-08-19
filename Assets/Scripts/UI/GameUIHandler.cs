using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIHandler : MonoBehaviour
{
    [SerializeField] private Text _timerText;

    [SerializeField] private Text _scoreText;

    public void OnUpdateTimer()
    {
        _timerText.text = GameManager.Instance.CurrentTimer.ToString("F2");
    }

    public void OnUpdateScore()
    {
        _scoreText.text = GameManager.Instance.CurrentScore.ToString("F2");
    }
}
