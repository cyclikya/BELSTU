using UnityEngine;

public class Core : MonoBehaviour
{
    public float speed = 30f;
    public GameObject explosion1; 
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("goal") || collision.gameObject.CompareTag("Player"))
        {
            if (explosion1 != null)
            {
                GameObject explosionInstance = Instantiate(explosion1, transform.position, Quaternion.identity);
                Destroy(explosionInstance, 3f);
            }
            
            Destroy(gameObject);
        }
    }
}