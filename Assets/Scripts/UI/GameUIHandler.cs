using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIHandler : MonoBehaviour
{
    [SerializeField] private Text _timerText;

    [SerializeField] private Color _timerNearEndColor;

    [SerializeField] private int _timerFlickerFrequency = 3;

    [SerializeField] private Text _scoreText;

    [SerializeField] private Color _scoreUpdateColor;

    [SerializeField] private int _scoreFlickerFrequency = 1;

    [SerializeField] private float _flickerDelay = 0.1f;

    private Color _timerBaseColor;

    private Color _scoreBaseColor;

    private WaitForSeconds _flickerWaitTime;

    public void OnUpdateTimer()
    {
        _timerText.text = GameManager.Instance.CurrentTimer.ToString("F2");
    }

    public void OnUpdateScore()
    {
        _scoreText.text = GameManager.Instance.CurrentScore.ToString("F2");
    }

    private void Awake()
    {
        _timerBaseColor = _timerText.color;
        _scoreBaseColor = _scoreText.color;
    }

    private void Start()
    {
        _flickerWaitTime = new WaitForSeconds(_flickerDelay);
    }

    [ContextMenu("Flicker Timer")]
    public void FlickerTimerNearEndColor()
    {
        StartCoroutine(FlickerTextUiColorRoutine(_timerText, _timerBaseColor, _timerNearEndColor, _timerFlickerFrequency));
    }

    [ContextMenu("Flicker Score")]
    public void FlickerScoreUpdateColor()
    {
        StartCoroutine(FlickerTextUiColorRoutine(_scoreText, _scoreBaseColor, _scoreUpdateColor, _scoreFlickerFrequency));
    }

    private IEnumerator FlickerTextUiColorRoutine(Text textUi, Color baseColor, Color flickerColor, int flickerFrequency)
    {
        yield return _flickerWaitTime;
        for (int i = 0; i < flickerFrequency; ++i)
        {
            textUi.color = flickerColor;
            yield return _flickerWaitTime;
            textUi.color = baseColor;
            yield return _flickerWaitTime;
        }
    }
}
