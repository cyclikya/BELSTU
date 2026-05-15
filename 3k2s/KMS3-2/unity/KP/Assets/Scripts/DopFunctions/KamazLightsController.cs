using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Управляет фарами, стоп-сигналами и поворотниками.
// Все лампы ищутся внутри объекта fary из KamazContext.
public class KamazLightsController : MonoBehaviour
{
    private class LampNode
    {
        public Light light;
        public Renderer[] renderers;
        public Material[] savedMaterials;
        public Color activeColor;

        public void CacheMaterials()
        {
            if (renderers == null)
            {
                return;
            }

            savedMaterials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    savedMaterials[i] = new Material(renderers[i].material);
                }
            }
        }

        public void SetState(bool enabled)
        {
            if (light != null)
            {
                light.enabled = enabled;
            }

            if (renderers == null || savedMaterials == null)
            {
                return;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null || savedMaterials[i] == null)
                {
                    continue;
                }

                if (enabled)
                {
                    renderers[i].material.color = activeColor;
                }
                else
                {
                    renderers[i].material = savedMaterials[i];
                }
            }
        }
    }

    private enum BlinkMode
    {
        None,
        Left,
        Right,
        Hazard
    }

    [Header("References")]
    [SerializeField] private KamazContext kamazContext;

    [Header("Input")]
    [SerializeField] private KeyCode headlightsKey = KeyCode.F;
    [SerializeField] private KeyCode hazardKey = KeyCode.X;
    [SerializeField] private KeyCode leftTurnKey = KeyCode.Q;
    [SerializeField] private KeyCode rightTurnKey = KeyCode.E;

    [Header("Blink")]
    [SerializeField] private float blinkInterval = 0.5f;
    [SerializeField] private float turnSignalDuration = 10f;
    [SerializeField] private float inputBlockAfterEnterCabinSeconds = 0.25f;

    private const string BackL = "bDriveSignal_L";
    private const string BackR = "bDriveSignal_R";
    private const string FaraFL = "fara_FL";
    private const string FaraFR = "fara_FR";
    private const string StopBL = "stopSignal_BL";
    private const string StopBR = "stopSignal_BR";
    private const string TurnBL = "turnSignal_BL";
    private const string TurnBR = "turnSignal_BR";
    private const string TurnFL = "turnSignal_FL";
    private const string TurnFR = "turnSignal_FR";

    private readonly Dictionary<string, LampNode> lamps = new Dictionary<string, LampNode>();

    private BlinkMode blinkMode;
    private bool engineRunning;
    private bool isInCabin;
    private bool headlightsOn;
    private bool hazardEnabled;
    private float inputBlockedUntilTime;
    private Coroutine blinkCoroutine;
    private Coroutine turnTimeoutCoroutine;

    private void Awake()
    {
        if (kamazContext == null)
        {
            kamazContext = GetComponent<KamazContext>();
        }

        CacheLampNodes();
        ForceAllOff();
    }

    private void OnDisable()
    {
        ForceAllOff();
    }

    private void Update()
    {
        if (!engineRunning || !isInCabin || Time.unscaledTime < inputBlockedUntilTime)
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

    public void SetEngineRunning(bool value)
    {
        engineRunning = value;
        if (!engineRunning)
        {
            ForceAllOff();
            if (kamazContext != null && kamazContext.AudioController != null)
            {
                kamazContext.AudioController.SetTurnSignalLoop(false);
            }
        }
    }

    public void SetInCabin(bool value)
    {
        if (!isInCabin && value)
        {
            BlockInputForSeconds(inputBlockAfterEnterCabinSeconds);
        }

        isInCabin = value;
    }

    public void BlockInputForSeconds(float seconds)
    {
        inputBlockedUntilTime = Mathf.Max(inputBlockedUntilTime, Time.unscaledTime + seconds);
    }

    public void SetHeadlights(bool enabled)
    {
        headlightsOn = enabled && engineRunning;
        SetLampState(FaraFL, headlightsOn);
        SetLampState(FaraFR, headlightsOn);
        if (kamazContext != null && kamazContext.AudioController != null)
        {
            kamazContext.AudioController.PlayHeadlightSwitch();
        }
    }

    public void SetBrakeSignals(bool enabled)
    {
        SetLampState(StopBL, enabled);
        SetLampState(StopBR, enabled);
    }

    public void SetReverseSignals(bool enabled)
    {
        SetLampState(BackL, enabled);
        SetLampState(BackR, enabled);
    }

    private void ToggleHazard()
    {
        if (!engineRunning || !isInCabin)
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

    private void StartTimedTurn(BlinkMode mode)
    {
        if (!engineRunning || !isInCabin || hazardEnabled)
        {
            return;
        }

        StartBlink(mode);
        StopTurnTimeout();
        turnTimeoutCoroutine = StartCoroutine(TurnTimeoutRoutine());
    }

    private IEnumerator TurnTimeoutRoutine()
    {
        yield return new WaitForSeconds(turnSignalDuration);
        StartBlink(BlinkMode.None);
        turnTimeoutCoroutine = null;
    }

    private void StopTurnTimeout()
    {
        if (turnTimeoutCoroutine != null)
        {
            StopCoroutine(turnTimeoutCoroutine);
            turnTimeoutCoroutine = null;
        }
    }

    private void StartBlink(BlinkMode mode)
    {
        blinkMode = mode;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }

        SetAllTurnSignals(false);

        if (blinkMode != BlinkMode.None)
        {
            if (kamazContext != null && kamazContext.AudioController != null)
            {
                kamazContext.AudioController.SetTurnSignalLoop(true);
            }
            blinkCoroutine = StartCoroutine(BlinkRoutine());
            return;
        }

        if (kamazContext != null && kamazContext.AudioController != null)
        {
            kamazContext.AudioController.SetTurnSignalLoop(false);
        }
    }

    private IEnumerator BlinkRoutine()
    {
        bool enabled = false;
        while (blinkMode != BlinkMode.None)
        {
            enabled = !enabled;
            ApplyBlinkState(enabled);
            yield return new WaitForSeconds(blinkInterval);
        }

        ApplyBlinkState(false);
        blinkCoroutine = null;
    }

    private void ApplyBlinkState(bool enabled)
    {
        if (blinkMode == BlinkMode.Left)
        {
            SetLampState(TurnBL, enabled);
            SetLampState(TurnFL, enabled);
            SetLampState(TurnBR, false);
            SetLampState(TurnFR, false);
            return;
        }

        if (blinkMode == BlinkMode.Right)
        {
            SetLampState(TurnBR, enabled);
            SetLampState(TurnFR, enabled);
            SetLampState(TurnBL, false);
            SetLampState(TurnFL, false);
            return;
        }

        if (blinkMode == BlinkMode.Hazard)
        {
            SetLampState(TurnBL, enabled);
            SetLampState(TurnBR, enabled);
            SetLampState(TurnFL, enabled);
            SetLampState(TurnFR, enabled);
            return;
        }

        SetAllTurnSignals(false);
        if (kamazContext != null && kamazContext.AudioController != null)
        {
            kamazContext.AudioController.SetTurnSignalLoop(false);
        }
    }

    private void SetAllTurnSignals(bool enabled)
    {
        SetLampState(TurnBL, enabled);
        SetLampState(TurnBR, enabled);
        SetLampState(TurnFL, enabled);
        SetLampState(TurnFR, enabled);
    }

    private void ForceAllOff()
    {
        headlightsOn = false;
        hazardEnabled = false;
        blinkMode = BlinkMode.None;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        StopTurnTimeout();
        SetLampState(FaraFL, false);
        SetLampState(FaraFR, false);
        SetLampState(StopBL, false);
        SetLampState(StopBR, false);
        SetLampState(BackL, false);
        SetLampState(BackR, false);
        SetAllTurnSignals(false);
        if (kamazContext != null && kamazContext.AudioController != null)
        {
            kamazContext.AudioController.SetTurnSignalLoop(false);
        }
    }

    private void CacheLampNodes()
    {
        lamps.Clear();

        Transform faryRoot = kamazContext != null ? kamazContext.Fary : null;
        if (faryRoot == null)
        {
            return;
        }

        CacheLamp(faryRoot, BackL, Color.white);
        CacheLamp(faryRoot, BackR, Color.white);
        CacheLamp(faryRoot, FaraFL, Color.white);
        CacheLamp(faryRoot, FaraFR, Color.white);
        CacheLamp(faryRoot, StopBL, Color.red);
        CacheLamp(faryRoot, StopBR, Color.red);
        CacheLamp(faryRoot, TurnBL, Color.yellow);
        CacheLamp(faryRoot, TurnBR, Color.yellow);
        CacheLamp(faryRoot, TurnFL, Color.yellow);
        CacheLamp(faryRoot, TurnFR, Color.yellow);
    }

    private void CacheLamp(Transform root, string nodeName, Color activeColor)
    {
        Transform lampTransform = FindChildByName(root, nodeName);
        if (lampTransform == null)
        {
            return;
        }

        LampNode lamp = new LampNode();
        lamp.light = lampTransform.GetComponent<Light>();
        lamp.renderers = lampTransform.GetComponentsInChildren<Renderer>(true);
        lamp.activeColor = activeColor;
        lamp.CacheMaterials();
        lamps[nodeName] = lamp;
    }

    private void SetLampState(string nodeName, bool enabled)
    {
        if (lamps.TryGetValue(nodeName, out LampNode lamp))
        {
            lamp.SetState(enabled);
        }
    }

    private Transform FindChildByName(Transform root, string objectName)
    {
        if (root == null)
        {
            return null;
        }

        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name == objectName)
            {
                return children[i];
            }
        }

        return null;
    }
}
