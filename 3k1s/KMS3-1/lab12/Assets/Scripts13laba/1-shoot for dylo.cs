using UnityEngine;

public class стрелять : MonoBehaviour
{
    public GameObject core;
    public AudioClip shootSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Стрельба как в условии бота (но для игрока)
        Vector3 spawnPos = transform.position + transform.forward * 30f;
        GameObject newCore = Instantiate(core, spawnPos, transform.rotation);
        
        // Назначаем тег
        newCore.tag = "core";
        
        // Звук
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}