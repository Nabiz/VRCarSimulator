using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PupilLabs;

public class CarControllerWheel : MonoBehaviour
{
    private float horizontalInput, verticalInput, breakInput;

    private bool isStarted = false;
    private bool gearForrward = true;

    private Rigidbody rb;
    private float radius = 2f, wheelbase = 2.15f, trackWidth = 1f;
    private float downForce = 500f;

    public GameObject xrrig;

    // Settings
    [SerializeField] private float motorTorque, breakForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    [SerializeField] private AnimationCurve engineTorque;

    [SerializeField] private float[] gears = new float[6];
    private int currentGear = 0;


    [SerializeField] private Transform steerWheel;
    float speed = 0f;

    [SerializeField] private Image arrowGPS;

    [Header("Recording")]
    public AnnotationPublisher annotationPublisher;
    public RecordingController recordingController;

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

        if (Input.GetKey(KeyCode.H))
        {
            xrrig.transform.localPosition = new Vector3(-0.30f, -0.52f, 0.0f);
        }

        if (isStarted)
        {
            GetInput();
        }
    }
    private void FixedUpdate()
    {
        speed = 3.6f * rb.velocity.magnitude;
        if (isStarted)
        {
            if (gearForrward)
            {
                ChangeGear();
            }  
            CalculateMotorTorque();
            Motor();
            Steering();
            UpdateWheels();
        }
        steerWheel.localEulerAngles = new Vector3(0f, 0f, -horizontalInput * 450f);
        AddDownForce();
        AddWindForce();
        if (recordingController.IsRecording)
        {
            LogCarStatusAnnotation();
        }
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

    private void AddWindForce()
    {
            rb.AddForce(0.4f * 0.5f * 1.3f * 4.0f * rb.velocity.sqrMagnitude * -rb.velocity.normalized);
    }

    private void Motor()
    {
        if (gearForrward)
        {
            frontLeftWheelCollider.motorTorque = motorTorque;
            frontRightWheelCollider.motorTorque = motorTorque;
            rearLeftWheelCollider.motorTorque = motorTorque;
            rearRightWheelCollider.motorTorque = motorTorque;
        }
        else
        {
            frontLeftWheelCollider.motorTorque = -motorTorque;
            frontRightWheelCollider.motorTorque = -motorTorque;
            rearLeftWheelCollider.motorTorque = -motorTorque;
            rearRightWheelCollider.motorTorque = -motorTorque;
        }

        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = breakInput * breakForce;
        frontLeftWheelCollider.brakeTorque = breakInput * breakForce;
        rearLeftWheelCollider.brakeTorque = breakInput * breakForce;
        rearRightWheelCollider.brakeTorque = breakInput * breakForce;
    }

    private void Steering()
    {
        if (horizontalInput > 0)
        {
            frontLeftWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (radius + (trackWidth / 2.0f))) * horizontalInput;
            frontRightWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (radius - (trackWidth / 2.0f))) * horizontalInput;
        }
        else if(horizontalInput < 0)
        {
            frontLeftWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (radius - (trackWidth / 2.0f))) * horizontalInput;
            frontRightWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (radius + (trackWidth / 2.0f))) * horizontalInput;
        }
        else
        {
            frontLeftWheelCollider.steerAngle = 0f;
            frontRightWheelCollider.steerAngle = 0f;
        }
    }

    float engineRpm = 0;
    private void CalculateMotorTorque()
    {
        float wheelRpm = CalculateWheelRpm();
        motorTorque = engineTorque.Evaluate(engineRpm) * gears[currentGear] * verticalInput;
        float velocity = 0.0f;
        engineRpm = Mathf.SmoothDamp(engineRpm, 800 + (Mathf.Abs(wheelRpm) * 3.2f * gears[currentGear]), ref velocity, 0.02f);
    }

    private void ChangeGear()
    {
        if (engineRpm > 4500)
        {
            currentGear = Math.Clamp(currentGear + 1, 0, gears.Length - 1);
        }
        else if (engineRpm < 2000)
        {
            currentGear = Math.Clamp(currentGear - 1, speed < 4.0f ? 0 : 1, gears.Length - 1);
        }
    }

    private float CalculateWheelRpm()
    {
        float wheelRpm = 0.0f;
        wheelRpm = frontLeftWheelCollider.rpm + frontRightWheelCollider.rpm + rearLeftWheelCollider.rpm + rearRightWheelCollider.rpm;
        return wheelRpm/4f;
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

    public float getSpeed()
    {
        return speed;
    }

    public float getEngineRpm()
    {
        return engineRpm;
    }

    public int getCurrentGear()
    {
        return gearForrward ? currentGear+1 : -1;
    }

    private void LogCarStatusAnnotation()
    {
        //if (Time.frameCount % 2 == 0)
        {
           Dictionary<string, object> carStaus = new Dictionary<string, object>();
            carStaus.Add("speed", speed);
            carStaus.Add("pos_x", transform.position.x);
            carStaus.Add("pos_y", transform.position.y);
            carStaus.Add("gas_input", verticalInput);
            carStaus.Add("break_input", breakInput);
            carStaus.Add("wheel_input", horizontalInput);
            annotationPublisher.SendAnnotation("car_status", 0f, carStaus);
        }
    }

    public void setDiretctionGPS(int direction = 0)
    {
        switch(direction)
        {
            case -1:
                arrowGPS.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90f);
                break;
            case 0:
                arrowGPS.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0f);
                break;
            case 1:
                arrowGPS.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -90f);
                break;
            default:
                arrowGPS.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 180f);
                break;
        }
    }
}