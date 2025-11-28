using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plane : MonoBehaviour
{
    MeshRenderer rend;
    public float minX, maxX, minZ, maxZ;
    public float nX, nY, nZ;

    void Start()
    {
        rend = gameObject.GetComponent<MeshRenderer>();
        nY = gameObject.transform.position.y + 5;
    }

    void Update()
    {
        minX = rend.bounds.min.x;
        maxX = rend.bounds.max.x;
        minZ = rend.bounds.min.z;
        maxZ = rend.bounds.max.z;

        nX = Random.Range(minX, maxX);
        nZ = Random.Range(minZ, maxZ);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            cube.transform.position = new Vector3(nX, nY, nZ);
            cube.AddComponent<Rigidbody>();
        }

        if (Input.GetKey(KeyCode.W))
        {
            Quaternion rotZ = Quaternion.AngleAxis(-10f * Time.deltaTime, Vector3.forward);
            transform.rotation *= rotZ;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Quaternion rotZ = Quaternion.AngleAxis(-1, Vector3.forward);
            transform.rotation *= rotZ;
        }
    }
}
