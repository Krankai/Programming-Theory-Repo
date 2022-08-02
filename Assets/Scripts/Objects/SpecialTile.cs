using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTile : Tile
{
    private Color _baseColor;

    private Renderer _renderer;

    protected override void Start()
    {
        base.Start();

        _baseColor = TileColor;
        TileColor = Color.red;
    }

    protected override void ShowTrueTile()
    {
        base.ShowTrueTile();

        _renderer.material.color = TileColor;
    }

    protected override void HideTrueTile()
    {
        base.HideTrueTile();

        _renderer.material.color = _baseColor;
    }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }
}
