using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Color TileColor { get; protected set; }

    public void FlickTrueTile(float duration)
    {
        float delay = 0.1f;
        Invoke("ShowTrueTile", delay);
        Invoke("HideTrueTile", delay + duration);
    }

    public void OnTriggered()
    {
        ShowTrueTile();
    }

    protected virtual void Start()
    {
        TileColor = Color.white;
    }

    protected virtual void ShowTrueTile()
    {
        // Do nothing for base tile
    }

    protected virtual void HideTrueTile()
    {
        // Do nothing for base tile
    }
}
