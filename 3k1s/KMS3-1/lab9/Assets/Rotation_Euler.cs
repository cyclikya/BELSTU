using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation_Euler : MonoBehaviour
{
    private float angleX = 0f;
    private float angleZ = 0f;
    
    void Update()
    {
        angleX += 1;
        angleZ += 1;

        transform.eulerAngles = new Vector3(angleX, 0, angleZ);

    }
}