using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _normalTilePrefab;

    [SerializeField] private GameObject _specialTilePrefab;

    [SerializeField] private GameObject _numberedTilePrefab;

    [SerializeField] private GameObject _playerPrefab;

    private int _maxOrderNumber;

    private Tile _tileScript;

    private List<CachedSpecialTile> _cachedSpecialTiles;

    private List<int> _orderNumberPool;

    public float GetTileSideLength() => _tileScript.GetSideLength();

    public GameObject SpawnTile(TileType type, Vector3 spawnPosition, Transform parent)
    {
        GameObject spawnPrefab = _normalTilePrefab;
        if (TileType.Special == type)
        {
            spawnPrefab = _specialTilePrefab;
        }
        else if (TileType.Numbered == type)
        {
            spawnPrefab = _numberedTilePrefab;
        }

        GameObject tileObject = Instantiate(spawnPrefab, spawnPosition, spawnPrefab.transform.rotation, parent);
        if (TileType.Numbered == type)
        {
            tileObject.GetComponent<NumberedTile>().OrderNumber = GetOrderNumberFromPool();
        }

        return tileObject;
    }

    // Either spawn (if normal tile) or cache (if special tile) for later spawning (to improve batching performance)
    public void SpawnOrCacheTile(TileType type, Vector3 spawnPosition, Transform parent)
    {
        if (TileType.Normal != type)
        {
            // Cache
            CachedSpecialTile cachedTile = new CachedSpecialTile();
            cachedTile._type = type;
            cachedTile._spawnPosition = spawnPosition;
            cachedTile._parentTransform = parent;

            _cachedSpecialTiles.Add(cachedTile);

            return;
        }

        // Spawn
        Instantiate(_normalTilePrefab, spawnPosition, _normalTilePrefab.transform.rotation, parent);
    }

    public void SpawnCachedSpecialTiles(params Tile.OnTriggerDelegate[] delegates)
    {
        foreach (var spawnTileInfo in _cachedSpecialTiles)
        {
            Vector3 spawnPosition = spawnTileInfo._spawnPosition;
            Transform parent = spawnTileInfo._parentTransform;
            
            GameObject spawnPrefab = _normalTilePrefab;
            switch (spawnTileInfo._type)
            {
                case TileType.Special:
                    spawnPrefab = _specialTilePrefab;
                    break;
                case TileType.Numbered:
                    spawnPrefab = _numberedTilePrefab;
                    break;
            }

            GameObject tileObject = Instantiate(spawnPrefab, spawnPosition, spawnPrefab.transform.rotation, parent);
            if (TileType.Numbered == spawnTileInfo._type)
            {
                tileObject.GetComponent<NumberedTile>().OrderNumber = GetOrderNumberFromPool();
            }

            Tile tileScript = tileObject.GetComponent<Tile>();
            foreach (var delegateFunc in delegates)
            {
                tileScript.TriggerDelegate += delegateFunc;
            }
        }

        // Clear after done processing cached tiles
        _cachedSpecialTiles.Clear();
    }

    public GameObject SpawnPlayer(Vector3 spawnPosition)
    {
        return Instantiate(_playerPrefab, spawnPosition, _playerPrefab.transform.rotation);
    }

    public void InitOrderNumberPool(int maxNumber)
    {
        if (maxNumber <= 0) return;

        _maxOrderNumber = maxNumber;
        _orderNumberPool.Clear();

        for (int i = 1; i <= _maxOrderNumber; ++i)
        {
            _orderNumberPool.Add(i);
        }
    }

    private void Awake()
    {
        _tileScript = _normalTilePrefab.GetComponent<Tile>();
        _cachedSpecialTiles = new List<CachedSpecialTile>();
        _orderNumberPool = new List<int>();
    }

    private int GetOrderNumberFromPool()
    {
        if (_orderNumberPool.Count == 0) return -1;

        int index = Random.Range(0, _orderNumberPool.Count);
        int value = _orderNumberPool[index];

        _orderNumberPool.RemoveAt(index);
        return value;
    }
}

struct CachedSpecialTile
{
    public TileType _type;
    public Transform _parentTransform;
    public Vector3 _spawnPosition;
}
