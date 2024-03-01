using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] GameObject spawnObject;
    [SerializeField] Animator animator;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PlayerCar")
        {
            // spawnObject.SetActive(true);
            animator.SetTrigger("AnimationTrigger");
        }

    }
}
