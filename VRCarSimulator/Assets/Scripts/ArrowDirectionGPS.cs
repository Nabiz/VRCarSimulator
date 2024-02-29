using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDirectionGPS : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int direction;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PlayerCar")
        {
            other.gameObject.GetComponent<CarControllerWheel>().setDiretctionGPS(direction);
        }
    }
}
