using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float startingSpeed = 5.0f;

    [SerializeField] private float maxSpeed = 10.0f;

    [SerializeField] private float accelerationRate = 1.0f;     // speed per second

    [SerializeField] private float decelerationRate = 2.0f;    // speed per second

    [SerializeField] private float decelerateWaitTime = 0.5f;   // waiting time for the current speed to be reset (after no input from users)

    //private bool isTouchDevice;

    private float currentSpeed;

    private bool decelerateFlag;

    private float decelerateTiming;

    private bool isMouseDragged;

    private bool isGrounded;

    private Vector3 mouseStartPosition;

    private Rigidbody rb;

    private Vector3 vectorZero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        vectorZero = Vector3.zero;

        currentSpeed = startingSpeed;
        decelerateFlag = false;
        decelerateTiming = decelerateWaitTime;
        isMouseDragged = false;
        isGrounded = false;

        mouseStartPosition = vectorZero;
    }

    private void FixedUpdate()
    {
        if (isGrounded && rb.velocity.y != 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }

        if (decelerateFlag)
        {
            rb.velocity *= Mathf.Clamp(1 - Time.fixedDeltaTime * decelerationRate, 0, 1);
        }
    }

    private void Update()
    {
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
            isGrounded = true;
        }
    }

    private void ProcessMouseInput(ref float horizontalInput, ref float verticalInput)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isMouseDragged)
            {
                mouseStartPosition = Input.mousePosition;
            }
            isMouseDragged = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMouseDragged = false;
        }

        if (isMouseDragged)
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

        float horizontalDelta = currentMousePosition.x - mouseStartPosition.x;
        return Mathf.Clamp(horizontalDelta, minInputValue, maxInputValue);
    }

    private float GetMouseVerticalInput(Vector3 currentMousePosition)
    {
        float minInputValue = -1f;
        float maxInputValue = 1f;

        float verticalDelta = currentMousePosition.y - mouseStartPosition.y;
        return Mathf.Clamp(verticalDelta, minInputValue, maxInputValue);
    }

    private void ProcessMovement(float horizontalInput, float verticalInput)
    {
        bool receivedInput = (horizontalInput != 0 || verticalInput != 0);

        if (currentSpeed > startingSpeed)
        {
            if (!decelerateFlag && !receivedInput)
            {
                decelerateFlag = true;
            }
            else if (receivedInput)
            {
                decelerateFlag = false;
                decelerateTiming = decelerateWaitTime;
            }
        }

        if (decelerateFlag)
        {
            decelerateTiming -= Time.deltaTime;
            if (decelerateTiming <= 0)
            {
                decelerateFlag = false;
                decelerateTiming = decelerateWaitTime;

                currentSpeed = startingSpeed;
            }
        }

        if (receivedInput)
        {
            currentSpeed = Mathf.Clamp(currentSpeed + Time.deltaTime * accelerationRate, startingSpeed, maxSpeed);

            Vector3 appliedForce = new Vector3(horizontalInput * Time.deltaTime * currentSpeed, 0, verticalInput * Time.deltaTime * currentSpeed);
            rb.AddForce(appliedForce, ForceMode.VelocityChange);
        }
    }
}
