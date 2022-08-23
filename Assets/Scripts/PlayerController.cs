using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsPlayable { get; set; }

    [SerializeField] private float _startingSpeed = 5.0f;

    [SerializeField] private float _maxSpeed = 10.0f;

    [SerializeField] private float _speedModifier = 1.5f;

    [SerializeField] private float _accelerationRate = 1.0f;    // speed per second

    [SerializeField] private float _decelerationRate = 2.0f;    // speed per second

    [SerializeField] private float _decelerateWaitTime = 0.5f;  // waiting time for the current speed to be reset (after no input from users)

    [SerializeField] private float _mouseSensitivity = 0.008f;

    [SerializeField] private float _updateAnchorMouseClickWaitTime = 1f;      // time required before new 'anchor' mouse click is updated

    //private bool isTouchDevice;

    private float _currentSpeed;

    private bool _decelerateFlag;

    private float _decelerateTiming;

    private bool _isMouseDragged;

    private bool _isMouseIdle;

    private bool _isMouseFinishCountdown;       // finish countdown to update new anchor (?)

    private bool _isGrounded;

    private float _mouseTimer;

    private Vector3 _anchorMousePosition;

    private Vector3 _lastMousePosition;

    private Vector2 _cacheMouseInput;      // x: horizontal, y: vertical

    private Rigidbody _rigidBody;

    private MeshRenderer _meshRenderer;

    private Vector3 _vectorZero;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;

        _rigidBody = GetComponent<Rigidbody>();

        _cacheMouseInput = new Vector2();
    }

    private void Start()
    {
        _vectorZero = Vector3.zero;

        _currentSpeed = _startingSpeed;
        _decelerateFlag = false;
        _decelerateTiming = _decelerateWaitTime;

        _isMouseDragged = false;
        _isMouseIdle = true;
        _isMouseFinishCountdown = false;
        _isGrounded = false;

        _anchorMousePosition = _vectorZero;

        _meshRenderer.enabled = true;

        IsPlayable = false;
    }

    private void FixedUpdate()
    {
        if (_isGrounded && _rigidBody.velocity.y != 0)
        {
            _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, 0, _rigidBody.velocity.z);
        }

        if (_decelerateFlag)
        {
            _rigidBody.velocity *= Mathf.Clamp(1 - Time.fixedDeltaTime * _decelerationRate, 0, 1);
        }
    }

    private void Update()
    {
        if (!IsPlayable) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput == 0 && verticalInput == 0)
        {
            ProcessMouseInput(ref horizontalInput, ref verticalInput);
        }

        ProcessMovement(horizontalInput, verticalInput);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Tile") || collision.gameObject.CompareTag("Boundary"))
        {
            _isGrounded = true;
        }
    }

    private void ProcessMouseInput(ref float horizontalInput, ref float verticalInput)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!_isMouseDragged)
            {
                _anchorMousePosition = _lastMousePosition = Input.mousePosition;
            }
            _isMouseDragged = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isMouseDragged = false;
        }

        if (_isMouseDragged)
        {
            Vector3 currentMousePosition = Input.mousePosition;

            UpdateAnchorMouseClick(currentMousePosition);
            RestoreMouseInputs(ref horizontalInput, ref verticalInput, currentMousePosition);
        }
    }

    private float GetAxisMouseInput(string axis, Vector3 currentMousePosition)
    {
        float inputValueRange = 1f;
        float axisDelta = 0f;

        if (axis.Equals("horizontal", System.StringComparison.OrdinalIgnoreCase))
        {
            axisDelta = currentMousePosition.x - _anchorMousePosition.x;
        }
        else if (axis.Equals("vertical", System.StringComparison.OrdinalIgnoreCase))
        {
            axisDelta = currentMousePosition.y - _anchorMousePosition.y;
        }
        
        return Mathf.Clamp(axisDelta * _mouseSensitivity, -inputValueRange, inputValueRange);
    }

    // Restore with cached values, or compute new
    private void RestoreMouseInputs(ref float horizontalInput, ref float verticalInput, Vector3 currentMousePosition)
    {
        horizontalInput = (_cacheMouseInput.x != 0) ? _cacheMouseInput.x : GetAxisMouseInput("horizontal", currentMousePosition);
        verticalInput = (_cacheMouseInput.y != 0) ? _cacheMouseInput.y : GetAxisMouseInput("vertical", currentMousePosition);
    }

    private void ProcessMovement(float horizontalInput, float verticalInput)
    {
        bool receivedInput = (horizontalInput != 0 || verticalInput != 0);

        // Set flag to decelerate if no inputs are received
        if (_currentSpeed > _startingSpeed)
        {
            if (!_decelerateFlag && !receivedInput)
            {
                _decelerateFlag = true;
            }
            else if (receivedInput)
            {
                _decelerateFlag = false;
                _decelerateTiming = _decelerateWaitTime;
            }
        }

        // Decelerate speed back to starting point gradually
        if (_decelerateFlag)
        {
            _decelerateTiming -= Time.deltaTime;
            if (_decelerateTiming <= 0)
            {
                _decelerateFlag = false;
                _decelerateTiming = _decelerateWaitTime;

                _currentSpeed = _startingSpeed;
            }
        }

        // Apply force for movement only if there are inputs from user
        if (receivedInput)
        {
            _currentSpeed = Mathf.Clamp(_currentSpeed + Time.deltaTime * _accelerationRate, _startingSpeed, _maxSpeed);

            Vector3 appliedForce = new Vector3(horizontalInput * Time.deltaTime * _currentSpeed, 0, verticalInput * Time.deltaTime * _currentSpeed);
            _rigidBody.AddForce(appliedForce * _speedModifier, ForceMode.VelocityChange);
        }
    }

    private void UpdateAnchorMouseClick(Vector3 currentMousePosition)
    {
        // Detect mouse idling state
        float deltaMouse = Vector3.Distance(currentMousePosition, _lastMousePosition);
        if (deltaMouse <= 1)
        {
            if (!_isMouseFinishCountdown)
            {
                _mouseTimer = (_mouseTimer > 0) ? _mouseTimer : _updateAnchorMouseClickWaitTime;
            }
            _isMouseIdle = true;
        }
        else
        {
            _mouseTimer = 0;

            _isMouseIdle = false;
            _isMouseFinishCountdown = false;

            // Reset cached mouse input
            _cacheMouseInput.x = _cacheMouseInput.y = 0;
        }

        // Countdown to update new anchor (when idle)
        if (_isMouseIdle && !_isMouseFinishCountdown && _mouseTimer > 0)
        {
            _mouseTimer -= Time.deltaTime;
            if (_mouseTimer <= 0)
            {
                // Cache mouse input so that "player" won't slow down after updating new anchor
                _cacheMouseInput.x = GetAxisMouseInput("horizontal", currentMousePosition);
                _cacheMouseInput.y = GetAxisMouseInput("vertical", currentMousePosition);

                _anchorMousePosition = currentMousePosition;
                _isMouseFinishCountdown = true;
            }
        }

        _lastMousePosition = currentMousePosition;
    }
}
