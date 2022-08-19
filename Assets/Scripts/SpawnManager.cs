using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public float GetTileSideLength() => _tileScript.GetSideLength();

    [SerializeField] private GameObject _normalTilePrefab;

    [SerializeField] private GameObject _specialTilePrefab;

    [SerializeField] private GameObject _numberedTilePrefab;

    [SerializeField] private GameObject _playerPrefab;

    private Tile _tileScript;

    private List<int> _orderNumberPool;

    private int _maxOrderNumber;

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
            //int orderNumber = Random.Range(1, maxPossibleOrderNumber + 1);
            //tileObject.GetComponent<NumberedTile>().OrderNumber = Random.Range(1, orderNumber);
            tileObject.GetComponent<NumberedTile>().OrderNumber = GetOrderNumberFromPool();
        }

        return tileObject;
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
    }

    private void Start()
    {
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
