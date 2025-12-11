using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robot : MonoBehaviour {

    public float speed = 5f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.E))
        {
            movement += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.R))
        {
            movement += Vector3.back;
        }
        if (Input.GetKey(KeyCode.T))
        {
            movement += Vector3.left;
        }
        if (Input.GetKey(KeyCode.Y))
        {
            movement += Vector3.right;
        }

        transform.Translate(movement * speed * Time.deltaTime);
    }
}
