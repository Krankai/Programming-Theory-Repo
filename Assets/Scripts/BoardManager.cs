using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public bool IsInitBoard { get; private set; }

    [SerializeField] private GameObject _normalTilePrefab;

    [SerializeField] private GameObject _specialTilePrefab;

    [SerializeField] private GameObject _numberedTilePrefab;

    [SerializeField] private int _rows;

    [SerializeField] private int _columns;

    private Tile _normalTileScript;

    private SpecialTile _specialTileScript;

    private TileType[,] _tileMatrix;

    private bool _hasDistributedTiles;

    [ContextMenu("Distribute Tiles")]
    public void DistributeTiles()
    {
        int specialTileCount = _rows;       // each row have 1
        int normalTileCount = _rows * _columns - specialTileCount;

        for (int i = 0; i < _rows; ++i)
        {
            bool isSetSpecial = false;

            for (int j = 0; j < _columns; ++j)
            {
                if (!isSetSpecial)
                {
                    bool isValid = (j == _columns - 1) || (Random.Range(0, _columns) % _columns == 0);
                    if (isValid)
                    {
                        isSetSpecial = true;
                        _tileMatrix[i, j] = TileType.Special;

                        continue;
                    }
                }

                _tileMatrix[i, j] = TileType.Normal;
            }
        }

        _hasDistributedTiles = true;

        // DEBUG TEST
        // for (int i = 0; i < _rows; ++i)
        // {
        //     string rowWeights = "";
        //     for (int j = 0; j < _columns; ++j)
        //     {
        //         int temp = (int)_tileMatrix[i, j];
        //         rowWeights += $"{temp} ";
        //     }

        //     Debug.Log(i + ": " + rowWeights);
        // }
    }

    [ContextMenu("Generate Board")]
    public void GenerateBoard()
    {
        if (!_hasDistributedTiles)
        {
            Debug.LogWarning("Need to distribute tiles first!");
            DistributeTiles();
        }

        // Generate normal tile
        //SpawnNormalTiles(_rows, _columns);
        SpawnTiles();

        IsInitBoard = true;
    }

    [ContextMenu("Clear Board")]
    public void ClearBoard()
    {
        int total = transform.childCount;
        for (int i = 0; i < total; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        IsInitBoard = false;
    }

    // Flick true state of all tiles on the board on
    [ContextMenu("Flick Board")]
    public void FlickBoard()
    {
        // NOTE: bring this to argument later
        float duration = 1.0f;

        int total = transform.childCount;
        for (int i = 0; i < total; ++i)
        {
            transform.GetChild(i).gameObject.GetComponent<Tile>().FlickTrueTile(duration);
        }
    }

    private void Awake()
    {
        _normalTileScript = _normalTilePrefab.GetComponent<Tile>();
        _tileMatrix = new TileType[_rows, _columns];
    }

    private void Start()
    {
        IsInitBoard = false;
        _hasDistributedTiles = false;
    }

    // Spawn normal tiles in a square format
    private void SpawnNormalTiles(int rows, int columns)
    {
        // Determine position of (top,left) point of the square
        float tileSideLength = _normalTileScript.GetSideLength();

        float colLength = rows * tileSideLength;
        float rowLength = columns * tileSideLength;

        Vector3 topLeftPosition = new Vector3(-rowLength / 2f, 0f, colLength / 2f);

        // Spawn normal tiles
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < columns; ++j)
            {
                Vector3 spawnPosition = topLeftPosition + new Vector3((j + 0.5f) * tileSideLength, 0, -(0.5f + i) * tileSideLength);
                Instantiate(_normalTilePrefab, spawnPosition, _normalTilePrefab.transform.rotation, transform);
            }
        }
    }

    // Spawn tiles based on the previously distributed board (must call method DistributeTiles() before)
    private void SpawnTiles()
    {
        int rows = _rows;
        int cols = _columns;

        // Determine position of (top,left) point of the square
        float tileSideLength = _normalTileScript.GetSideLength();

        float colLength = rows * tileSideLength;
        float rowLength = cols * tileSideLength;

        Vector3 topLeftPosition = new Vector3(-rowLength / 2f, 0f, colLength / 2f);

        // Spawning...
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                Vector3 spawnPosition = topLeftPosition + new Vector3((j + 0.5f) * tileSideLength, 0, -(0.5f + i) * tileSideLength);
                
                GameObject spawnPrefab = _normalTilePrefab;
                if (_tileMatrix[i, j] == TileType.Special)
                {
                    spawnPrefab = _specialTilePrefab;
                }

                Instantiate(spawnPrefab, spawnPosition, _normalTilePrefab.transform.rotation, transform);
            }
        }
    }
}

enum TileType
{
    Normal = 0,
    Special,
    Numbered,
}
