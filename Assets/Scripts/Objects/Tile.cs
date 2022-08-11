using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    public Color TileColor { get; protected set; }

    [SerializeField] private float minDistance = 1.2f;

    protected UnityEvent _triggerEvent;

    private bool _isTriggered;

    private bool _isFlickered;

    private BoxCollider _collider;

    public bool IsFlickered => _isFlickered;

    public bool IsTriggered => _isTriggered;

    public bool IsNear(Vector3 comparePosition) => Vector3.Distance(comparePosition, transform.position) <= minDistance;

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

        float delay = 0.1f;
        Invoke("ShowTrueTile", delay);
        Invoke("HideTrueTile", delay + duration);

        _isFlickered = true;
        StartCoroutine(SetFlickerStateCoroutine(false, delay + duration));
    }

    public void TriggerTile()
    {
        if (IsFlickered || IsTriggered) return;
        ShowTrueTile();

        _triggerEvent?.Invoke();
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

        _triggerEvent = new UnityEvent();
    }

    protected virtual void Start()
    {
        TileColor = Color.white;
        _isTriggered = false;
    }

    protected virtual void ShowTrueTile()
    {
        _isTriggered = true;
    }

    protected virtual void HideTrueTile()
    {
        _isTriggered = false;
    }

    private IEnumerator SetFlickerStateCoroutine(bool state, float delay)
    {
        yield return new WaitForSeconds(delay);
        _isFlickered = state;
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Player") && IsNear(collisionInfo.gameObject.transform.position))
        {
            Debug.Log("Distance: " + Vector3.Distance(collisionInfo.gameObject.transform.position, transform.position));
            TriggerTile();
        }
    }
}

public enum TileType
{
    Normal = 0,
    Special,
    Numbered,
}
