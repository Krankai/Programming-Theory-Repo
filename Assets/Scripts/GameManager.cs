using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float _playerStartingHeight = 8f;

    private BoardManager _boardManager;

    private SpawnManager _spawnManager;

    private Camera _camera;

    #region Helpers
    [ContextMenu("Generate Board 1")]
    public void GenerateBoardHelper1()
    {
        _boardManager.GenerateBoard(Random.Range(0, 3));
    }

    [ContextMenu("Generate Board 2")]
    public void GenerateBoardHelper2()
    {
        _boardManager.GenerateBoard(0);
    }

    [ContextMenu("Flick Board")]
    public void FlickBoard()
    {
        _boardManager.FlickerBoard(1.0f);
    }

    [ContextMenu("Clear Board")]
    public void ClearBoard()
    {
        _boardManager.ClearBoard();
    }

    [ContextMenu("Spawn Player")]
    public void SpawnPlayer()
    {
        if (!_boardManager.IsInitBoard) return;

        Vector3 spawnPosition = _boardManager.MidPoint;
        spawnPosition.y = _playerStartingHeight;

        _spawnManager.SpawnPlayer(spawnPosition);
    }
    #endregion

    private void Awake()
    {
        _boardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        
        _camera = Camera.main;
    }

    private void Update()
    {
        DebugDetectMouseClick();
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
}
