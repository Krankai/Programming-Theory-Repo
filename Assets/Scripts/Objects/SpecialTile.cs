using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTile : Tile
{
    private Color _baseColor;

    private Renderer _renderer;


    public override void ShowTrueTile()
    {
        _renderer.material.color = TileColor;
    }

    public override void HideTrueTile()
    {
        _renderer.material.color = _baseColor;
    }

    protected override void Awake()
    {
        base.Awake();

        _renderer = GetComponent<Renderer>();

        _baseColor = TileColor;
        TileColor = Color.red;
    }
}