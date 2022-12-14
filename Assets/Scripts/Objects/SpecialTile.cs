using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class SpecialTile : Tile
{
    public Color TileColor { get; protected set; }
    
    [SerializeField] private Color _baseColor = Color.white;

    [SerializeField] private Color _triggerColor = Color.red;

    private Material _material;

    protected override void Awake()
    {
        base.Awake();

        _material = GetComponent<Renderer>().material;
    }

    // POLYMORPHISM
    protected override void ShowTrueTile()
    {
        base.ShowTrueTile();

        TileColor = _triggerColor;
        UpdateTileColor();
    }

    // POLYMORPHISM
    protected override void HideTrueTile()
    {
        base.HideTrueTile();

        TileColor = _baseColor;
        UpdateTileColor();
    }

    protected void UpdateTileColor()
    {
        _material.color = TileColor;
    }
}
