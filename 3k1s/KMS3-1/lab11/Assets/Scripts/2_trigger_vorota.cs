using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger_vorota : MonoBehaviour {

    public GameObject vorota_lev;
    public GameObject vorota_prav;


    void OnTriggerEnter(Collider col)
    {
        if (col.name == "robot")
        {
            vorota_lev.transform.position -= new Vector3(2f, 0, 0);
            vorota_prav.transform.position += new Vector3(2f, 0, 0);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.name == "robot")
        {
            vorota_lev.transform.position += new Vector3(2f, 0, 0);
            vorota_prav.transform.position -= new Vector3(2f, 0, 0);
        }
    }

    public GameObject vint;
    void OnTriggerStay(Collider col)
    {
        if (col.name == "robot")
        {
            vint.transform.Translate(2f * Time.deltaTime, 0, 2f * Time.deltaTime);
            vint.transform.Rotate(0, 5, 0);

        }
    }
}
