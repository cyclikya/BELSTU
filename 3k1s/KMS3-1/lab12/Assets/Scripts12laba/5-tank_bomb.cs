using UnityEngine;

public class Bombs : MonoBehaviour
{
    public GameObject bombPrefab;
    public float dropHeight = 200f;
    public float dropAreaMultiplier = 2f;
    public int bombsPerPress = 1;

    public AudioSource tankMoveAudio;

    private Collider tankCollider;

    void Start()
    {
        tankCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            if (!tankMoveAudio.isPlaying)
            {
                tankMoveAudio.Play();
            }
        }
        else
        {
            tankMoveAudio.Pause();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropBombs();
        }
    }

    void DropBombs()
    {
        Vector3 center = transform.position + transform.forward * tankCollider.bounds.size.z;
        Vector3 size = tankCollider.bounds.size * dropAreaMultiplier;

        for (int i = 0; i < bombsPerPress; i++)
        {
            float x = Random.Range(-size.x / 2, size.x / 2);
            float z = Random.Range(-size.z / 2, size.z / 2);

            Vector3 spawnPos = center + new Vector3(x, dropHeight, z);
            Instantiate(bombPrefab, spawnPos, Quaternion.identity);
        }
    }
}
