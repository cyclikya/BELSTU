using UnityEngine;

public class Zadanie12 : MonoBehaviour
{
    public GameObject prefab;
    GameObject obj;
    Rigidbody rb;

    void OnTriggerEnter(Collider col)
    {
        float x = Random.Range(0, 5);
        float z = Random.Range(0, 5);

        obj = Instantiate(prefab, new Vector3(x, 0, z), Quaternion.identity);
        rb = obj.AddComponent<Rigidbody>();
        rb.useGravity = true;
    }
}
