using UnityEngine;

public class MiniCameraOverlay : MonoBehaviour
{
    [Header("Source Camera")]
    [SerializeField] private Camera sourceCamera;
    [SerializeField] private KamazContext kamazContext;

    [Header("Orbit")]
    [SerializeField] private Transform orbitTarget;
    [SerializeField] private bool orbitByArrowKeys = true;
    [SerializeField] private float orbitDistance = 6f;
    [SerializeField] private float orbitYawSpeed = 90f;
    [SerializeField] private float orbitPitchSpeed = 65f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Window (pixels)")]
    [SerializeField] private int windowWidth = 360;
    [SerializeField] private int windowHeight = 200;
    [SerializeField] private int margin = 16;
    [SerializeField] private bool topRight = true;

    [Header("Frame")]
    [SerializeField] private bool drawFrame = true;
    [SerializeField] private Color frameColor = new Color(0f, 0f, 0f, 0.75f);
    [SerializeField] private int frameThickness = 2;

    private RenderTexture miniTexture;
    private Texture2D whiteTexture;
    private int lastWidth;
    private int lastHeight;
    private float orbitYaw;
    private float orbitPitch = 15f;
    private bool orbitInitialized;

    private void Awake()
    {
        ResolveContext();
        AutoResolveSourceCamera();
        AutoResolveOrbitTarget();
        EnsureWhiteTexture();
        AllocateRenderTexture();
        BindCamera();
        InitializeOrbitFromCurrentCamera();
    }

    private void OnEnable()
    {
        ResolveContext();
        AutoResolveSourceCamera();
        AutoResolveOrbitTarget();
        BindCamera();
    }

    private void Update()
    {
        if (sourceCamera == null)
        {
            AutoResolveSourceCamera();
            BindCamera();
        }

        if (orbitTarget == null)
        {
            AutoResolveOrbitTarget();
        }

        UpdateOrbit();

        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            AllocateRenderTexture();
            BindCamera();
        }
    }

    private void OnGUI()
    {
        if (miniTexture == null || sourceCamera == null)
        {
            return;
        }

        Rect rect = GetWindowRect();

        if (drawFrame)
        {
            DrawRect(new Rect(
                rect.x - frameThickness,
                rect.y - frameThickness,
                rect.width + frameThickness * 2,
                rect.height + frameThickness * 2
            ), frameColor);
        }

        GUI.DrawTexture(rect, miniTexture, ScaleMode.StretchToFill, false);
    }

    private void OnDisable()
    {
        if (sourceCamera != null && sourceCamera.targetTexture == miniTexture)
        {
            sourceCamera.targetTexture = null;
        }
    }

    private void OnDestroy()
    {
        if (sourceCamera != null && sourceCamera.targetTexture == miniTexture)
        {
            sourceCamera.targetTexture = null;
        }

        if (miniTexture != null)
        {
            miniTexture.Release();
            Destroy(miniTexture);
            miniTexture = null;
        }

        if (whiteTexture != null)
        {
            Destroy(whiteTexture);
            whiteTexture = null;
        }
    }

    private Rect GetWindowRect()
    {
        float x = topRight ? Screen.width - windowWidth - margin : margin;
        float y = margin;
        return new Rect(x, y, windowWidth, windowHeight);
    }

    private void BindCamera()
    {
        if (sourceCamera == null || miniTexture == null)
        {
            return;
        }

        sourceCamera.enabled = true;
        sourceCamera.targetTexture = miniTexture;
    }

    private void AllocateRenderTexture()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;

        int safeWidth = Mathf.Max(16, windowWidth);
        int safeHeight = Mathf.Max(16, windowHeight);

        if (miniTexture != null && miniTexture.width == safeWidth && miniTexture.height == safeHeight)
        {
            return;
        }

        if (miniTexture != null)
        {
            miniTexture.Release();
            Destroy(miniTexture);
            miniTexture = null;
        }

        miniTexture = new RenderTexture(safeWidth, safeHeight, 16, RenderTextureFormat.ARGB32)
        {
            name = "MiniCameraOverlayRT"
        };
        miniTexture.Create();
    }

    private void EnsureWhiteTexture()
    {
        if (whiteTexture != null)
        {
            return;
        }

        whiteTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
    }

    private void DrawRect(Rect rect, Color color)
    {
        Color prev = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, whiteTexture);
        GUI.color = prev;
    }

    private void UpdateOrbit()
    {
        if (sourceCamera == null || orbitTarget == null || !orbitByArrowKeys)
        {
            return;
        }

        if (!orbitInitialized)
        {
            InitializeOrbitFromCurrentCamera();
        }

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
        sourceCamera.transform.position = orbitTarget.position + offset;
        sourceCamera.transform.LookAt(orbitTarget.position, Vector3.up);
    }

    private void InitializeOrbitFromCurrentCamera()
    {
        if (sourceCamera == null || orbitTarget == null)
        {
            return;
        }

        Vector3 offset = sourceCamera.transform.position - orbitTarget.position;
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

    private void AutoResolveOrbitTarget()
    {
        ResolveContext();

        if (orbitTarget != null)
        {
            return;
        }

        if (kamazContext != null)
        {
            if (kamazContext.Kabina != null)
            {
                orbitTarget = kamazContext.Kabina;
                orbitInitialized = false;
                return;
            }

            if (kamazContext.Root != null)
            {
                orbitTarget = kamazContext.Root;
                orbitInitialized = false;
                return;
            }
        }

        if (sourceCamera != null && sourceCamera.transform.parent != null)
        {
            orbitTarget = sourceCamera.transform.parent;
            orbitInitialized = false;
            return;
        }

        Camera main = Camera.main;
        if (main != null)
        {
            orbitTarget = main.transform;
            orbitInitialized = false;
        }
    }

    private void AutoResolveSourceCamera()
    {
        if (sourceCamera != null)
        {
            return;
        }

#if UNITY_2023_1_OR_NEWER
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
        Camera[] cameras = FindObjectsOfType<Camera>();
#endif

        for (int i = 0; i < cameras.Length; i++)
        {
            Camera cam = cameras[i];
            if (cam == null)
            {
                continue;
            }

            if (cam == Camera.main)
            {
                continue;
            }

            sourceCamera = cam;
            orbitInitialized = false;
            break;
        }
    }

    private void ResolveContext()
    {
        if (kamazContext != null)
        {
            return;
        }

        kamazContext = KamazContext.Instance;
        if (kamazContext == null)
        {
#if UNITY_2023_1_OR_NEWER
            kamazContext = FindFirstObjectByType<KamazContext>();
#else
            kamazContext = FindObjectOfType<KamazContext>();
#endif
        }
    }
}
