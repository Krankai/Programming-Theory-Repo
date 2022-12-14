using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    public delegate void OnTriggerDelegate(GameObject tile, bool isValid);

    public OnTriggerDelegate TriggerDelegate;

    //public Color TileColor { get; protected set; }

    //[SerializeField] protected Color _baseColor = Color.white;

    [SerializeField] private float _minDistance = 1.4f;

    [SerializeField] private float _flickerDelay = 0.1f;

    private bool _isTriggered;

    private bool _isFlickered;

    private bool _isAcceptTrigger;

    private BoxCollider _collider;

    //private Renderer _renderer;

    public bool IsFlickered => _isFlickered;

    public bool IsTriggered => _isTriggered;

    public bool IsNear(Vector3 comparePosition) => Vector3.Distance(comparePosition, transform.position) <= _minDistance;

    // ABSTRACTION
    public float GetSideLength()
    {
        if (_collider == null)
        {
            _collider = GetComponent<BoxCollider>();
        }

        return _collider.size.x * transform.localScale.x;
    }

    public void FlickerTile(float duration)
    {
        if (IsTriggered) return;

        _isFlickered = true;

        Invoke("ShowTrueTile", _flickerDelay);
        Invoke("HideTrueTile", _flickerDelay + duration);

        StartCoroutine(SetFlickerStateCoroutine(false, _flickerDelay + duration));
    }

    public virtual void TriggerTile()
    {
        if (IsFlickered || IsTriggered) return;

        _isAcceptTrigger = false;
        ShowTrueTile();
        TriggerDelegate?.Invoke(gameObject, IsValidTrigger());

    }

    public void UntriggerTile()
    {
        if (IsFlickered || !IsTriggered) return;
        HideTrueTile();
    }

    protected virtual void Awake()
    {
        if (_collider == null)
        {
            _collider = GetComponent<BoxCollider>();
        }

        //_renderer = GetComponent<Renderer>();
    }

    protected virtual void Start()
    {
        //TileColor = _baseColor;
        //UpdateTileColor();

        _isFlickered = false;
        _isTriggered = false;
        _isAcceptTrigger = true;
    }

    protected virtual void ShowTrueTile()
    {
        _isTriggered = true;
    }

    protected virtual void HideTrueTile()
    {
        _isTriggered = false;
    }

    protected virtual bool IsValidTrigger()
    {
        return true;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (_isAcceptTrigger && collision.gameObject.CompareTag("Player") && IsNear(collision.gameObject.transform.position))
        {
            TriggerTile();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isAcceptTrigger = true;
        }
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
