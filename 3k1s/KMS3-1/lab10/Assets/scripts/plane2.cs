using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plane2 : MonoBehaviour
{
    MeshRenderer rend;

    public float minX, maxX, minZ, maxZ;
    public float nY;

    public GameObject cubePrefab;
    public GameObject spherePrefab;

    void Start()
    {
        rend = gameObject.GetComponent<MeshRenderer>();

        minX = rend.bounds.min.x;
        maxX = rend.bounds.max.x;
        minZ = rend.bounds.min.z;
        maxZ = rend.bounds.max.z;

        nY = gameObject.transform.position.y + 5;
    }

    void Update()
    {
        float nX = Random.Range(minX, maxX);
        float nZ = Random.Range(minZ, maxZ);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject cube = Instantiate(cubePrefab, new Vector3(nX, nY, nZ), Quaternion.identity);
            cube.AddComponent<Rigidbody>();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject sphere = Instantiate(spherePrefab, new Vector3(nX, nY, nZ), Quaternion.identity);
            sphere.AddComponent<Rigidbody>();
        }

        if (Input.GetKey(KeyCode.W))
        {
            Quaternion rotZ = Quaternion.AngleAxis(-10f * Time.deltaTime, Vector3.forward);
            transform.rotation *= rotZ;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Quaternion rotZ = Quaternion.AngleAxis(-1, new Vector3(0, 0, 1));
            gameObject.transform.rotation *= rotZ;
        }
    }
}