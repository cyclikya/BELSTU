using UnityEngine;

public class Tank : MonoBehaviour
{
    Transform bash;
    Transform stv;
    public float TankMoveSpeed = 0.1f;
    public float RotateSpeed = 1f;
    
    void Start()
    {
        bash = gameObject.transform.Find("bashnya");
        stv = bash.transform.Find("dylo");
    }

    void Update()
    {
        float z = Input.GetAxis("Vertical") * TankMoveSpeed;
        transform.Translate(0, 0, z);

        float x = Input.GetAxis("Horizontal") * RotateSpeed;
        transform.Rotate(0f, x, 0f);

        float h = Input.GetAxis("Mouse X") * RotateSpeed;
        bash.Rotate(0f, h, 0f);

        float v = Input.GetAxis("Mouse Y");
        stv.transform.Rotate(0, 0, v);
    }
}