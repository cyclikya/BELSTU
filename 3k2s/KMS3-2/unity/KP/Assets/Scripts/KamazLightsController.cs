using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamazLightsController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode headlightsKey = KeyCode.F;
    [SerializeField] private KeyCode hazardKey = KeyCode.X;
    [SerializeField] private KeyCode leftTurnKey = KeyCode.Q;
    [SerializeField] private KeyCode rightTurnKey = KeyCode.E;

    [Header("Blink Settings")]
    [SerializeField] private float blinkInterval = 0.5f;
    [SerializeField] private float turnSignalDuration = 10f;

    [Header("Lamp Material Highlight")]
    [SerializeField] private Color whiteLampHighlightColor = new Color(230f / 255f, 230f / 255f, 230f / 255f, 100f / 255f);
    [SerializeField] private Color redLampHighlightColor = new Color(1f, 0.25f, 0.25f, 100f / 255f);
    [SerializeField] private Color yellowLampHighlightColor = new Color(1f, 0.9f, 0.2f, 100f / 255f);
    [SerializeField] [Range(0f, 1f)] private float lampBaseBrightenStrength = 0.45f;
    [SerializeField] [Range(0f, 2f)] private float lampEmissionBoost = 1.2f;

    private const string FaraFL = "fara_FL";
    private const string FaraFR = "fara_FR";
    private const string TurnBL = "turnSignal_BL";
    private const string TurnBR = "turnSignal_BR";
    private const string TurnFL = "turnSignal_FL";
    private const string TurnFR = "turnSignal_FR";
    private const string StopBL = "stopSignal_BL";
    private const string StopBR = "stopSignal_BR";
    private const string BackL = "bDriveSignal_L";
    private const string BackR = "bDriveSignal_R";

    private static readonly string[] AllLampNames =
    {
        FaraFL, FaraFR,
        TurnBL, TurnBR, TurnFL, TurnFR,
        StopBL, StopBR,
        BackL, BackR
    };

    private enum BlinkMode
    {
        None,
        Left,
        Right,
        Hazard
    }

    private readonly Dictionary<string, LampNodeState> lampNodeCache =
        new Dictionary<string, LampNodeState>(System.StringComparer.OrdinalIgnoreCase);

    private BlinkMode blinkMode = BlinkMode.None;
    private bool headlightsOn;
    private bool hazardEnabled;
    private bool engineRunning;
    private Coroutine blinkCoroutine;
    private Coroutine turnTimeoutCoroutine;

    private void Awake()
    {
        CacheLampNodes();
        ForceAllOff();
    }

    private void OnDisable()
    {
        ForceAllOff();
    }

    private void Update()
    {
        if (!engineRunning)
        {
            return;
        }

        if (Input.GetKeyDown(headlightsKey))
        {
            SetHeadlights(!headlightsOn);
        }

        if (Input.GetKeyDown(hazardKey))
        {
            ToggleHazard();
        }

        if (Input.GetKeyDown(leftTurnKey))
        {
            StartTimedTurn(BlinkMode.Left);
        }

        if (Input.GetKeyDown(rightTurnKey))
        {
            StartTimedTurn(BlinkMode.Right);
        }
    }

    public void SetEngineRunning(bool running)
    {
        engineRunning = running;

        if (!engineRunning)
        {
            ForceAllOff();
        }
    }

    public void SetHeadlights(bool enabled)
    {
        headlightsOn = enabled && engineRunning;
        SetNodeLight(FaraFL, headlightsOn);
        SetNodeLight(FaraFR, headlightsOn);
    }

    public void SetBrakeSignals(bool enabled)
    {
        SetNodeLight(StopBL, enabled);
        SetNodeLight(StopBR, enabled);
    }

    public void SetReverseSignals(bool enabled)
    {
        SetNodeLight(BackL, enabled);
        SetNodeLight(BackR, enabled);
    }

    private void ToggleHazard()
    {
        if (!engineRunning)
        {
            return;
        }

        hazardEnabled = !hazardEnabled;

        if (hazardEnabled)
        {
            StopTurnTimeout();
            StartBlink(BlinkMode.Hazard);
        }
        else
        {
            StartBlink(BlinkMode.None);
        }
    }

    private void StartTimedTurn(BlinkMode direction)
    {
        if (!engineRunning || hazardEnabled)
        {
            return;
        }

        if (direction != BlinkMode.Left && direction != BlinkMode.Right)
        {
            return;
        }

        StartBlink(direction);
        StopTurnTimeout();
        turnTimeoutCoroutine = StartCoroutine(TurnSignalTimeout());
    }

    private IEnumerator TurnSignalTimeout()
    {
        yield return new WaitForSeconds(turnSignalDuration);
        StartBlink(BlinkMode.None);
        turnTimeoutCoroutine = null;
    }

    private void StopTurnTimeout()
    {
        if (turnTimeoutCoroutine == null)
        {
            return;
        }

        StopCoroutine(turnTimeoutCoroutine);
        turnTimeoutCoroutine = null;
    }

    private void StartBlink(BlinkMode mode)
    {
        if (!engineRunning)
        {
            mode = BlinkMode.None;
        }

        blinkMode = mode;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        SetAllTurnSignals(false);

        if (blinkMode != BlinkMode.None)
        {
            blinkCoroutine = StartCoroutine(BlinkRoutine());
        }
    }

    private IEnumerator BlinkRoutine()
    {
        bool isOn = false;

        while (blinkMode != BlinkMode.None)
        {
            isOn = !isOn;
            ApplyBlinkState(isOn);
            yield return new WaitForSeconds(blinkInterval);
        }

        ApplyBlinkState(false);
        blinkCoroutine = null;
    }

    private void ApplyBlinkState(bool enabled)
    {
        switch (blinkMode)
        {
            case BlinkMode.Left:
                SetNodeLight(TurnBL, enabled);
                SetNodeLight(TurnFL, enabled);
                SetNodeLight(TurnBR, false);
                SetNodeLight(TurnFR, false);
                break;

            case BlinkMode.Right:
                SetNodeLight(TurnBR, enabled);
                SetNodeLight(TurnFR, enabled);
                SetNodeLight(TurnBL, false);
                SetNodeLight(TurnFL, false);
                break;

            case BlinkMode.Hazard:
                SetNodeLight(TurnBL, enabled);
                SetNodeLight(TurnBR, enabled);
                SetNodeLight(TurnFL, enabled);
                SetNodeLight(TurnFR, enabled);
                break;

            default:
                SetAllTurnSignals(false);
                break;
        }
    }

    private void SetAllTurnSignals(bool enabled)
    {
        SetNodeLight(TurnBL, enabled);
        SetNodeLight(TurnBR, enabled);
        SetNodeLight(TurnFL, enabled);
        SetNodeLight(TurnFR, enabled);
    }

    private void SetNodeLight(string nodeName, bool enabled)
    {
        LampNodeState state = GetOrCreateLampNodeState(nodeName);
        if (state == null)
        {
            return;
        }

        if (state.light != null)
        {
            state.light.enabled = enabled;
        }

        ApplyLampMaterialState(state, enabled, GetHighlightColorForNode(nodeName));
    }

    private void ForceAllOff()
    {
        hazardEnabled = false;
        headlightsOn = false;
        blinkMode = BlinkMode.None;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (turnTimeoutCoroutine != null)
        {
            StopCoroutine(turnTimeoutCoroutine);
            turnTimeoutCoroutine = null;
        }

        for (int i = 0; i < AllLampNames.Length; i++)
        {
            SetNodeLight(AllLampNames[i], false);
        }
    }

    private void CacheLampNodes()
    {
        for (int i = 0; i < AllLampNames.Length; i++)
        {
            GetOrCreateLampNodeState(AllLampNames[i]);
        }
    }

    private LampNodeState GetOrCreateLampNodeState(string nodeName)
    {
        if (string.IsNullOrEmpty(nodeName))
        {
            return null;
        }

        if (lampNodeCache.TryGetValue(nodeName, out LampNodeState cachedState))
        {
            return cachedState;
        }

        Transform node = FindDeepChild(transform, nodeName);
        if (node == null)
        {
            return null;
        }

        Light nodeLight = node.GetComponent<Light>();
        Renderer[] renderers = CollectNodeRenderers(node);
        RendererColorState[] colorStates = new RendererColorState[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null)
            {
                continue;
            }

            Material material = renderer.material;
            colorStates[i] = new RendererColorState
            {
                hasBaseColor = material.HasProperty("_BaseColor"),
                hasColor = material.HasProperty("_Color"),
                hasEmission = material.HasProperty("_EmissionColor"),
                emissionKeywordEnabled = material.IsKeywordEnabled("_EMISSION"),
                baseColor = material.HasProperty("_BaseColor") ? material.GetColor("_BaseColor") : Color.white,
                color = material.HasProperty("_Color") ? material.GetColor("_Color") : Color.white,
                emissionColor = material.HasProperty("_EmissionColor") ? material.GetColor("_EmissionColor") : Color.black
            };
        }

        LampNodeState state = new LampNodeState
        {
            light = nodeLight,
            renderers = renderers,
            rendererStates = colorStates,
            isHighlighted = false
        };

        lampNodeCache[nodeName] = state;
        return state;
    }

    private Renderer[] CollectNodeRenderers(Transform node)
    {
        List<Renderer> renderers = new List<Renderer>();

        Collider[] colliders = node.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            if (collider == null)
            {
                continue;
            }

            Renderer renderer = collider.GetComponent<Renderer>();
            if (renderer != null && !renderers.Contains(renderer))
            {
                renderers.Add(renderer);
            }
        }

        if (renderers.Count == 0)
        {
            Renderer fallbackRenderer = node.GetComponent<Renderer>();
            if (fallbackRenderer != null)
            {
                renderers.Add(fallbackRenderer);
            }
        }

        return renderers.ToArray();
    }

    private void ApplyLampMaterialState(LampNodeState state, bool isOn, Color highlightColor)
    {
        if (state == null || state.renderers == null || state.rendererStates == null)
        {
            return;
        }

        if (state.isHighlighted == isOn)
        {
            return;
        }

        float t = Mathf.Clamp01(lampBaseBrightenStrength);
        Color targetColor = new Color(highlightColor.r, highlightColor.g, highlightColor.b, 1f);

        for (int i = 0; i < state.renderers.Length; i++)
        {
            Renderer renderer = state.renderers[i];
            if (renderer == null)
            {
                continue;
            }

            Material material = renderer.material;
            RendererColorState rendererState = state.rendererStates[i];

            if (isOn)
            {
                if (rendererState.hasBaseColor)
                {
                    material.SetColor("_BaseColor", Color.Lerp(rendererState.baseColor, targetColor, t));
                }

                if (rendererState.hasColor)
                {
                    material.SetColor("_Color", Color.Lerp(rendererState.color, targetColor, t));
                }

                if (rendererState.hasEmission)
                {
                    material.EnableKeyword("_EMISSION");
                    Color targetEmission = rendererState.emissionColor + targetColor * lampEmissionBoost;
                    material.SetColor("_EmissionColor", targetEmission);
                }
            }
            else
            {
                if (rendererState.hasBaseColor)
                {
                    material.SetColor("_BaseColor", rendererState.baseColor);
                }

                if (rendererState.hasColor)
                {
                    material.SetColor("_Color", rendererState.color);
                }

                if (rendererState.hasEmission)
                {
                    material.SetColor("_EmissionColor", rendererState.emissionColor);
                    if (!rendererState.emissionKeywordEnabled)
                    {
                        material.DisableKeyword("_EMISSION");
                    }
                }
            }
        }

        state.isHighlighted = isOn;
    }

    private Color GetHighlightColorForNode(string nodeName)
    {
        switch (nodeName)
        {
            case StopBL:
            case StopBR:
                return redLampHighlightColor;

            case TurnBL:
            case TurnBR:
            case TurnFL:
            case TurnFR:
                return yellowLampHighlightColor;

            case FaraFL:
            case FaraFR:
            case BackL:
            case BackR:
            default:
                return whiteLampHighlightColor;
        }
    }

    private Transform FindDeepChild(Transform parent, string objectName)
    {
        if (parent == null)
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

    private class LampNodeState
    {
        public Light light;
        public Renderer[] renderers;
        public RendererColorState[] rendererStates;
        public bool isHighlighted;
    }

    private struct RendererColorState
    {
        public bool hasBaseColor;
        public bool hasColor;
        public bool hasEmission;
        public bool emissionKeywordEnabled;
        public Color baseColor;
        public Color color;
        public Color emissionColor;
    }
}
