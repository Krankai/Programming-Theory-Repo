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

    private TileType[,] _tileMatrix;

    public void GenerateBoard(int numberedTileCount)
    {
        if (IsInitBoard)
        {
            ClearBoard();
        }

        DistributeTiles(numberedTileCount);
        SpawnTiles();

        IsInitBoard = true;
    }

    public void ClearBoard()
    {
        int total = transform.childCount;
        for (int i = 0; i < total; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        IsInitBoard = false;
    }

    // Flick true state of all tiles on the board on for the special duration
    public void FlickBoard(float flickDuration)
    {
        if (!IsInitBoard) return;

        int total = transform.childCount;
        for (int i = 0; i < total; ++i)
        {
            transform.GetChild(i).gameObject.GetComponent<Tile>().FlickTrueTile(flickDuration);
        }
    }

    private void Awake()
    {
        _normalTileScript = _normalTilePrefab.GetComponent<Tile>();
        _tileMatrix = new TileType[_rows, _columns];
    }

    private void OnValidate()
    {
        _tileMatrix = new TileType[_rows, _columns];
    }

    private void Start()
    {
        IsInitBoard = false;
    }

    private void DistributeTiles(int numberedTileCount)
    {
        int total = _rows * _columns;

        for (int i = 0; i < _rows; ++i)
        {
            bool isSetAbnormalTile = false;     // for each row

            for (int j = 0; j < _columns; ++j)
            {
                bool isLastTileOfRow = (j == _columns - 1);
                bool isLastTileOfBoard = isLastTileOfRow && (i == _rows - 1);

                int numRowsLeft = _rows - i;

                // Check for numbered tile
                if (!isSetAbnormalTile && numberedTileCount > 0)
                {
                    bool isNumberedTile = (isLastTileOfRow && numRowsLeft <= numberedTileCount) || Random.Range(0, _columns) == 0;

                    if (isNumberedTile)
                    {
                        isSetAbnormalTile = true;
                        --numberedTileCount;
                        _tileMatrix[i, j] = TileType.Numbered;

                        continue;
                    }
                }

                // Check for special tile
                if (!isSetAbnormalTile && numRowsLeft > numberedTileCount)
                {
                    bool isSpecialTile = isLastTileOfRow || (Random.Range(0, _columns) == 0);

                    if (isSpecialTile)
                    {
                        isSetAbnormalTile = true;
                        _tileMatrix[i, j] = TileType.Special;

                        continue;
                    }
                }

                _tileMatrix[i, j] = TileType.Normal;
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
                else if (_tileMatrix[i, j] == TileType.Numbered)
                {
                    spawnPrefab = _numberedTilePrefab;
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
