using UnityEngine;

public class core : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector3.forward);
    }
    void onCollisionEnter(Collision col)
    {
        Destroy(gameObject);
        Debug.Log("Снаряд разрушен об что-то");
    }
}
