using UnityEngine;

public class GaugeNeedle : MonoBehaviour
{
    [Header("Input Range")]
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;

    [Header("Needle Angles (Local Y)")]
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;

    [Header("Smoothing")]
     private float smoothSpeed = 10f;

    [Header("Debug/Test")]
    private bool useDebugValue = false;
    [SerializeField] private float debugValue = 0f;

    private float targetValue;
    private Quaternion initialLocalRotation;

    public float Value
    {
        get => targetValue;
        set => SetValue(value);
    }

    public void SetValue(float value)
    {
        targetValue = Mathf.Clamp(value, minValue, maxValue);
    }

    public void SetValueImmediate(float value)
    {
        targetValue = Mathf.Clamp(value, minValue, maxValue);
        transform.localRotation = GetRotationForValue(targetValue);
    }

    public void Configure(float newMinValue, float newMaxValue, float newMinAngle, float newMaxAngle)
    {
        minValue = newMinValue;
        maxValue = newMaxValue;
        minAngle = newMinAngle;
        maxAngle = newMaxAngle;
        SetValue(targetValue);
    }

    public void SetDebugMode(bool enabled)
    {
        useDebugValue = enabled;
    }

    private void Awake()
    {
        initialLocalRotation = transform.localRotation;
        SetValue(debugValue);
    }

    private void Update()
    {
        if (useDebugValue)
        {
            SetValue(debugValue);
        }

        Quaternion targetRotation = GetRotationForValue(targetValue);
        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * smoothSpeed
        );
    }

    private Quaternion GetRotationForValue(float value)
    {
        float t = Mathf.InverseLerp(minValue, maxValue, Mathf.Clamp(value, minValue, maxValue));
        float y = Mathf.Lerp(minAngle, maxAngle, t);
        return initialLocalRotation * Quaternion.Euler(0f, y, 0f);
    }
}
