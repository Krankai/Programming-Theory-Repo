using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public bool IsInitBoard { get; private set; }

    // public Vector3 MidPoint { get; private set; }

    // public float GetRowLength() => _rowLength;

    // public float GetColLength() => _colLength;

    public float RowLength => _spawnManager.GetTileSideLength() * _rows;

    public float ColLength => _spawnManager.GetTileSideLength() * _columns;

    public Vector3 MidPoint => new Vector3(_midbotBoardTransform.position.x, 0, _midbotBoardTransform.position.z + ColLength / 2);

    [SerializeField] private int _rows;

    [SerializeField] private int _columns;

    [SerializeField] private Transform _midbotBoardTransform;

    [SerializeField] private GameObject _tilesGroupObject;

    [SerializeField] private GameObject _boundariesGroupObject;

    private Transform TileGroupTransform => _tilesGroupObject ? _tilesGroupObject.transform : transform;

    private Transform BoundaryGroupTransform => _boundariesGroupObject ? _boundariesGroupObject.transform : transform;

    private TileType[,] _tileMatrix;

    // private float _rowLength;

    // private float _colLength;

    // private float _midpoint;

    private SpawnManager _spawnManager;

    public void GenerateBoard(int numberedTileCount)
    {
        if (IsInitBoard)
        {
            ClearBoard();
        }

        DistributeTiles(numberedTileCount);
        SpawnTiles();

        SetupBoundaries();

        IsInitBoard = true;
    }

    public void GenerateBoard(int numberedTileCount, int rows, int columns)
    {
        _rows = rows;
        _columns = columns;

        GenerateBoard(numberedTileCount);
    }

    public void GenerateBoard(int numberedTileCount, Vector2 size)
    {
        _rows = (int)size.x;
        _columns = (int)size.y;

        GenerateBoard(numberedTileCount);
    }

    public void ClearBoard()
    {
        Transform parentTransform = TileGroupTransform;

        int total = parentTransform.childCount;
        for (int i = 0; i < total; ++i)
        {
            Destroy(parentTransform.GetChild(i).gameObject);
        }

        IsInitBoard = false;
    }

    // Flick true state of all tiles on the board on for the special duration
    public void FlickerBoard(float flickDuration)
    {
        if (!IsInitBoard) return;

        Transform parentTransform = TileGroupTransform;

        int total = parentTransform.childCount;
        for (int i = 0; i < total; ++i)
        {
            parentTransform.GetChild(i).gameObject.GetComponent<Tile>()?.FlickerTile(flickDuration);
        }
    }

    // Reset trigger state of all tiles on the board
    public void ResetBoard()
    {
        // todo: improve by create a list holding triggered tiles only!!!

        if (!IsInitBoard) return;

        Transform parentTransform = TileGroupTransform;

        int total = parentTransform.childCount;
        for (int i = 0; i < total; ++i)
        {
            parentTransform.GetChild(i).gameObject.GetComponent<Tile>()?.UntriggerTile();
        }
    }

    private void Awake()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    private void OnValidate()
    {
        _tileMatrix = new TileType[_rows, _columns];
    }

    private void Start()
    {
        IsInitBoard = false;

        // _rowLength = _spawnManager.GetTileSideLength() * _rows;
        // _colLength = _spawnManager.GetTileSideLength() * _columns;

        // MidPoint = new Vector3(_midbotBoardTransform.position.x, 0, _midbotBoardTransform.position.z + _colLength / 2);
    }

    // Return number of special + numbered tiles
    private int DistributeTiles(int numberedTileCount)
    {
        _tileMatrix = new TileType[_rows, _columns];
        
        // include both special and numbered tiles
        int abnormalTileCount = numberedTileCount;

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

                        ++abnormalTileCount;

                        continue;
                    }
                }

                _tileMatrix[i, j] = TileType.Normal;
            }
        }

        return abnormalTileCount;
    }

    // Spawn tiles based on the previously distributed board (must call method DistributeTiles() before)
    private void SpawnTiles()
    {
        float tileSideLength = _spawnManager.GetTileSideLength();
        Vector3 topLeftPosition = new Vector3(_midbotBoardTransform.position.x - RowLength / 2f, 0f, _midbotBoardTransform.position.z + ColLength);

        // Spawning...
        for (int i = 0; i < _rows; ++i)
        {
            for (int j = 0; j < _columns; ++j)
            {
                Vector3 spawnPosition = topLeftPosition + new Vector3((j + 0.5f) * tileSideLength, 0, -(0.5f + i) * tileSideLength);
                _spawnManager.SpawnTile(_tileMatrix[i,j], spawnPosition, _tilesGroupObject ? _tilesGroupObject.transform : transform);
            }
        }
    }

    private void SetupBoundaries()
    {
        Transform parent = _boundariesGroupObject ? _boundariesGroupObject.transform : transform;

        float rowLength = RowLength;
        float colLength = ColLength;

        CreateBoundary("TopBoundary", Boundary.Top, rowLength, colLength, parent);
        CreateBoundary("BottomBoundary", Boundary.Bottom, rowLength, colLength, parent);
        CreateBoundary("LeftBoundary", Boundary.Left, rowLength, colLength, parent);
        CreateBoundary("RightBoundary", Boundary.Right, rowLength, colLength, parent);
    }

    private void CreateBoundary(string name, Boundary type, float rowLength, float colLength, Transform parent)
    {
        const float DepthScale = 5.0f;

        bool isVerticalDirection = (type == Boundary.Top || type == Boundary.Bottom);

        GameObject boundaryObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boundaryObject.name = name;
        boundaryObject.transform.parent = parent;

        // scale
        BoxCollider collider = boundaryObject.GetComponent<BoxCollider>();
        boundaryObject.transform.localScale = isVerticalDirection
            ? new Vector3(rowLength / collider.size.x, 1, DepthScale)
            : new Vector3(DepthScale, 1, colLength / collider.size.z);

        // position
        float xPosition = _midbotBoardTransform.position.x;
        if (type == Boundary.Left)
        {
            xPosition -= rowLength / 2 + boundaryObject.transform.localScale.x * collider.size.x / 2;
        }
        else if (type == Boundary.Right)
        {
            xPosition += rowLength / 2 + boundaryObject.transform.localScale.x * collider.size.x / 2;
        }

        float zPosition = _midbotBoardTransform.position.z + colLength / 2;
        if (type == Boundary.Top)
        {
            zPosition += colLength / 2 + boundaryObject.transform.localScale.z * collider.size.z / 2;
        }
        else if (type == Boundary.Bottom)
        {
            zPosition -= colLength / 2 + boundaryObject.transform.localScale.z * collider.size.z / 2;
        }

        boundaryObject.transform.position = new Vector3(xPosition, 0, zPosition);

        // disable MeshRender (to not appear visually in game scene)
        boundaryObject.GetComponent<MeshRenderer>().enabled = false;
    }
}

enum Boundary
{
    Top = 0,
    Bottom,
    Left,
    Right,
}
