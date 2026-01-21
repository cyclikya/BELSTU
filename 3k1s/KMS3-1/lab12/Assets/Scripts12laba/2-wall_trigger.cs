using UnityEngine;

public class RotateWall : MonoBehaviour
{
    public Transform wall;         
    public float rotationSpeed = 30f;


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            wall.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }

}
