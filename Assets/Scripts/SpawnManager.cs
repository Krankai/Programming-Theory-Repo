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

        return Instantiate(spawnPrefab, spawnPosition, spawnPrefab.transform.rotation, parent);
    }

    public GameObject SpawnPlayer(Vector3 spawnPosition)
    {
        return Instantiate(_playerPrefab, spawnPosition, _playerPrefab.transform.rotation);
    }

    private void Awake()
    {
        _tileScript = _normalTilePrefab.GetComponent<Tile>();
    }
}
