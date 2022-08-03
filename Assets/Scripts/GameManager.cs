using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool IsInitBoard { get; private set; }

    [SerializeField] private GameObject _normalTilePrefab;

    [SerializeField] private GameObject _specialTilePrefab;

    [SerializeField] private GameObject _numberedTilePrefab;

    [SerializeField] private int _rows;

    [SerializeField] private int _columns;

    private Tile _normalTileScript;

    private SpecialTile _specialTileScript;

    private Camera _camera;

    // todo: 2-dimensional array to hold tile's type for each position...

    [ContextMenu("Generate Board")]
    public void GenerateBoard()
    {
        //int specialTilesCount = size;       // each row must have at least 1

        // Generate normal tile
        SpawnNormalTiles(_rows, _columns);

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

    private void Awake()
    {
        _camera = Camera.main;

        _normalTileScript = _normalTilePrefab.GetComponent<Tile>();
    }

    private void Start()
    {
        //Debug.Log(_normalTileScript.GetSize());
        IsInitBoard = false;
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
}
