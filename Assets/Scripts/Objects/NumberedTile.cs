using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberedTile : SpecialTile
{
    [SerializeField] private Text _numberText;

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

    protected override void ShowTrueTile()
    {
        base.ShowTrueTile();

        _numberText.text = $"{OrderNumber}";
    }

    protected override void HideTrueTile()
    {
        base.HideTrueTile();

        _numberText.text = "";
    }
}
