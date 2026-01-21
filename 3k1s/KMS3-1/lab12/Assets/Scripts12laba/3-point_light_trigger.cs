using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    public Light targetLight;
    public float enterIntensity = 100f;

    private float originalIntensity;

    void Start()
    {
        originalIntensity = targetLight.intensity;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            targetLight.intensity = enterIntensity;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            targetLight.intensity = originalIntensity;
    }
}