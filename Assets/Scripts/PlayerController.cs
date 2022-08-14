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

    //private bool isTouchDevice;

    private float _currentSpeed;

    private bool _decelerateFlag;

    private float _decelerateTiming;

    private bool _isMouseDragged;

    private bool _isGrounded;

    private Vector3 _mouseStartPosition;

    private Rigidbody _rigidBody;

    private MeshRenderer _meshRenderer;

    private Vector3 _vectorZero;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;

        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _vectorZero = Vector3.zero;

        _currentSpeed = _startingSpeed;
        _decelerateFlag = false;
        _decelerateTiming = _decelerateWaitTime;
        _isMouseDragged = false;
        _isGrounded = false;

        _mouseStartPosition = _vectorZero;

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
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
        else if (/*collision.gameObject.GetComponent<Tile>()*/collision.gameObject.CompareTag("Tile"))
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
                _mouseStartPosition = Input.mousePosition;
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

            horizontalInput = GetMouseHorizontalInput(currentMousePosition);
            verticalInput = GetMouseVerticalInput(currentMousePosition);
        }
    }

    private float GetMouseHorizontalInput(Vector3 currentMousePosition)
    {
        float minInputValue = -1f;
        float maxInputValue = 1f;

        float horizontalDelta = currentMousePosition.x - _mouseStartPosition.x;
        return Mathf.Clamp(horizontalDelta, minInputValue, maxInputValue);
    }

    private float GetMouseVerticalInput(Vector3 currentMousePosition)
    {
        float minInputValue = -1f;
        float maxInputValue = 1f;

        float verticalDelta = currentMousePosition.y - _mouseStartPosition.y;
        return Mathf.Clamp(verticalDelta, minInputValue, maxInputValue);
    }

    private void ProcessMovement(float horizontalInput, float verticalInput)
    {
        bool receivedInput = (horizontalInput != 0 || verticalInput != 0);

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

        if (receivedInput)
        {
            _currentSpeed = Mathf.Clamp(_currentSpeed + Time.deltaTime * _accelerationRate, _startingSpeed, _maxSpeed);

            Vector3 appliedForce = new Vector3(horizontalInput * Time.deltaTime * _currentSpeed, 0, verticalInput * Time.deltaTime * _currentSpeed);
            _rigidBody.AddForce(appliedForce * _speedModifier, ForceMode.VelocityChange);
        }
    }
}
