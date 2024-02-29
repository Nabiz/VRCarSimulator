using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour
{
    MeshRenderer meshRenderer;
    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    public void changeColor()
    {
        meshRenderer.material.color = Color.red;
        Invoke("restColor", 0.5f);
    }

    void restColor()
    {
        meshRenderer.material.color = Color.white;
    }
}
