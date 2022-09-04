using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameUIHandler : MonoBehaviour
{
    [SerializeField] private Text _timerText;

    [SerializeField] private Color _timerNearEndColor;

    [SerializeField] private int _timerFlickerFrequency = 3;

    [SerializeField] private Text _scoreText;

    [SerializeField] private Color _scoreUpdateColor;

    [SerializeField] private int _scoreFlickerFrequency = 1;

    [SerializeField] private float _flickerDelay = 0.1f;

    [SerializeField] private Text _roundText;

    [SerializeField] private float _showRoundUiDuration = 1.0f;

    [SerializeField] private Text _clearedText;

    [SerializeField] private float _hideClearUiDelay = 0.4f;

    [SerializeField] private Text _countdownText;

    [SerializeField] private float _transitionStartDelay = 1.0f;

    [SerializeField] private float _transitionDuration = 1.0f;

    [SerializeField] private GameObject _gameOverGroup;

    [SerializeField] private Text _finalScoreText;

    [SerializeField] private UnityEvent _finishStartGameEvent;

    [SerializeField] private UnityEvent _finishEndGameEvent;

    [SerializeField] private UnityEvent _countdownStepEvent;

    [SerializeField] private UnityEvent _countdownFinishEvent;

    private Color _timerBaseColor;

    private Color _scoreBaseColor;

    private Coroutine _timerFlickerCoroutine;

    private WaitForSeconds _flickerWaitTime;

    private bool _isTransitionOn;

    private float _cachedTransitionTimer;

    private float _cachedTransitionScore;

    private float _transitionInitialScore;

    private float _transitionTargetScore;
    
    private float _transitionInitialTimer;

    private float _transitionTimePassed;

    private WaitForSeconds _countdownWaitTime;

    private WaitForSeconds _transitionStartWaitTime;

    private WaitForSeconds _hideClearUiWaitTime;

    private GameOverUIHandler _gameOverUiHandler;

    private void UpdateTimerUi(float value) => _timerText.text = value.ToString("F2");

    private void UpdateScoreUi(float value) => _scoreText.text = value.ToString("F2");

    public void Reset()
    {
        _timerFlickerCoroutine = null;

        _cachedTransitionScore = -1;
        _cachedTransitionTimer = -1;
        _transitionTimePassed = -1;
    }

    public void OnUpdateTimer()
    {
        //_timerText.text = GameManager.Instance.CurrentTimer.ToString("F2");
        UpdateTimerUi(GameManager.Instance.CurrentTimer);
    }

    public void OnUpdateScore()
    {
        //_scoreText.text = GameManager.Instance.CurrentScore.ToString("F2");
        UpdateScoreUi(GameManager.Instance.CurrentScore);
    }

    [ContextMenu("Flicker Timer")]
    public void FlickerTimerNearEndColor()
    {
        _timerFlickerCoroutine = StartCoroutine(FlickerTextUiColorRoutine(_timerText, _timerBaseColor, _timerNearEndColor, _timerFlickerFrequency));
    }

    public void StopFlickerTimer()
    {
        if (_timerFlickerCoroutine != null)
        {
            StopCoroutine(_timerFlickerCoroutine);
        }
    }

    [ContextMenu("Flicker Score")]
    public void FlickerScoreUpdateColor()
    {
        StartCoroutine(FlickerTextUiColorRoutine(_scoreText, _scoreBaseColor, _scoreUpdateColor, _scoreFlickerFrequency));
    }

    public void StartTransitionTimerToScore(float intialScore, float targetScore, float initialTimer)
    {
        _transitionInitialScore = intialScore;
        _transitionTargetScore = targetScore;
        _transitionInitialTimer = initialTimer;

        _cachedTransitionScore = 0;
        _cachedTransitionTimer = initialTimer;
        _transitionTimePassed = 0;

        _isTransitionOn = true;
    }

    public float ShowRoundUi(int roundNumber)
    {
        _roundText.text = $"Round {roundNumber}";
        _roundText.gameObject.SetActive(true);

        Invoke("HideRoundUi", _showRoundUiDuration);

        return _showRoundUiDuration;
    }

    public void ShowCountdownUi(int countdownDuration)
    {
        StartCoroutine(ShowCountdownUiRoutine(countdownDuration));
    }

    public void ShowEndOfRoundUi()
    {
        _clearedText.gameObject.SetActive(true);
        StartCoroutine(StartTransitionRoutine(GameManager.Instance.LastRoundScore, GameManager.Instance.CurrentScore, GameManager.Instance.CurrentTimer));
    }

    public void ShowGameOverScreen(bool isSuccess)
    {
        string scoreText = GameManager.Instance.CurrentScore.ToString("F2");

        _finalScoreText.text = $"Final Score: {scoreText}";
        //_gameOverGroup.SetActive(true);
        _gameOverUiHandler.EnableUiWithSuccess(isSuccess);
    }

    public void HideGameOverScreen()
    {
        //_gameOverGroup.SetActive(false);
        _gameOverUiHandler.DisableUi();
    }

    private void Awake()
    {
        _timerBaseColor = _timerText.color;
        _scoreBaseColor = _scoreText.color;

        _gameOverUiHandler = _gameOverGroup.GetComponent<GameOverUIHandler>();
    }

    private void Start()
    {
        _flickerWaitTime = new WaitForSeconds(_flickerDelay);
        _isTransitionOn = false;

        _countdownWaitTime = new WaitForSeconds(1);
        _transitionStartWaitTime = new WaitForSeconds(_transitionStartDelay);
        _hideClearUiWaitTime = new WaitForSeconds(_hideClearUiDelay);

        Reset();
    }

    private void Update()
    {
        if (_isTransitionOn)
        {
            TransitionTimerToScore();
        }
    }

    private void TransitionTimerToScore()
    {
        _transitionTimePassed = Mathf.Clamp(_transitionTimePassed + Time.deltaTime, 0, _transitionDuration);
        _cachedTransitionTimer = Mathf.Lerp(_transitionInitialTimer, 0, _transitionTimePassed / _transitionDuration);
        _cachedTransitionScore = Mathf.Lerp(_transitionInitialScore, _transitionTargetScore, _transitionTimePassed / _transitionDuration);

        UpdateTimerUi(_cachedTransitionTimer);
        UpdateScoreUi(_cachedTransitionScore);

        if (_transitionTimePassed >= _transitionDuration)
        {
            _isTransitionOn = false;
            StartCoroutine(HideClearUiRoutine());
        }
    }

    private void HideRoundUi()
    {
        _roundText.gameObject.SetActive(false);
    }

    private void HideClearUi()
    {
        _clearedText.gameObject.SetActive(false);
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

    private IEnumerator ShowCountdownUiRoutine(int countdownDuration)
    {
        _roundText.gameObject.SetActive(false);

        // "1..2..3..Start"
        _countdownText.gameObject.SetActive(true);
        for (int i = countdownDuration; i >= 1; --i)
        {
            _countdownStepEvent.Invoke();
            _countdownText.text = $"{i}";

            yield return _countdownWaitTime;
        }

        _countdownFinishEvent.Invoke();
        _countdownText.text = "Start";

        yield return _countdownWaitTime;

        _countdownText.gameObject.SetActive(false);

        _finishStartGameEvent.Invoke();
    }

    private IEnumerator StartTransitionRoutine(float initialScore, float targetScore, float initialTimer)
    {
        yield return _transitionStartWaitTime;
        StartTransitionTimerToScore(initialScore, targetScore, initialTimer);
    }

    private IEnumerator HideClearUiRoutine()
    {
        yield return _hideClearUiWaitTime;

        HideClearUi();
        _finishEndGameEvent.Invoke();
    }
}
