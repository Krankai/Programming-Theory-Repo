using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardManager _boardManager;

    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private float _playerStartingHeight = 8f;

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
        _boardManager.FlickBoard(1.0f);
    }

    [ContextMenu("Clear Board")]
    public void ClearBoard()
    {
        _boardManager.ClearBoard();
    }
    #endregion

    [ContextMenu("Spawn Player")]
    public void SpawnPlayer()
    {
        if (!_boardManager.IsInitBoard) return;

        Vector3 spawnPosition = _boardManager.MidPoint;
        spawnPosition.y = _playerStartingHeight;

        GameObject playerObject = Instantiate(_playerPrefab, spawnPosition, _playerPrefab.transform.rotation);
    }

    private void Awake()
    {
        // if (_boardManager == null)
        // {
        //     _boardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();
        // }
        
        _camera = Camera.main;
    }

    private void Update()
    {
        bool isLeftClick = Input.GetMouseButtonDown(0);
        if (isLeftClick || Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("SpecialTile"))
                {
                    SpecialTile tile = hit.transform.gameObject.GetComponent<SpecialTile>();
                    if (isLeftClick)
                        tile.FlickTrueTile(1);
                    else
                        tile.OnTriggered();
                }
            }
        }
    }
}
