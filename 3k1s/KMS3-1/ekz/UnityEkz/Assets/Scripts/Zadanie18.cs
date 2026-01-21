using UnityEngine;

public class Zadanie18 : MonoBehaviour
{
    float rotx = 0f;
    float roty = 0f;
    public Transform bashnya;
    public Transform dylo;

    public AudioSource dyloSource;
    public AudioSource tankSource;
    public AudioClip shootClip;


    public GameObject corePrefab;
    
    void Update()
    {
        float v = Input.GetAxis("Vertical");
        transform.Translate(0, 0, v);

        float h = Input.GetAxis("Horizontal");
        transform.Rotate(0f, h, 0f);

        float x = Input.GetAxis("Mouse X");
        rotx = Mathf.Clamp(rotx + x, -90, 90);
        bashnya.localRotation = Quaternion.Euler(0, rotx, 0);

        float y = Input.GetAxis("Mouse Y");
        roty = Mathf.Clamp(roty + y, -100, -60);
        dylo.localRotation = Quaternion.Euler(-roty, 0, 0);

        if(Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            if (!tankSource.isPlaying)
            {
                tankSource.Play();
            }
        } else 
        {
            if (tankSource.isPlaying)
            {
                tankSource.Stop();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 spawnPosition = dylo.position;
            GameObject core = Instantiate(corePrefab, spawnPosition, bashnya.rotation);
            
            dyloSource.PlayOneShot(shootClip);

            Destroy(core, 5);            
        }





    }
}