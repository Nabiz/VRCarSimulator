using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private RectTransform speedNeedle;
    [SerializeField] private CarControllerWheel playerCar;

    // Update is called once per frame
    void FixedUpdate()
    {
        speedNeedle.localEulerAngles = new Vector3(0f, 0f, 224f - 1.5f * playerCar.getSpeed());
    }
}
