using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private RectTransform speedNeedle;
    [SerializeField] private RectTransform rpmNeedle;
    [SerializeField] private CarControllerWheel playerCar;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI gearText;

    // Update is called once per frame
    private void Start()
    {
        InvokeRepeating("UpdateSpeedometerHUD", 1f, 0.1f);
    }

    private void UpdateSpeedometerHUD()
    {
        speedNeedle.localEulerAngles = new Vector3(0f, 0f, 224f - 1.5f * playerCar.getSpeed());
        rpmNeedle.localEulerAngles = new Vector3(0f, 0f, 224f - 0.0375f * playerCar.getEngineRpm());
        speedText.text = Math.Round(playerCar.getSpeed()).ToString();

        gearText.text = playerCar.getCurrentGear() == -1 ? "R" : playerCar.getCurrentGear().ToString();
    }
}
