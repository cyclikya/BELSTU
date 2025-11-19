using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptScale : MonoBehaviour {

    void Update()
    {
        transform.localScale += new Vector3(0.2f, 0.2f, 0);
    }
}
