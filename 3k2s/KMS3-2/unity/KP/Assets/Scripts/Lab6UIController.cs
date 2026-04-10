using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lab6UIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Text infoText;
    [SerializeField] private GameObject helpWindow;
    [SerializeField] private string defaultInfoText = "Наведите курсор на кнопку элемента установки.";

    [Header("Camera")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float cameraMoveSpeed = 2f;
    [SerializeField] private Transform viewShassi;
    [SerializeField] private Transform viewKabina;
    [SerializeField] private Transform viewDveri;
    [SerializeField] private Transform viewPanel;
    [SerializeField] private Transform viewKuzov;
    [SerializeField] private Transform viewGidravl;

    [Header("Elements")]
    [SerializeField] private Transform shassi;
    [SerializeField] private Transform kabina;
    [SerializeField] private Transform doorL;
    [SerializeField] private Transform doorR;
    [SerializeField] private Transform panel;
    [SerializeField] private Transform kuzov;
    [SerializeField] private Transform gidravl;

    [Header("Highlight")]
    [SerializeField] private Color highlightColor = new Color(1f, 0.9f, 0.2f, 1f);
    [SerializeField] [Range(0f, 1f)] private float highlightStrength = 0.35f;

    private readonly Dictionary<Renderer, RendererColorState> rendererStateCache = new Dictionary<Renderer, RendererColorState>();

    private bool moveCamera;
    private float cameraOffset;
    private Vector3 startPosition;
    private Vector3 needPosition;
    private Quaternion startRotation;
    private Quaternion needRotation;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (infoText != null)
        {
            infoText.text = defaultInfoText;
        }

        if (helpWindow != null)
        {
            helpWindow.SetActive(false);
        }

        CacheElementRenderers(shassi);
        CacheElementRenderers(kabina);
        CacheElementRenderers(doorL);
        CacheElementRenderers(doorR);
        CacheElementRenderers(panel);
        CacheElementRenderers(kuzov);
        CacheElementRenderers(gidravl);
    }

    private void Update()
    {
        if (!moveCamera || targetCamera == null)
        {
            return;
        }

        cameraOffset += Time.deltaTime * cameraMoveSpeed;
        targetCamera.transform.position = Vector3.Lerp(startPosition, needPosition, cameraOffset);
        targetCamera.transform.rotation = Quaternion.Slerp(startRotation, needRotation, cameraOffset);

        if (cameraOffset >= 1f)
        {
            moveCamera = false;
            cameraOffset = 0f;
            targetCamera.transform.position = needPosition;
            targetCamera.transform.rotation = needRotation;
        }
    }

    public void OpenHelp()
    {
        if (helpWindow != null)
        {
            helpWindow.SetActive(true);
        }
    }

    public void CloseHelp()
    {
        if (helpWindow != null)
        {
            helpWindow.SetActive(false);
        }
    }

    public void OnShassiEnter()
    {
        SetInfo("Шасси — базовая несущая часть КамАЗа, на которой закреплены основные узлы.");
        SetHighlight(shassi, true);
    }

    public void OnShassiExit()
    {
        SetHighlight(shassi, false);
        ResetInfo();
    }

    public void OnShassiClick()
    {
        MoveCameraTo(viewShassi);
    }

    public void OnKabinaEnter()
    {
        SetInfo("Кабина — рабочее место водителя, где расположены органы управления.");
        SetHighlight(kabina, true);
    }

    public void OnKabinaExit()
    {
        SetHighlight(kabina, false);
        ResetInfo();
    }

    public void OnKabinaClick()
    {
        MoveCameraTo(viewKabina);
    }

    public void OnDveriEnter()
    {
        SetInfo("Двери — обеспечивают доступ в кабину и выход из нее.");
        SetHighlight(doorL, true);
        SetHighlight(doorR, true);
    }

    public void OnDveriExit()
    {
        SetHighlight(doorL, false);
        SetHighlight(doorR, false);
        ResetInfo();
    }

    public void OnDveriClick()
    {
        MoveCameraTo(viewDveri);
    }

    public void OnPanelEnter()
    {
        SetInfo("Панель управления — содержит переключатели и кнопки управления системами.");
        SetHighlight(panel, true);
    }

    public void OnPanelExit()
    {
        SetHighlight(panel, false);
        ResetInfo();
    }

    public void OnPanelClick()
    {
        MoveCameraTo(viewPanel);
    }

    public void OnKuzovEnter()
    {
        SetInfo("Кузов — грузовая часть автомобиля для перевозки материалов.");
        SetHighlight(kuzov, true);
    }

    public void OnKuzovExit()
    {
        SetHighlight(kuzov, false);
        ResetInfo();
    }

    public void OnKuzovClick()
    {
        MoveCameraTo(viewKuzov);
    }

    public void OnGidravlEnter()
    {
        SetInfo("Гидравлический пресс — механизм подъема и опускания кузова.");
        SetHighlight(gidravl, true);
    }

    public void OnGidravlExit()
    {
        SetHighlight(gidravl, false);
        ResetInfo();
    }

    public void OnGidravlClick()
    {
        MoveCameraTo(viewGidravl);
    }

    private void MoveCameraTo(Transform viewPoint)
    {
        if (targetCamera == null || viewPoint == null)
        {
            return;
        }

        startPosition = targetCamera.transform.position;
        startRotation = targetCamera.transform.rotation;
        needPosition = viewPoint.position;
        needRotation = viewPoint.rotation;
        cameraOffset = 0f;
        moveCamera = true;
    }

    private void SetInfo(string textValue)
    {
        if (infoText != null)
        {
            infoText.text = textValue;
        }
    }

    private void ResetInfo()
    {
        if (infoText != null)
        {
            infoText.text = defaultInfoText;
        }
    }

    private void CacheElementRenderers(Transform root)
    {
        if (root == null)
        {
            return;
        }

        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null || rendererStateCache.ContainsKey(renderer))
            {
                continue;
            }

            Material material = renderer.material;
            RendererColorState state = new RendererColorState
            {
                hasBaseColor = material.HasProperty("_BaseColor"),
                hasColor = material.HasProperty("_Color"),
                baseColor = material.HasProperty("_BaseColor") ? material.GetColor("_BaseColor") : Color.white,
                color = material.HasProperty("_Color") ? material.GetColor("_Color") : Color.white
            };

            rendererStateCache.Add(renderer, state);
        }
    }

    private void SetHighlight(Transform root, bool enabled)
    {
        if (root == null)
        {
            return;
        }

        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null || !rendererStateCache.TryGetValue(renderer, out RendererColorState state))
            {
                continue;
            }

            Material material = renderer.material;
            if (enabled)
            {
                if (state.hasBaseColor)
                {
                    material.SetColor("_BaseColor", Color.Lerp(state.baseColor, highlightColor, highlightStrength));
                }

                if (state.hasColor)
                {
                    material.SetColor("_Color", Color.Lerp(state.color, highlightColor, highlightStrength));
                }
            }
            else
            {
                if (state.hasBaseColor)
                {
                    material.SetColor("_BaseColor", state.baseColor);
                }

                if (state.hasColor)
                {
                    material.SetColor("_Color", state.color);
                }
            }
        }
    }

    private struct RendererColorState
    {
        public bool hasBaseColor;
        public bool hasColor;
        public Color baseColor;
        public Color color;
    }
}
