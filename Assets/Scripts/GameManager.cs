using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Serializable]
    public struct Round
    {
        public int _number;
        public int _timer;
        public Vector2 _boardSize;
        public int _numberedTileCount;       // number of 'numbered tiles' on this round
    }

    public static GameManager Instance { get; private set; }

    public int CurrentRoundNumber { get; private set; }

    public int RemainedHiddenTiles { get; private set; }

    public float CurrentTimer { get; set; }

    public float CurrentScore { get; private set; }

    public float LastRoundScore { get; private set; }

    [SerializeField] private float _playerStartingHeight = 8f;

    [SerializeField] private Round[] _rounds;

    [SerializeField] private float _startRoundDelay = 2.0f;

    [SerializeField] private float _flickerDuration = 1.0f;

    [SerializeField] private int _countdownDuration = 3;

    [SerializeField] private float _delayOffset = 1.5f;

    [SerializeField] private bool _isAutoAdvance = true;

    [SerializeField] private float _advanceNextRoundDelay = 2.0f;

    [SerializeField] private UnityEvent _initRoundEvent;

    [SerializeField] private UnityEvent _timerEvent;

    [SerializeField] private UnityEvent _scoreEvent;

    [SerializeField] private UnityEvent _timerNearEndEvent;

    private bool _isTimerOn;

    private int _cachedHiddenTiles;

    private BoardManager _boardManager;

    private SpawnManager _spawnManager;

    private AudioManager _audioManager;

    private GameUIHandler _uiHandler;

    private PlayerController _playerController;

    private Camera _camera;

    private Vector2 _vector2Zero;

    private WaitForSeconds _startRoundWaitTime;

    public int GetNumberTriggeredTiles() => _boardManager.GetNumberTriggeredTiles();

    private int GetTotalRounds() => _rounds.Length;

    private int GetRoundTimer(int number) => (number >= 0 && number <= _rounds.Length) ? _rounds[number - 1]._timer : 0;

    private Vector2 GetRoundBoardSize(int number) => (number >= 0 && number <= _rounds.Length) ? _rounds[number - 1]._boardSize : _vector2Zero;

    private int GetRoundNumberedTiles(int number) => (number >= 0 && number <= _rounds.Length) ? _rounds[number - 1]._numberedTileCount : 0;

    public void SpawnPlayer()
    {
        if (!_boardManager.IsInitBoard) return;

        // Destroy current player to spawn new
        var playerScript = GameObject.FindObjectOfType<PlayerController>();
        if (playerScript != null)
        {
            Destroy(playerScript.gameObject);
        }

        Vector3 spawnPosition = _boardManager.MidPoint;
        spawnPosition.y = _playerStartingHeight;

        var playerObject = _spawnManager.SpawnPlayer(spawnPosition);
        _playerController = playerObject?.GetComponent<PlayerController>();
    }

    public void UpdateRemainedTiles(GameObject tile, bool isValid)
    {
        if (!isValid || tile.GetComponent<SpecialTile>() == null) return;

        // Pre-update check
        if (RemainedHiddenTiles <= 0 || !_isTimerOn)
        {
            return;
        }

        --RemainedHiddenTiles;

        // Post-udpate check
        if (RemainedHiddenTiles <= 0)
        {
            _isTimerOn = false;

            LastRoundScore = CurrentScore;
            CurrentScore += CurrentTimer;

            _uiHandler.ShowEndOfRoundUi();
        }
    }

    public void ResetRemainedTiles()
    {
        RemainedHiddenTiles = _cachedHiddenTiles;
    }

    public bool SetupRound()
    {
        if (++CurrentRoundNumber > GetTotalRounds()) return false;

        _initRoundEvent.Invoke();

        CurrentTimer = GetRoundTimer(CurrentRoundNumber);
        _timerEvent.Invoke();

        RemainedHiddenTiles = _boardManager.GenerateBoard(GetRoundNumberedTiles(CurrentRoundNumber), GetRoundBoardSize(CurrentRoundNumber));
        _cachedHiddenTiles = RemainedHiddenTiles;

        SpawnPlayer();

        return true;
    }

    public void StartNextRound()
    {
        if (!SetupRound()) return;
        StartCoroutine(PreRoundProcessRoutine());
    }

    public void Restart()
    {
        CurrentRoundNumber = 0;
        LastRoundScore = CurrentScore = 0;
        _scoreEvent.Invoke();

        _audioManager.ToggleBGM(true);

        StartNextRound();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void EnablePlayState()
    {
        _playerController.IsPlayable = true;
        _isTimerOn = true;
    }

    public void DisablePlayState()
    {
        _playerController.IsPlayable = false;
        _isTimerOn = false;
    }

    public void ProcessEndOfRound()
    {
        if (_isAutoAdvance)
        {
            StartCoroutine(AdvanceRoundRoutine());
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _boardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        _uiHandler = GameObject.Find("Canvas").GetComponent<GameUIHandler>();
        _camera = Camera.main;
    }

    private void Start()
    {
        InitRoundInfo();

        CurrentRoundNumber = 0;
        RemainedHiddenTiles = 0;
        CurrentTimer = 0;
        LastRoundScore = CurrentScore = 0;

        _isTimerOn = false;
        _vector2Zero = Vector2.zero;
        _startRoundWaitTime = new WaitForSeconds(_startRoundDelay);

        StartNextRound();
    }

    private void Update()
    {
        if (_isTimerOn)
        {
            CurrentTimer = Mathf.Clamp(CurrentTimer - Time.deltaTime, 0f, CurrentTimer);
            _timerEvent.Invoke();

            if (CurrentTimer <= 0)
            {
                ProcessGameOver();
            }
            else if (CurrentTimer <= 5 && (CurrentTimer + Time.deltaTime) > 5)
            {
                //Debug.Log("Less than 5 seconds left");
                _timerNearEndEvent.Invoke();
            }
        }
    }

    private void InitRoundInfo()
    {
        const int totalRounds = 5;
        const int roundTimer = 15;
        const int complexRoundStartIndex = 3;

        Vector2 boardSize66 = new Vector2(6, 6);
        Vector2 boardSize88 = new Vector2(8, 8);

        _rounds = new Round[totalRounds];

        for (int i = 0; i < totalRounds; ++i)
        {
            _rounds[i]._number = i + 1;
            _rounds[i]._timer = roundTimer;
            _rounds[i]._boardSize = boardSize66;
            _rounds[i]._numberedTileCount = 0;

            if (i >= complexRoundStartIndex)
            {
                _rounds[i]._timer *= 2;
                _rounds[i]._boardSize = boardSize88;
                _rounds[i]._numberedTileCount = UnityEngine.Random.Range(1, 3);
            }
        }
    }

    private void ProcessGameOver()
    {
        DisablePlayState();
        _uiHandler.ShowGameOverScreen(false);
        _audioManager.ToggleBGM(false);
    }

    private IEnumerator PreRoundProcessRoutine()
    {
        yield return _startRoundWaitTime;

        float duration = _uiHandler.ShowRoundUi(CurrentRoundNumber);
        _audioManager.PlayRoundSignalAudio();
        yield return new WaitForSeconds(duration + _delayOffset / 2);

        // Flickering time
        _boardManager.FlickerBoard(_flickerDuration);
        _audioManager.PlayFlickerSignalAudio();

        // Countdown to start
        yield return new WaitForSeconds(_flickerDuration + _delayOffset);
        _uiHandler.ShowCountdownUi(_countdownDuration);

        // Note: playing state will be enabled by GameUIHandler when countdown-routine is done
    }

    private IEnumerator AdvanceRoundRoutine()
    {
        yield return new WaitForSeconds(_advanceNextRoundDelay);

        if (CurrentRoundNumber >= GetTotalRounds())
        {
            _uiHandler.ShowGameOverScreen(true);
            _audioManager.ToggleBGM(false);
        }
        else
        {
            StartNextRound();
        }
    }
}
