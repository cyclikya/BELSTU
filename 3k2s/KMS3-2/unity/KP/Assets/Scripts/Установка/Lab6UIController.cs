using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lab6UIController : MonoBehaviour
{
    public enum InstallationSection
    {
        Kuzov,
        Door,
        Lights,
        Panel,
        Steering,
        Key
    }

    private static readonly string[] KuzovNodes = { "kuzov"};
    private static readonly string[] DoorNodes = { "doorL", "doorR" };
    private static readonly string[] LightNodes = { "fary" };
    private static readonly string[] PanelNodes = { "panel" };
    private static readonly string[] SteeringNodes = { "ryle" };
    private static readonly string[] KeyNodes = { "key" };

    [Header("Required References")]
    [SerializeField] private KamazContext kamazContext;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Text descriptionText;

    [Header("View Points")]
    [SerializeField] private Transform startViewPoint;
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
    [SerializeField] private string panelDescription = "Панель приборов и переключатели.";
    [TextArea(2, 6)]
    [SerializeField] private string steeringDescription = "Рулевое колесо и управление направлением.";
    [TextArea(2, 6)]
    [SerializeField] private string keyDescription = "Замок зажигания и запуск двигателя.";

    [Header("Camera Move")]
    [SerializeField] private float cameraMoveDuration = 0.8f;

    [Header("Highlight")]
    [SerializeField] private Color highlightColor = new Color(230f / 255f, 230f / 255f, 230f / 255f, 45f / 255f);

    private readonly Dictionary<InstallationSection, List<Transform>> sectionTargets = new Dictionary<InstallationSection, List<Transform>>();
    private readonly Dictionary<Renderer, Material> highlightedRenderers = new Dictionary<Renderer, Material>();

    private bool move;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 needPosition;
    private Quaternion needRotation;
    private const int practiceSceneBuildIndex = 1;
    private float moveTimer;

    private void Awake()
    {
        if (kamazContext == null)
        {
            kamazContext = GetComponentInParent<KamazContext>();
        }

        if (targetCamera == null)
        {
            targetCamera = GetComponentInParent<Camera>();
        }

        if (descriptionText == null)
        {
            descriptionText = GetComponentInParent<Text>();
        }

        kamazContext.AutoResolve();
        RebuildTargets();
        SetDescription(defaultSetupDescription);
    }

    private void Update()
    {
        if (!move)
        {
            return;
        }

        float duration = Mathf.Max(0.01f, cameraMoveDuration);
        moveTimer += Time.deltaTime;
        float t = Mathf.Clamp01(moveTimer / duration);
        float k = Mathf.SmoothStep(0f, 1f, t);

        Transform cam = targetCamera.transform;
        cam.position = Vector3.Lerp(startPosition, needPosition, k);
        cam.rotation = Quaternion.Slerp(startRotation, needRotation, k);

        if (t >= 1f)
        {
            move = false;
        }
    }


    public void HoverSection(InstallationSection section)
    {
        if (!sectionTargets.TryGetValue(section, out List<Transform> targets) || targets.Count == 0)
        {
            ClearHighlight();
            return;
        }

        ClearHighlight();
        ApplyHighlight(targets);
    }

    public void UnhoverSection(InstallationSection section)
    {
        ClearHighlight();
    }

    public void OpenSection(InstallationSection section)
    {
        SetDescription(GetSectionDescription(section));

        Transform viewPoint = GetSectionViewPoint(section);
        if (viewPoint != null)
        {
            MoveCameraByViewPoint(viewPoint);
        }
    }

    private void MoveCameraByViewPoint(Transform point)
    {
        startPosition = targetCamera.transform.position;
        startRotation = targetCamera.transform.rotation;
        needPosition = point.position;
        needRotation = point.rotation;
        moveTimer = 0f;
        move = true;
    }

    private void SetDescription(string value)
    {
        descriptionText.text = value ?? string.Empty;
    }

    public void FocusSetupStart()
    {
        ClearHighlight();
        SetDescription(defaultSetupDescription);
        if (startViewPoint != null)
        {
            MoveCameraByViewPoint(startViewPoint);
        }
    }

    public void LoadPracticeScene()
    {
        if (practiceSceneBuildIndex >= 0 && practiceSceneBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(practiceSceneBuildIndex);
        }
        else
        {
            Debug.LogError("Lab6UIController: Некорректный build index для сцены Практика.");
        }
    }

    private void ApplyHighlight(List<Transform> roots)
    {
        for (int i = 0; i < roots.Count; i++)
        {
            Renderer[] renderers = roots[i].GetComponentsInChildren<Renderer>(true);
            for (int r = 0; r < renderers.Length; r++)
            {
                Renderer renderer = renderers[r];
                if (renderer == null || highlightedRenderers.ContainsKey(renderer))
                {
                    continue;
                }

                Material material = renderer.material;
                highlightedRenderers[renderer] = new Material(material);
                material.color = highlightColor;
            }
        }
    }

    private void ClearHighlight()
    {
        foreach (KeyValuePair<Renderer, Material> pair in highlightedRenderers)
        {
            Renderer renderer = pair.Key;
            if (renderer == null)
            {
                continue;
            }

            renderer.material = pair.Value;
        }

        highlightedRenderers.Clear();
    }

    public void RebuildTargets()
    {
        sectionTargets.Clear();
        BuildTargetsFor(InstallationSection.Kuzov);
        BuildTargetsFor(InstallationSection.Door);
        BuildTargetsFor(InstallationSection.Lights);
        BuildTargetsFor(InstallationSection.Panel);
        BuildTargetsFor(InstallationSection.Steering);
        BuildTargetsFor(InstallationSection.Key);
    }
    private void BuildTargetsFor(InstallationSection section)
    {
        string[] nodes = GetSectionNodeNames(section);
        List<Transform> targets = new List<Transform>();
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                Transform node = kamazContext.GetNode(nodes[i]);
                if (node != null && !targets.Contains(node))
                {
                    targets.Add(node);
                }
            }
        }

        sectionTargets[section] = targets;
    }

    private string[] GetSectionNodeNames(InstallationSection section)
    {
        switch (section)
        {
            case InstallationSection.Kuzov: return KuzovNodes;
            case InstallationSection.Door: return DoorNodes;
            case InstallationSection.Lights: return LightNodes;
            case InstallationSection.Panel: return PanelNodes;
            case InstallationSection.Steering: return SteeringNodes;
            case InstallationSection.Key: return KeyNodes;
            default: return null;
        }
    }

    private Transform GetSectionViewPoint(InstallationSection section)
    {
        switch (section)
        {
            case InstallationSection.Kuzov: return kuzovViewPoint;
            case InstallationSection.Door: return doorViewPoint;
            case InstallationSection.Lights: return lightsViewPoint;
            case InstallationSection.Panel: return panelViewPoint;
            case InstallationSection.Steering: return steeringViewPoint;
            case InstallationSection.Key: return keyViewPoint;
            default: return null;
        }
    }

    private string GetSectionDescription(InstallationSection section)
    {
        switch (section)
        {
            case InstallationSection.Kuzov: return kuzovDescription;
            case InstallationSection.Door: return doorDescription;
            case InstallationSection.Lights: return lightsDescription;
            case InstallationSection.Panel: return panelDescription;
            case InstallationSection.Steering: return steeringDescription;
            case InstallationSection.Key: return keyDescription;
            default: return defaultSetupDescription;
        }
    }

}
