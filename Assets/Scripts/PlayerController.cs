using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float startingSpeed = 5.0f;

    [SerializeField] private float maxSpeed = 10.0f;

    [SerializeField] private float accelerationRate = 1.0f;     // speed per second

    [SerializeField] private float decelerateWaitTime = 0.5f;   // waiting time for the current speed to be reset (after no input from users)

    private float currentSpeed;

    private bool decelerateFlag;

    private float decelerateTiming;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currentSpeed = startingSpeed;
        decelerateFlag = false;
        decelerateTiming = decelerateWaitTime;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

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
                currentSpeed = startingSpeed;
                decelerateTiming = decelerateWaitTime;
            }
        }

        if (receivedInput)
        {
            currentSpeed = Mathf.Clamp(currentSpeed + Time.deltaTime * accelerationRate, startingSpeed, maxSpeed);
        }

        transform.Translate(horizontalInput * Time.deltaTime * currentSpeed, 0, verticalInput * Time.deltaTime * currentSpeed);
    }
}
