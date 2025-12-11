using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class capsule : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public GameObject Cube1;
    public GameObject Cube2;
    public Texture texture1;
    public Texture texture2;

    // Update is called once per frame
    void Update () {

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        transform.Translate(moveDirection * 3f * Time.deltaTime, Space.World);

        float mouseX = Input.GetAxis("Mouse X"); ;
        transform.Rotate(0f, mouseX * 120f * Time.deltaTime, 0f);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Cube1.GetComponent<Renderer>().material.mainTexture = texture1;
        }

    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject == Cube1)
        {
            Cube1.GetComponent<Renderer>().material.color = Color.red;
        }

        if (col.gameObject == Cube2)
        {
            Cube2.GetComponent<Renderer>().material.mainTexture = texture2;
        }
    }
}
