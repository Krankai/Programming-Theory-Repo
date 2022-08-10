using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Color TileColor { get; protected set; }

    private bool _isTriggered;

    private bool _isFlickered;

    private BoxCollider _collider;

    public bool IsFlickered() => _isFlickered;

    public bool IsTriggered() => _isTriggered;

    public float GetSideLength()
    {
        if (_collider == null)
        {
            _collider = GetComponent<BoxCollider>();
        }

        return _collider.size.x * transform.localScale.x;
    }

    public void FlickTrueTile(float duration)
    {
        if (IsTriggered()) return;

        float delay = 0.1f;
        Invoke("ShowTrueTile", delay);
        Invoke("HideTrueTile", delay + duration);

        _isFlickered = true;
        StartCoroutine(SetFlickerStateCoroutine(false, delay + duration));
    }

    public void OnTriggered()
    {
        if (IsFlickered()) return;

        if (IsTriggered())
            HideTrueTile();
        else
            ShowTrueTile();
        
        _isTriggered = !_isTriggered;
    }

    protected virtual void Awake()
    {
        if (_collider == null)
        {
            _collider = GetComponent<BoxCollider>();
        }
    }

    protected virtual void Start()
    {
        TileColor = Color.white;
        _isTriggered = false;
    }

    protected virtual void ShowTrueTile()
    {
        // Do nothing for base tile
    }

    protected virtual void HideTrueTile()
    {
        // Do nothing for base tile
    }

    private IEnumerator SetFlickerStateCoroutine(bool state, float delay)
    {
        yield return new WaitForSeconds(delay);
        _isFlickered = state;
    }
}

public enum TileType
{
    Normal = 0,
    Special,
    Numbered,
}
