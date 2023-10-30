using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] GameObject spawnObject;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PlayerCar")
        {
            spawnObject.SetActive(true);
        }

    }
}
