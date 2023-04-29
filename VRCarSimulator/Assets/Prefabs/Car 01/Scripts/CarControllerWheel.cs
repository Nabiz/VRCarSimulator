using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CarControllerWheel : MonoBehaviour
{
    private float horizontalInput, verticalInput, breakInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;

    private bool isStarted = false;
    private bool gearForrward = true;

    private Rigidbody rb;
    private float radius = 6f;
    private float downForce = 50f;

    // Settings
    [SerializeField] private float motorForce, breakForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    [SerializeField] private Transform steerWheel;
    double speed = 0f;
    public TextMeshProUGUI speedText;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            isStarted = true;
        }

        if (isStarted)
        {
            GetInput();
        }
    }
    private void FixedUpdate()
    {
        speed = 3.6 * rb.velocity.magnitude;
        speedText.text =  Math.Round(speed).ToString();
        Debug.Log(speed);
        if (isStarted)
        {
            //Debug.Log(speed);
            HandleMotor();
            HandleSteering();
            UpdateWheels();
        }
        steerWheel.localEulerAngles = new Vector3(0f, 0f, -horizontalInput * 90f);
        AddDownForce();
    }

    private void GetInput()
    {
        // Steering Input
        horizontalInput = Input.GetAxis("Horizontal");

        // Acceleration Input
        verticalInput = (Input.GetAxis("VerticalGas") + 1.0f)/2.0f;

        // Breaking Input
        breakInput = (Input.GetAxis("VerticalBreak") + 1.0f) / 2.0f;
        
        if (Input.GetButtonDown("LowerGear") && speed < 4f)
        {
            gearForrward = false;
        }
        if (Input.GetButtonDown("HigherGear") && speed < 4f)
        {
            gearForrward = true;
        }
    }
    private void AddDownForce()
    {
        rb.AddForce(-transform.up * downForce * rb.velocity.magnitude);
    }

    private void HandleMotor()
    {
        if (gearForrward)
        {
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        }
        else
        {
            frontLeftWheelCollider.motorTorque = -verticalInput * motorForce;
            frontRightWheelCollider.motorTorque = -verticalInput * motorForce;
        }

        //currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = breakInput * breakForce;
        frontLeftWheelCollider.brakeTorque = breakInput * breakForce;
        rearLeftWheelCollider.brakeTorque = breakInput * breakForce;
        rearRightWheelCollider.brakeTorque = breakInput * breakForce;
    }

    private void HandleSteering()
    {
        if (horizontalInput > 0)
        {
            frontLeftWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
            frontRightWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontalInput;
        }
        else if(horizontalInput < 0)
        {
            frontLeftWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontalInput;
            frontRightWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
        }
        else
        {
            frontLeftWheelCollider.steerAngle = 0f;
            frontRightWheelCollider.steerAngle = 0f;
        }
        //currentSteerAngle = maxSteerAngle * horizontalInput;
        //frontLeftWheelCollider.steerAngle = currentSteerAngle;
        //frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}