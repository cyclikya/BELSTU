using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Lab6UIController : MonoBehaviour
{
    public enum InstallationSection
    {
        Kuzov = 0,
        Door = 1,
        Lights = 2,
        Panel = 3,
        Steering = 4,
        Key = 5
    }

    private static readonly string[] KuzovNodes = { "kuzov", "kryshka", "gidravl" };
    private static readonly string[] DoorNodes = { "doorL", "doorR" };
    private static readonly string[] LightNodes =
    {
        "fary",
        "bDriveSignal_L", "bDriveSignal_R",
        "fara_FL", "fara_FR",
        "turnSignal_FL", "turnSignal_FR",
        "turnSignal_BL", "turnSignal_BR",
        "stopSignal_BL", "stopSignal_BR"
    };
    private static readonly string[] PanelNodes =
    {
        "panel",
        "btn_L", "btn_R",
        "switcher_avariyka", "switcher_dvorniki", "switcher_fary", "switcher_kuzov",
        "pedal_gaz", "pedal_sceplenie", "pedal_tormoz",
        "peredachi",
        "spidometr", "strelka_spid",
        "tachometr", "strelka_tach"
    };
    private static readonly string[] SteeringNodes = { "ryle" };
    private static readonly string[] KeyNodes = { "key" };

    [System.Serializable]
    private class SectionRuntime
    {
        public Transform viewPoint;
        public string description;
        public string[] nodeNames;
        public readonly List<Transform> targets = new List<Transform>();
    }

    [Header("Main References")]
    [SerializeField] private KamazContext kamazContext;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform startViewPoint;
    [SerializeField] private Component descriptionTextComponent;

    [Header("Scenes")]
    [SerializeField] private int practiceSceneBuildIndex = 1;
    [SerializeField] private string practiceSceneName = "\u041F\u0440\u0430\u043A\u0442\u0438\u043A\u0430";

    [Header("View Points")]
    [SerializeField] private Transform kuzovViewPoint;
    [SerializeField] private Transform doorViewPoint;
    [SerializeField] private Transform lightsViewPoint;
    [SerializeField] private Transform panelViewPoint;
    [SerializeField] private Transform steeringViewPoint;
    [SerializeField] private Transform keyViewPoint;

    [Header("Descriptions")]
    [TextArea(2, 6)]
    [SerializeField] private string defaultSetupDescription = "Выберите узел, чтобы посмотреть его описание.";
    [TextArea(2, 6)]
    [SerializeField] private string kuzovDescription = "Кузов КамАЗа и подъемный механизм.";
    [TextArea(2, 6)]
    [SerializeField] private string doorDescription = "Двери кабины водителя.";
    [TextArea(2, 6)]
    [SerializeField] private string lightsDescription = "Система внешнего освещения и сигналов.";
    [TextArea(2, 6)]
    [SerializeField] private string panelDescription = "Панель приборов, переключатели, педали и рычаги.";
    [TextArea(2, 6)]
    [SerializeField] private string steeringDescription = "Рулевое колесо и управление направлением.";
    [TextArea(2, 6)]
    [SerializeField] private string keyDescription = "Замок зажигания и запуск двигателя.";

    [Header("Camera Move")]
    [SerializeField] private float cameraMoveDuration = 0.8f;
    [SerializeField] private AnimationCurve cameraMoveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Highlight")]
    [SerializeField] private Color highlightColor = new Color(230f / 255f, 230f / 255f, 230f / 255f, 45f / 255f);

    private readonly Dictionary<InstallationSection, SectionRuntime> sectionMap = new Dictionary<InstallationSection, SectionRuntime>();
    private readonly Dictionary<Renderer, RendererColorState> highlightedRenderers = new Dictionary<Renderer, RendererColorState>();

    private int highlightedSectionIndex = -1;
    private Coroutine moveCoroutine;
    private TMP_Text descriptionTmpText;
    private Text descriptionUiText;

    private void Awake()
    {
        ResolveKamazContext();
        ResolveCamera();
        InitializeSectionMap();
        ResolveSectionTargets();
        CacheDescriptionTarget();
        SetDescription(defaultSetupDescription);
    }

    private void OnDisable()
    {
        ClearHighlight();
    }

    [ContextMenu("Resolve Section Targets")]
    public void ResolveSectionTargets()
    {
        ResolveKamazContext();
        InitializeSectionMap();

        foreach (KeyValuePair<InstallationSection, SectionRuntime> pair in sectionMap)
        {
            SectionRuntime section = pair.Value;
            section.targets.Clear();

            if (section.nodeNames == null)
            {
                continue;
            }

            for (int i = 0; i < section.nodeNames.Length; i++)
            {
                string nodeName = section.nodeNames[i];
                Transform node = ResolveNode(nodeName);
                if (node != null && !section.targets.Contains(node))
                {
                    section.targets.Add(node);
                }
            }
        }
    }

    public void HoverSection(InstallationSection section)
    {
        SectionRuntime runtime = GetSection(section);
        if (runtime == null)
        {
            return;
        }

        if (runtime.targets.Count == 0)
        {
            ResolveSectionTargets();
        }

        if (runtime.targets.Count == 0)
        {
            ClearHighlight();
            return;
        }

        if (highlightedSectionIndex == (int)section)
        {
            return;
        }

        ClearHighlight();
        ApplyHighlight(runtime.targets);
        highlightedSectionIndex = (int)section;
    }

    public void UnhoverSection(InstallationSection section)
    {
        if (highlightedSectionIndex == (int)section)
        {
            ClearHighlight();
        }
    }

    public void OpenSection(InstallationSection section)
    {
        SectionRuntime runtime = GetSection(section);
        if (runtime == null)
        {
            return;
        }

        HoverSection(section);
        SetDescription(runtime.description);

        if (runtime.viewPoint != null)
        {
            MoveCameraTo(runtime.viewPoint);
        }
    }

    public void HoverSectionByIndex(int sectionIndex)
    {
        if (!TryGetSection(sectionIndex, out InstallationSection section))
        {
            return;
        }

        HoverSection(section);
    }

    public void UnhoverSectionByIndex(int sectionIndex)
    {
        if (!TryGetSection(sectionIndex, out InstallationSection section))
        {
            return;
        }

        UnhoverSection(section);
    }

    public void OpenSectionByIndex(int sectionIndex)
    {
        if (!TryGetSection(sectionIndex, out InstallationSection section))
        {
            return;
        }

        OpenSection(section);
    }

    public void FocusSetupStart()
    {
        ClearHighlight();
        SetDescription(defaultSetupDescription);

        if (startViewPoint != null)
        {
            MoveCameraTo(startViewPoint);
        }
    }

    public void LoadPracticeScene()
    {
        if (practiceSceneBuildIndex >= 0 && practiceSceneBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(practiceSceneBuildIndex);
            return;
        }

        if (!string.IsNullOrWhiteSpace(practiceSceneName))
        {
            SceneManager.LoadScene(practiceSceneName);
        }
    }

    private void InitializeSectionMap()
    {
        EnsureSection(InstallationSection.Kuzov, KuzovNodes, kuzovViewPoint, kuzovDescription);
        EnsureSection(InstallationSection.Door, DoorNodes, doorViewPoint, doorDescription);
        EnsureSection(InstallationSection.Lights, LightNodes, lightsViewPoint, lightsDescription);
        EnsureSection(InstallationSection.Panel, PanelNodes, panelViewPoint, panelDescription);
        EnsureSection(InstallationSection.Steering, SteeringNodes, steeringViewPoint, steeringDescription);
        EnsureSection(InstallationSection.Key, KeyNodes, keyViewPoint, keyDescription);
    }

    private void EnsureSection(InstallationSection sectionId, string[] nodeNames, Transform viewPoint, string description)
    {
        if (!sectionMap.TryGetValue(sectionId, out SectionRuntime section))
        {
            section = new SectionRuntime();
            sectionMap[sectionId] = section;
        }

        section.nodeNames = nodeNames;
        section.viewPoint = viewPoint;
        section.description = description;
    }

    private SectionRuntime GetSection(InstallationSection section)
    {
        InitializeSectionMap();
        sectionMap.TryGetValue(section, out SectionRuntime runtime);
        return runtime;
    }

    private bool TryGetSection(int sectionIndex, out InstallationSection section)
    {
        section = InstallationSection.Kuzov;
        if (!System.Enum.IsDefined(typeof(InstallationSection), sectionIndex))
        {
            return false;
        }

        section = (InstallationSection)sectionIndex;
        return true;
    }

    private Transform ResolveNode(string nodeName)
    {
        if (string.IsNullOrWhiteSpace(nodeName))
        {
            return null;
        }

        Transform node = null;
        if (kamazContext != null)
        {
            node = kamazContext.GetNode(nodeName);
        }

        if (node != null)
        {
            return node;
        }

        return FindDeepChild(transform, nodeName);
    }

    private void MoveCameraTo(Transform point)
    {
        ResolveCamera();
        if (targetCamera == null || point == null)
        {
            return;
        }

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        moveCoroutine = StartCoroutine(MoveCameraRoutine(point));
    }

    private IEnumerator MoveCameraRoutine(Transform point)
    {
        Vector3 fromPos = targetCamera.transform.position;
        Quaternion fromRot = targetCamera.transform.rotation;
        Vector3 toPos = point.position;
        Quaternion toRot = point.rotation;

        float duration = Mathf.Max(0.01f, cameraMoveDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveT = cameraMoveCurve != null ? cameraMoveCurve.Evaluate(t) : t;

            targetCamera.transform.position = Vector3.Lerp(fromPos, toPos, curveT);
            targetCamera.transform.rotation = Quaternion.Slerp(fromRot, toRot, curveT);

            yield return null;
        }

        targetCamera.transform.SetPositionAndRotation(toPos, toRot);
        moveCoroutine = null;
    }

    private void ApplyHighlight(List<Transform> roots)
    {
        float t = Mathf.Clamp01(highlightColor.a);
        Color target = new Color(highlightColor.r, highlightColor.g, highlightColor.b, 1f);

        for (int i = 0; i < roots.Count; i++)
        {
            Transform root = roots[i];
            if (root == null)
            {
                continue;
            }

            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            for (int r = 0; r < renderers.Length; r++)
            {
                Renderer renderer = renderers[r];
                if (renderer == null || highlightedRenderers.ContainsKey(renderer))
                {
                    continue;
                }

                Material material = renderer.material;
                RendererColorState state = new RendererColorState
                {
                    hasBaseColor = material.HasProperty("_BaseColor"),
                    hasColor = material.HasProperty("_Color"),
                    hasEmission = material.HasProperty("_EmissionColor"),
                    baseColor = material.HasProperty("_BaseColor") ? material.GetColor("_BaseColor") : Color.white,
                    color = material.HasProperty("_Color") ? material.GetColor("_Color") : Color.white,
                    emissionColor = material.HasProperty("_EmissionColor") ? material.GetColor("_EmissionColor") : Color.black
                };

                highlightedRenderers[renderer] = state;

                if (state.hasBaseColor)
                {
                    material.SetColor("_BaseColor", Color.Lerp(state.baseColor, target, t));
                }

                if (state.hasColor)
                {
                    material.SetColor("_Color", Color.Lerp(state.color, target, t));
                }

                if (state.hasEmission)
                {
                    material.SetColor("_EmissionColor", Color.Lerp(state.emissionColor, state.emissionColor + target * 0.2f, t));
                }
            }
        }
    }

    private void ClearHighlight()
    {
        foreach (KeyValuePair<Renderer, RendererColorState> pair in highlightedRenderers)
        {
            Renderer renderer = pair.Key;
            if (renderer == null)
            {
                continue;
            }

            Material material = renderer.material;
            RendererColorState state = pair.Value;

            if (state.hasBaseColor)
            {
                material.SetColor("_BaseColor", state.baseColor);
            }

            if (state.hasColor)
            {
                material.SetColor("_Color", state.color);
            }

            if (state.hasEmission)
            {
                material.SetColor("_EmissionColor", state.emissionColor);
            }
        }

        highlightedRenderers.Clear();
        highlightedSectionIndex = -1;
    }

    private void ResolveCamera()
    {
        if (targetCamera != null)
        {
            return;
        }

        targetCamera = Camera.main;
        if (targetCamera != null)
        {
            return;
        }

#if UNITY_2023_1_OR_NEWER
        targetCamera = FindFirstObjectByType<Camera>();
#else
        targetCamera = FindObjectOfType<Camera>();
#endif
    }

    private void ResolveKamazContext()
    {
        if (kamazContext == null)
        {
            kamazContext = GetComponent<KamazContext>();
        }

        if (kamazContext == null)
        {
            kamazContext = KamazContext.Instance;
        }

        if (kamazContext == null)
        {
#if UNITY_2023_1_OR_NEWER
            kamazContext = FindFirstObjectByType<KamazContext>();
#else
            kamazContext = FindObjectOfType<KamazContext>();
#endif
        }

        if (kamazContext != null)
        {
            kamazContext.AutoResolve();
        }
    }

    private Transform FindDeepChild(Transform parent, string objectName)
    {
        if (parent == null || string.IsNullOrEmpty(objectName))
        {
            return null;
        }

        if (string.Equals(parent.name, objectName, System.StringComparison.OrdinalIgnoreCase))
        {
            return parent;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Transform found = FindDeepChild(child, objectName);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private void CacheDescriptionTarget()
    {
        descriptionTmpText = null;
        descriptionUiText = null;

        if (descriptionTextComponent != null)
        {
            descriptionTmpText = descriptionTextComponent as TMP_Text;
            descriptionUiText = descriptionTextComponent as Text;
        }

        if (descriptionTmpText == null && descriptionUiText == null)
        {
            descriptionTmpText = FindBestTmpTextCandidate();
        }

        if (descriptionTmpText == null && descriptionUiText == null)
        {
            descriptionUiText = FindBestUiTextCandidate();
        }
    }

    private TMP_Text FindBestTmpTextCandidate()
    {
        TMP_Text[] all;
#if UNITY_2023_1_OR_NEWER
        all = FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
        all = FindObjectsOfType<TMP_Text>(true);
#endif

        return FindBestTextByName(all);
    }

    private Text FindBestUiTextCandidate()
    {
        Text[] all;
#if UNITY_2023_1_OR_NEWER
        all = FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
        all = FindObjectsOfType<Text>(true);
#endif

        return FindBestTextByName(all);
    }

    private T FindBestTextByName<T>(T[] all) where T : Component
    {
        if (all == null || all.Length == 0)
        {
            return null;
        }

        T fallback = null;
        for (int i = 0; i < all.Length; i++)
        {
            T candidate = all[i];
            if (candidate == null)
            {
                continue;
            }

            if (fallback == null)
            {
                fallback = candidate;
            }

            string name = candidate.gameObject.name;
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            string lower = name.ToLowerInvariant();
            if (lower == "text" || lower.Contains("description") || lower.Contains("опис"))
            {
                return candidate;
            }
        }

        return fallback;
    }

    private void SetDescription(string value)
    {
        if (descriptionTmpText == null && descriptionUiText == null)
        {
            CacheDescriptionTarget();
        }

        string text = value ?? string.Empty;

        if (descriptionTmpText != null)
        {
            descriptionTmpText.text = text;
            return;
        }

        if (descriptionUiText != null)
        {
            descriptionUiText.text = text;
        }
    }

    private struct RendererColorState
    {
        public bool hasBaseColor;
        public bool hasColor;
        public bool hasEmission;
        public Color baseColor;
        public Color color;
        public Color emissionColor;
    }
}

