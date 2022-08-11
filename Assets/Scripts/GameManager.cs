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
    }

    public static GameManager Instance { get; private set; }

    public int CurrentRoundNumber { get; private set; }

    public int RemainedHiddenTiles { get; private set; }

    public float CurrentTimer { get; private set; }

    [SerializeField] private float _playerStartingHeight = 8f;

    [SerializeField] private Round[] _rounds;

    [SerializeField] private UnityEvent _endOfRoundEvent;

    [SerializeField] private UnityEvent _countdownEvent;

    private BoardManager _boardManager;

    private SpawnManager _spawnManager;

    private Camera _camera;

    private bool _isTimerOn;

    private int GetTotalRounds() => _rounds.Length;

    private int GetRoundTimer(int number) => _rounds[number - 1]._timer;

    private Vector2 GetRoundBoardSize(int number) => _rounds[number - 1]._boardSize;

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
        StartNextRound(0);
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

        _spawnManager.SpawnPlayer(spawnPosition);
    }

    public void UpdateRemainedTiles()
    {
        Debug.Log("Update remained tiles...");
    }

    public void EndOfRound()
    {
        Debug.Log("Time's up...");
    }

    public bool SetupRound(int hiddenTiles)
    {
        RemainedHiddenTiles = hiddenTiles;
        if (++CurrentRoundNumber > GetTotalRounds()) return false;

        Debug.Log($"Round {CurrentRoundNumber}");
        CurrentTimer = GetRoundTimer(CurrentRoundNumber);

        _boardManager.GenerateBoard(0, GetRoundBoardSize(CurrentRoundNumber));

        SpawnPlayer();

        return true;
    }

    public void StartNextRound(int hiddenTiles)
    {
        if (!SetupRound(hiddenTiles)) return;

        const int countdownDuration = 3;
        const float countdownDelay = 1.5f;
        StartCoroutine(CountdownToStartRoutine(countdownDelay, countdownDuration, CurrentRoundNumber));
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
    }

    private void Update()
    {
        DebugDetectMouseClick();

        if (_isTimerOn)
        {
            CurrentTimer = Mathf.Clamp(CurrentTimer - Time.deltaTime, 0f, CurrentTimer);
            Debug.Log("timer: " + CurrentTimer);

            if (CurrentTimer <= 0)
            {
                _isTimerOn = false;
                _endOfRoundEvent.Invoke();
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
        const int roundTimer = 10;

        Vector2 boardSize66 = new Vector2(6, 6);
        Vector2 boardSize88 = new Vector2(8, 8);

        _rounds = new Round[totalRounds];

        for (int i = 0; i < totalRounds; ++i)
        {
            _rounds[i]._number = i + 1;
            _rounds[i]._timer = roundTimer;
            _rounds[i]._boardSize = (i >= 3) ? boardSize88 : boardSize66;
        }
    }

    private IEnumerator CountdownToStartRoutine(float delay, int duration, int roundNumber)
    {
        yield return new WaitForSeconds(delay);

        do
        {
            Debug.Log($"Start in {duration}...");

            _countdownEvent.Invoke();
            yield return new WaitForSeconds(1);
        }
        while (--duration > 0);

        _isTimerOn = true;
    }

    // todo: implement scoring system (and check total time need to finish)
}


