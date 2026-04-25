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

    [Header("References")]
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
    [TextArea(2, 6)] [SerializeField] private string defaultSetupDescription = "КАМАЗ (самосвал) — это большегрузный автомобиль-самосвал, предназначенный для перевозки сыпучих грузов (песка, щебня, земли). Его главная особенность — кузов, который поднимается гидравликой и опрокидывается назад для автоматической разгрузки.";
    [TextArea(2, 6)] [SerializeField] private string kuzovDescription = "Кузов автомобиля, который поднимается гидравликой и опрокидывается назад для автоматической разгрузки.";
    [TextArea(2, 6)] [SerializeField] private string doorDescription = "Двери кабины водителя.";
    [TextArea(2, 6)] [SerializeField] private string lightsDescription = "Система внешнего освещения и сигналов.";
    [TextArea(2, 6)] [SerializeField] private string panelDescription = "Панель приборов, переключатели, педали и рычаги.";
    [TextArea(2, 6)] [SerializeField] private string steeringDescription = "Рулевое колесо и управление направлением.";
    [TextArea(2, 6)] [SerializeField] private string keyDescription = "Замок зажигания и запуск двигателя.";

    [Header("View")]
    [SerializeField] private float cameraMoveDuration = 0.8f;
    [SerializeField] private int practiceSceneBuildIndex = 1;

    private readonly Dictionary<Renderer, Material> highlightedRenderers = new Dictionary<Renderer, Material>();
    private readonly Color highlightOverlayColor = new Color(230f / 255f, 230f / 255f, 230f / 255f, 1f);
    private const float highlightStrength = 80f / 255f;

    private bool move;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 needPosition;
    private Quaternion needRotation;
    private float moveTimer;

    private void Awake()
    {
        SetDescription(defaultSetupDescription);
    }

    private void Update()
    {
        if (!move || targetCamera == null)
        {
            return;
        }

        moveTimer += Time.deltaTime;
        float t = Mathf.Clamp01(moveTimer / Mathf.Max(0.01f, cameraMoveDuration));
        float k = Mathf.SmoothStep(0f, 1f, t);

        targetCamera.transform.position = Vector3.Lerp(startPosition, needPosition, k);
        targetCamera.transform.rotation = Quaternion.Slerp(startRotation, needRotation, k);

        if (t >= 1f)
        {
            move = false;
        }
    }

    public void HoverSection(InstallationSection section)
    {
        ClearHighlight();
        ApplyHighlight(GetTargets(section));
    }

    public void UnhoverSection(InstallationSection section)
    {
        ClearHighlight();
    }

    public void OpenSection(InstallationSection section)
    {
        SetDescription(GetSectionDescription(section));
        Transform point = GetSectionViewPoint(section);
        if (point != null)
        {
            MoveCamera(point);
        }
    }

    public void FocusSetupStart()
    {
        ClearHighlight();
        SetDescription(defaultSetupDescription);
        if (startViewPoint != null)
        {
            MoveCamera(startViewPoint);
        }
    }

    public void LoadPracticeScene()
    {
        SceneManager.LoadScene(practiceSceneBuildIndex);
    }

    private void MoveCamera(Transform point)
    {
        if (targetCamera == null || point == null)
        {
            return;
        }

        startPosition = targetCamera.transform.position;
        startRotation = targetCamera.transform.rotation;
        needPosition = point.position;
        needRotation = point.rotation;
        moveTimer = 0f;
        move = true;
    }

    private void SetDescription(string text)
    {
        if (descriptionText != null)
        {
            descriptionText.text = text;
        }
    }

    private void ApplyHighlight(Transform[] roots)
    {
        if (roots == null)
        {
            return;
        }

        for (int i = 0; i < roots.Length; i++)
        {
            Transform root = roots[i];
            if (root == null)
            {
                continue;
            }

            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            for (int j = 0; j < renderers.Length; j++)
            {
                Renderer renderer = renderers[j];
                if (renderer == null || highlightedRenderers.ContainsKey(renderer))
                {
                    continue;
                }

                Material material = renderer.material;
                highlightedRenderers[renderer] = new Material(material);
                material.color = Color.Lerp(material.color, highlightOverlayColor, highlightStrength);
            }
        }
    }

    private void ClearHighlight()
    {
        foreach (KeyValuePair<Renderer, Material> pair in highlightedRenderers)
        {
            if (pair.Key != null)
            {
                pair.Key.material = pair.Value;
            }
        }

        highlightedRenderers.Clear();
    }
    
    private Transform[] GetTargets(InstallationSection section)
    {
        if (kamazContext == null)
        {
            return null;
        }

        if (section == InstallationSection.Kuzov) return kamazContext.GetSetupTargets(KamazContext.SetupSection.Kuzov);
        if (section == InstallationSection.Door) return kamazContext.GetSetupTargets(KamazContext.SetupSection.Door);
        if (section == InstallationSection.Lights) return kamazContext.GetSetupTargets(KamazContext.SetupSection.Lights);
        if (section == InstallationSection.Panel) return kamazContext.GetSetupTargets(KamazContext.SetupSection.Panel);
        if (section == InstallationSection.Steering) return kamazContext.GetSetupTargets(KamazContext.SetupSection.Steering);
        return kamazContext.GetSetupTargets(KamazContext.SetupSection.Key);
    }

    private Transform GetSectionViewPoint(InstallationSection section)
    {
        if (section == InstallationSection.Kuzov) return kuzovViewPoint;
        if (section == InstallationSection.Door) return doorViewPoint;
        if (section == InstallationSection.Lights) return lightsViewPoint;
        if (section == InstallationSection.Panel) return panelViewPoint;
        if (section == InstallationSection.Steering) return steeringViewPoint;
        return keyViewPoint;
    }

    private string GetSectionDescription(InstallationSection section)
    {
        if (section == InstallationSection.Kuzov) return kuzovDescription;
        if (section == InstallationSection.Door) return doorDescription;
        if (section == InstallationSection.Lights) return lightsDescription;
        if (section == InstallationSection.Panel) return panelDescription;
        if (section == InstallationSection.Steering) return steeringDescription;
        return keyDescription;
    }
}

