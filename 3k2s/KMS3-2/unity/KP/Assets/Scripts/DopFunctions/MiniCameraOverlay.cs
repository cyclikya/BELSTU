using UnityEngine;
using UnityEngine.UI;

public class MiniCameraOverlay : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] private Camera miniCamera;
    [SerializeField] private RenderTexture outputTexture;
    [SerializeField] private RawImage outputImage;
    [SerializeField] private Transform orbitTarget;

    [Header("Orbit Control")]
    [SerializeField] private bool rotateByArrowKeys = true;
    [SerializeField] private float orbitDistance = 6f;
    [SerializeField] private float orbitYawSpeed = 90f;
    [SerializeField] private float orbitPitchSpeed = 65f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Camera")]
    [SerializeField] private bool enableCameraOnStart = true;

    private float orbitYaw;
    private float orbitPitch = 15f;
    private bool orbitInitialized;

    private void Awake()
    {
        ApplySceneReferences();
        InitializeOrbitFromCurrentCamera();
    }

    private void OnEnable()
    {
        ApplySceneReferences();
    }

    private void Update()
    {
        if (miniCamera == null || orbitTarget == null || !rotateByArrowKeys)
        {
            return;
        }

        if (!orbitInitialized)
        {
            InitializeOrbitFromCurrentCamera();
        }

        UpdateOrbit();
    }

    private void OnDisable()
    {
        if (miniCamera != null && miniCamera.targetTexture == outputTexture)
        {
            miniCamera.targetTexture = null;
        }
    }

    private void UpdateOrbit()
    {
        float yawInput = 0f;
        float pitchInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            yawInput -= 1f;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            yawInput += 1f;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            pitchInput += 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            pitchInput -= 1f;
        }

        orbitYaw += yawInput * orbitYawSpeed * Time.deltaTime;
        orbitPitch += pitchInput * orbitPitchSpeed * Time.deltaTime;
        orbitPitch = Mathf.Clamp(orbitPitch, minPitch, maxPitch);

        Quaternion orbitRotation = Quaternion.Euler(orbitPitch, orbitYaw, 0f);
        Vector3 offset = orbitRotation * new Vector3(0f, 0f, -Mathf.Max(0.1f, orbitDistance));
        miniCamera.transform.position = orbitTarget.position + offset;
        miniCamera.transform.LookAt(orbitTarget.position, Vector3.up);
    }

    private void InitializeOrbitFromCurrentCamera()
    {
        if (miniCamera == null || orbitTarget == null)
        {
            return;
        }

        Vector3 offset = miniCamera.transform.position - orbitTarget.position;
        float distance = offset.magnitude;
        if (distance > 0.001f)
        {
            orbitDistance = distance;
            orbitYaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
            orbitPitch = Mathf.Asin(offset.y / distance) * Mathf.Rad2Deg;
            orbitPitch = Mathf.Clamp(orbitPitch, minPitch, maxPitch);
        }

        orbitInitialized = true;
    }

    [ContextMenu("Apply Scene References")]
    public void ApplySceneReferences()
    {
        if (miniCamera != null && outputTexture != null)
        {
            miniCamera.targetTexture = outputTexture;
            if (enableCameraOnStart)
            {
                miniCamera.enabled = true;
            }
        }

        if (outputImage != null)
        {
            outputImage.texture = outputTexture;
        }
        orbitInitialized = false;
    }
}
