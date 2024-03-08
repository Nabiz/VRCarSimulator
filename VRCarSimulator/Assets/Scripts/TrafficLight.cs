using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [SerializeField] private MeshRenderer trafficLight;

    [Header("Materials")]
    [SerializeField] private Material trafficLightMaterial;
    [SerializeField] private Material red, yellow, green, redLight, yellowLight, greenLight;
    void Start()
    {
        ChangeLight("Stop");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeLight(string lightStatus)
    {
        switch(lightStatus)
        {
            case "Stop":
                Material[] stopMaterials = { trafficLightMaterial, redLight, yellow, green, red, yellow, greenLight };
                trafficLight.materials = stopMaterials;
                break;
            case "Ready":
                Material[] readyMaterials = { trafficLightMaterial, redLight, yellowLight, green, redLight, yellow, green };
                trafficLight.materials = readyMaterials;
                break;
            case "Go":
                Material[] goMaterials = { trafficLightMaterial, red, yellow, greenLight, redLight, yellow, green };
                trafficLight.materials = goMaterials;
                break;
            case "Warn":
                Material[] warnMaterials = { trafficLightMaterial, red, yellowLight, green, redLight, yellow, green };
                trafficLight.materials = warnMaterials;
                break;
        }
    }
}
