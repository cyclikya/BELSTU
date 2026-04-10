using System.Collections;
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

    private enum BlinkMode
    {
        None,
        Left,
        Right,
        Hazard
    }

    private BlinkMode blinkMode = BlinkMode.None;
    private bool headlightsOn;
    private bool hazardEnabled;
    private Coroutine blinkCoroutine;
    private Coroutine turnTimeoutCoroutine;

    private void Awake()
    {
        SetHeadlights(false);
        SetAllTurnSignals(false);
    }

    private void OnDisable()
    {
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

        SetAllTurnSignals(false);
    }

    private void Update()
    {
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

    public void SetHeadlights(bool enabled)
    {
        headlightsOn = enabled;
        SetNodeLight(FaraFL, enabled);
        SetNodeLight(FaraFR, enabled);
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
        if (hazardEnabled)
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
        Transform node = FindDeepChild(transform, nodeName);
        if (node == null)
        {
            return;
        }

        Light light = node.GetComponent<Light>();
        if (light != null)
        {
            light.enabled = enabled;
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
}
