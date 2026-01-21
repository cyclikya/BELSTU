using UnityEngine;

public class Zadanie11 : MonoBehaviour
{
    public GameObject prefab;
    float x;
    float z;
    Rigidbody rb;

    void Update()
    {
        x = Random.Range(-10, 10);
        z = Random.Range(-10, 10);
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject obj = Instantiate(prefab, new Vector3(x, 0, z), Quaternion.identity);
            rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = true;
        }
       
    }
}
