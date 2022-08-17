using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberedTile : SpecialTile
{
    [SerializeField] private Color _invalidColor = Color.cyan;

    [SerializeField] private Text _numberText;

    [SerializeField] private int _orderNumber;

    public int OrderNumber
    { 
        get { return _orderNumber; }
        set { _orderNumber = value > 0 ? value : 1; }
    }

    public override void TriggerTile()
    {
        if (IsFlickered || IsTriggered) return;
        ShowTrueTile();

        TriggerDelegate?.Invoke(gameObject, IsValidTrigger());
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
        
        if (!IsFlickered && !IsValidTrigger())
        {
            TileColor = _invalidColor;
            UpdateTileColor();
        }
    }

    protected override void HideTrueTile()
    {
        base.HideTrueTile();

        _numberText.text = "";
    }

    private bool IsValidTrigger()
    {
        return (_orderNumber == GameManager.Instance.GetNumberTriggeredTiles() + 1);
    }
}
