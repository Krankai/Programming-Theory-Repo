using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// INHERITANCE
public class NumberedTile : SpecialTile
{
    [SerializeField] private Color _invalidColor = Color.cyan;

    [SerializeField] private Text _numberText;

    // ENCAPSULATION
    [SerializeField] private int _orderNumber;

    public int OrderNumber
    { 
        get { return _orderNumber; }
        set { _orderNumber = value > 0 ? value : 1; }
    }

    protected override void Start()
    {
        base.Start();

        _numberText.text = "";
    }

    // POLYMORPHISM
    protected override void ShowTrueTile()
    {
        base.ShowTrueTile();

        _numberText.text = $"{OrderNumber}";
        
        if (!IsFlickered && !IsValidTrigger())
        {
            TileColor = _invalidColor;
            UpdateTileColor();
        }
    }

    // POLYMORPHISM
    protected override void HideTrueTile()
    {
        base.HideTrueTile();

        _numberText.text = "";
    }

    protected override bool IsValidTrigger()
    {
        return (_orderNumber == GameManager.Instance.GetNumberTriggeredTiles() + 1);
    }
}
