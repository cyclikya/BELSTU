using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop_Bomb : MonoBehaviour {

    public AudioClip spawnSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 1f;

        if (spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player")) 
        {
            Destroy(gameObject);
        }
    }
}
