using UnityEngine;

public class ScriptPosition : MonoBehaviour
{
    public float speedX = 2f; 
    public float speedY = 1f;
    public float speedZ = 0f;

    void Update()
    {
        transform.position += new Vector3(speedX * Time.deltaTime,
                                          speedY * Time.deltaTime,
                                          speedZ * Time.deltaTime);
    }
}

