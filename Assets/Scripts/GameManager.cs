using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // Timer for each round
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

    public float CurrentTimer { get; private set; }

    [SerializeField] private float _playerStartingHeight = 8f;

    [SerializeField] private Round[] _rounds;

    [SerializeField] private UnityEvent _failEndOfRoundEvent;

    [SerializeField] private UnityEvent _succeedEndOfRoundEvent;

    [SerializeField] private UnityEvent _countdownEvent;

    [SerializeField] private float _startRoundDelay = 2.0f;

    [SerializeField] private float _flickerDuration = 1.0f;

    [SerializeField] private int _countdownDuration = 3;

    [SerializeField] private float _countdownDelayOffset = 1.5f;

    [SerializeField] private bool _isAutoAdvance = true;

    [SerializeField] private float _advanceNextRoundDelay = 3.0f;

    private BoardManager _boardManager;

    private SpawnManager _spawnManager;

    private PlayerController _playerController;

    private Camera _camera;

    private bool _isTimerOn;

    private Vector2 _vector2Zero;

    private int GetTotalRounds() => _rounds.Length;

    private int GetRoundTimer(int number) => number < _rounds.Length ? _rounds[number - 1]._timer : 0;

    private Vector2 GetRoundBoardSize(int number) => number < _rounds.Length ? _rounds[number - 1]._boardSize : _vector2Zero;

    private int GetRoundNumberedTiles(int number) => number < _rounds.Length ? _rounds[number - 1]._numberedTileCount : 0;

    #region Helpers
    [ContextMenu("Generate Board 1")]
    public void GenerateBoardHelper1()
    {
        _boardManager.GenerateBoard(UnityEngine.Random.Range(0, 3));
    }

    [ContextMenu("Generate Board 2")]
    public void GenerateBoardHelper2()
    {
        _boardManager.GenerateBoard(0);
    }

    [ContextMenu("Flick Board")]
    public void FlickBoardHelper()
    {
        _boardManager.FlickerBoard(1.0f);
    }

    [ContextMenu("Clear Board")]
    public void ClearBoardHelper()
    {
        _boardManager.ClearBoard();
    }

    [ContextMenu("Start Round")]
    public void StartRoundHelper()
    {
        StartNextRound();
    }

    [ContextMenu("Restart")]
    public void RestartHelper()
    {
        Restart();
    }
    #endregion

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

    public void UpdateRemainedTiles()
    {
        // Pre-update check
        if (RemainedHiddenTiles <= 0)
        {
            Debug.LogWarning("No hidden tiles left...");
            return;
        }
        else if (!_isTimerOn)
        {
            Debug.LogWarning("Out of time!!!");
            return;
        }

        Debug.Log($"Update hidden tiles: {--RemainedHiddenTiles}");

        // Post-udpate check
        if (RemainedHiddenTiles <= 0)
        {
            _succeedEndOfRoundEvent.Invoke();
            _isTimerOn = false;

            Debug.Log($"Finish in {GetRoundTimer(CurrentRoundNumber) - CurrentTimer} seconds");
            //CurrentTimer = 0;
        }
    }

    public void SucceedEndOfRound()
    {
        Debug.Log("You succeed in passing this round. Congrats.");

        if (_isAutoAdvance)
        {
            StartCoroutine(AdvanceRoundRoutine());
        }
    }

    public void FailEndOfRound()
    {
        Debug.Log("Time's up. You fail to pass this round. Good luck next time.");
    }

    public bool SetupRound()
    {
        if (++CurrentRoundNumber > GetTotalRounds()) return false;

        CurrentTimer = GetRoundTimer(CurrentRoundNumber);
        RemainedHiddenTiles = _boardManager.GenerateBoard(GetRoundNumberedTiles(CurrentRoundNumber), GetRoundBoardSize(CurrentRoundNumber));
        SpawnPlayer();

        Debug.Log($"Round {CurrentRoundNumber}: {RemainedHiddenTiles} tiles");

        return true;
    }

    public void StartNextRound()
    {
        if (!SetupRound()) return;

        //const int countdownDuration = 3;
        //const float countdownDelay = 1.5f;
        //StartCoroutine(CountdownToStartRoutine(countdownDelay, countdownDuration, CurrentRoundNumber));
        StartCoroutine(PreRoundProcessRoutine());
    }

    public void Restart()
    {
        CurrentRoundNumber = 0;
        StartNextRound();
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
        
        _camera = Camera.main;
    }

    private void Start()
    {
        InitRoundInfo();

        CurrentRoundNumber = 0;
        RemainedHiddenTiles = 0;
        CurrentTimer = 0;

        _isTimerOn = false;

        _vector2Zero = Vector2.zero;
    }

    private void Update()
    {
        DebugDetectMouseClick();

        if (_isTimerOn)
        {
            CurrentTimer = Mathf.Clamp(CurrentTimer - Time.deltaTime, 0f, CurrentTimer);
            //Debug.Log("timer: " + CurrentTimer);

            if (CurrentTimer <= 0)
            {
                _isTimerOn = false;
                _failEndOfRoundEvent.Invoke();
            }
            else if (CurrentTimer <= 10 && (CurrentTimer + Time.deltaTime) > 10)
            {
                Debug.Log("Less than 10 seconds left");
            }
            else if (CurrentTimer <= 5 && (CurrentTimer + Time.deltaTime) > 5)
            {
                Debug.Log("Less than 5 seconds left");
            }
        }
    }

    private void DebugDetectMouseClick()
    {
        bool isLeftClick = Input.GetMouseButtonDown(0);
        if (isLeftClick || Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Tile"))
                {
                    Tile tileScript = hit.transform.gameObject.GetComponent<Tile>();
                    if (isLeftClick)
                    {
                        tileScript.FlickerTile(1);
                    }
                    else
                    {
                        if (tileScript.IsTriggered) tileScript.UntriggerTile();
                        else tileScript.TriggerTile();
                    }
                }
            }
        }
    }

    private void InitRoundInfo()
    {
        const int totalRounds = 5;
        const int roundTimer = 15;

        Vector2 boardSize66 = new Vector2(6, 6);
        Vector2 boardSize88 = new Vector2(8, 8);

        _rounds = new Round[totalRounds];

        for (int i = 0; i < totalRounds; ++i)
        {
            _rounds[i]._number = i + 1;
            _rounds[i]._timer = roundTimer;
            _rounds[i]._boardSize = boardSize66;
            _rounds[i]._numberedTileCount = 0;

            if (i >= 3)
            {
                _rounds[i]._timer *= 2;
                _rounds[i]._boardSize = boardSize88;
                _rounds[i]._numberedTileCount = UnityEngine.Random.Range(1, 3);
            }
        }
    }

    private IEnumerator PreRoundProcessRoutine()
    {
        yield return new WaitForSeconds(_startRoundDelay);

        // Flickering time
        _boardManager.FlickerBoard(_flickerDuration);

        // Countdown to start
        yield return new WaitForSeconds(_flickerDuration + _countdownDelayOffset);

        int coundownDuration = _countdownDuration;
        do
        {
            Debug.Log($"Start in {coundownDuration}...");

            _countdownEvent.Invoke();
            yield return new WaitForSeconds(1);
        }
        while (--coundownDuration > 0);
        Debug.Log("Start!");

        _playerController.IsPlayable = true;
        _isTimerOn = true;
    }

    private IEnumerator AdvanceRoundRoutine()
    {
        Debug.Log($"Next round in {_advanceNextRoundDelay} seconds...");
        yield return new WaitForSeconds(_advanceNextRoundDelay);

        StartNextRound();
    }
}
