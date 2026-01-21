using UnityEngine;

public class Zadanie10 : MonoBehaviour
{
    float x;
    float y;
    void Update()
    {
        x = Input.GetAxis("Mouse X");
        y = Input.GetAxis("Mouse Y");

        transform.rotation *= Quaternion.AngleAxis(x, Vector3.up);
        transform.rotation *= Quaternion.AngleAxis(-y, Vector3.right);

        // transform.Rotate(y , x , 0);
    }
}
