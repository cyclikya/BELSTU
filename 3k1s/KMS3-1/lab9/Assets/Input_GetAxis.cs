using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class Input_GetAxis : MonoBehaviour
{
    public float X;
    public float Z;
    public float x;
    public float y;
    private float verticalRotation = 0f;

    public float moveSpeed = 5f; 
    public float rotationSpeed = 3f; 

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        X = Input.GetAxis("Horizontal");
        Z = Input.GetAxis("Vertical");
        x = Input.GetAxis("Mouse X");
        y = Input.GetAxis("Mouse Y");

        transform.Translate(X * moveSpeed, 0, Z * moveSpeed);

        transform.Rotate(0, x * rotationSpeed, 0);

        verticalRotation -= y * rotationSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, 0, 90f); 

        Vector3 currentRotation = transform.localEulerAngles;
        currentRotation.x = verticalRotation;
        transform.localEulerAngles = currentRotation;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}