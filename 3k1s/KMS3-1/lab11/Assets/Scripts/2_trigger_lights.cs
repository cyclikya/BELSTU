using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger_lights : MonoBehaviour {

    public GameObject Cylinder;
    public Light Point1;
    public Light Point2;
    public Light Point3;

    void OnTriggerStay(Collider col)
    {
        if (col.name == "robot")
        {
            Cylinder.transform.Rotate(0, 5, 0);

            if (Point1.intensity < 10)
                Point1.intensity += 0.01f;
            else
                Point1.intensity = 0;

            if (Point2.intensity < 10)
                Point2.intensity += 0.01f;
            else
                Point2.intensity = 0;

            if (Point3.intensity < 10)
                Point3.intensity += 0.01f;
            else
                Point3.intensity = 0;
        }
    }
}
