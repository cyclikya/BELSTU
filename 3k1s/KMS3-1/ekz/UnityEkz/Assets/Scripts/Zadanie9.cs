using UnityEngine;

public class Zadanie9 : MonoBehaviour
{
    float x;
    float z;

    void Update()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        transform.Translate(x * Time.deltaTime, 0, z * Time.deltaTime);
    }
}
