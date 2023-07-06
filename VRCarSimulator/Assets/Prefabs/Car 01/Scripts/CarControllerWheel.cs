using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PupilLabs;

public class CarControllerWheel : MonoBehaviour
{
    private float horizontalInput, verticalInput, breakInput;

    private bool isStarted = false;
    private bool gearForrward = true;

    private Rigidbody rb;
    private float radius = 3.5f;
    private float downForce = 100f;

    // Settings
    [SerializeField] private float motorTorque, breakForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    [SerializeField] private AnimationCurve enginePower;

    [SerializeField] private float[] gears = new float[6];
    private int currentGear = 0;


    [SerializeField] private Transform steerWheel;
    float speed = 0f;
    public TextMeshProUGUI speedText;

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

        if (isStarted)
        {
            GetInput();
        }
    }
    private void FixedUpdate()
    {
        speed = 3.6f * rb.velocity.magnitude;
        speedText.text =  Math.Round(speed).ToString();
        if (isStarted)
        {
            ChangeGear();
            CalculateEngineTorque();
            Motor();
            Steering();
            UpdateWheels();
        }
        steerWheel.localEulerAngles = new Vector3(0f, 0f, -horizontalInput * 450f);
        AddDownForce();
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
            frontLeftWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.12f / (radius + (1.0f / 2))) * horizontalInput;
            frontRightWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.12f / (radius - (1.0f / 2))) * horizontalInput;
        }
        else if(horizontalInput < 0)
        {
            frontLeftWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.12f / (radius - (1.0f / 2))) * horizontalInput;
            frontRightWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.12f / (radius + (1.0f / 2))) * horizontalInput;
        }
        else
        {
            frontLeftWheelCollider.steerAngle = 0f;
            frontRightWheelCollider.steerAngle = 0f;
        }
    }

    float engineRpm = 0;
    private void CalculateEngineTorque()
    {
        float wheelRpm = CalculateWheelRpm();
        motorTorque = enginePower.Evaluate(engineRpm) * gears[currentGear] * verticalInput / 4.0f;
        float velocity = 0.0f;
        engineRpm = Mathf.SmoothDamp(engineRpm, 800 + (Mathf.Abs(wheelRpm) * 3.4f * gears[currentGear]), ref velocity, 0.02f);
        
    }

    private void ChangeGear()
    {
        if (engineRpm > 4800)
        {
            currentGear = Math.Clamp(currentGear + 1, 0, gears.Length-1);
        }
        else if (engineRpm < 2000)
        {
            currentGear = Math.Clamp(currentGear - 1, speed < 4.0f ? 0 : 1, gears.Length-1);
        }

        Debug.Log(engineRpm.ToString() + " " + (currentGear+1).ToString() + " " + speed.ToString());
    }

    private float CalculateWheelRpm()
    {
        float wheelRpm = 0.0f;
        wheelRpm = frontLeftWheelCollider.rpm + frontRightWheelCollider.rpm + rearLeftWheelCollider.rpm + rearRightWheelCollider.rpm;
        return wheelRpm/2f;
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

    private void LogCarStatusAnnotation()
    {
        if(Time.frameCount % 2 == 0)
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
}