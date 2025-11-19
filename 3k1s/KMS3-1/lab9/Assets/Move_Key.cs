using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Key : MonoBehaviour
{
    public float speed = 2f; // скорость движения

    void Update()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            direction += new Vector3(0, 1, 0);

        if (Input.GetKey(KeyCode.S))
            direction += new Vector3(0, -1, 0);

        if (Input.GetKey(KeyCode.A))
            direction += new Vector3(-1, 0, 0);

        if (Input.GetKey(KeyCode.D))
            direction += new Vector3(1, 0, 0);

        if (Input.GetKey(KeyCode.Q))
            direction += new Vector3(0, 0, 1);

        if (Input.GetKey(KeyCode.E))
            direction += new Vector3(0, 0, -1);

        // Плавное движение
        transform.position += direction * speed * Time.deltaTime;
    }
}
