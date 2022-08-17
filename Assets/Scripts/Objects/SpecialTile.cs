using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTile : Tile
{
    [SerializeField] protected Color _triggerColor = Color.red;

    //private Renderer _renderer;

    protected override void Awake()
    {
        base.Awake();
        
        //_renderer = GetComponent<Renderer>();
    }

    protected override void Start()
    {
        base.Start();

        //_baseColor = TileColor;
        //TileColor = Color.red;

        //_triggerEvent.AddListener(GameManager.Instance.UpdateRemainedTiles);
    }

    protected override void ShowTrueTile()
    {
        base.ShowTrueTile();

        //_renderer.material.color = TileColor;
        TileColor = _triggerColor;
        UpdateTileColor();
    }

    protected override void HideTrueTile()
    {
        base.HideTrueTile();

        //_renderer.material.color = _baseColor;
        TileColor = _baseColor;
        UpdateTileColor();
    }
}
