using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Color TileColor { get; protected set; }

    //private Renderer _renderer;


    public void FlickTrueTile(float duration)
    {
        float delay = 0.1f;
        Invoke("ShowTrueTile", delay);
        Invoke("HideTrueTile", delay + duration);
    }

    protected virtual void Awake()
    {
        //_renderer = GetComponent<Renderer>();

        TileColor = Color.white;
    }

    private void Start()
    {
        //_renderer.material.color = TileColor;
    }

    public virtual void ShowTrueTile()
    {
        // Do nothing for base tile
    }

    public virtual void HideTrueTile()
    {
        // Do nothing for base tile
    }
}
