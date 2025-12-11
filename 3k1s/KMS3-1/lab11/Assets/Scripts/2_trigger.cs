using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger : MonoBehaviour {

    public Light spot_light;

    void OnTriggerStay(Collider col)
    {
        if (col.name == "Player")
        {
            spot_light.enabled = true;
            spot_light.transform.Rotate(0, 1, 0);
        }
    }

    public Light point_light;

    private void OnTriggerEnter(Collider c1)
    {
        if (c1.name == "Player")
            point_light.enabled = true;
    }

    private void OnTriggerExit(Collider c1)
    {
        if (c1.name == "Player")
            point_light.enabled = false;
    }
}
