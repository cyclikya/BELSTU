using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation_Rotate : MonoBehaviour {

    public float rotationSpeed = 0f;

    void Update()
    {
        transform.Rotate(0, rotationSpeed, 0);
    }
}
