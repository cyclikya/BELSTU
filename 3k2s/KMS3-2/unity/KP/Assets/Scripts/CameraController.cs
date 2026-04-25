using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Главный скрипт игрока: переключает режимы ходьбы и вождения,
// открывает двери, запускает двигатель и рисует подсказки.
public class CameraController : MonoBehaviour
{
    public enum ControlMode
    {
        FreeMovement,
        Driving
    }

    [Header("References")]
    [SerializeField] private KamazContext kamazContext;
    [SerializeField] private KamazLightsController kamazLightsController;
    [SerializeField] private KamazCabinMechanismsController kamazCabinMechanismsController;

    [Header("Mode Objects")]
    [SerializeField] private FreeMovementMode freeMovementMode = new FreeMovementMode();
    [SerializeField] private DrivingMode drivingMode = new DrivingMode();

    [Header("Keys")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode engineToggleKey = KeyCode.Tab;
    [SerializeField] private KeyCode toggleUiCursorKey = KeyCode.Escape;

    [Header("Door")]
    [SerializeField] private float switchModeDistance = 20f;
    [SerializeField] private float exitToFreeModeDelay = 0.35f;
    [SerializeField] private float characterControllerEnableDelayAfterExit = 0.2f;
    [SerializeField] private float doorFreezeDuration = 0.85f;

    [Header("View")]
    [SerializeField] private bool showCrosshairDot = true;
    [SerializeField] private int crosshairSize = 3;
    [SerializeField] private Color crosshairColor = Color.white;
    [SerializeField] private bool allowUiCursorToggle = true;
    [SerializeField] private bool startWithUnlockedCursor;
    [SerializeField] private float drivingZoomMultiplier = 2f;
    [SerializeField] private float drivingZoomLerpSpeed = 12f;

    [Header("Hints")]
    [SerializeField] private bool showInteractionHint = true;
    [SerializeField] private Color interactionHintColor = Color.white;
    [SerializeField] private int interactionHintFontSize = 18;

    [Header("Highlight")]
    [SerializeField] private bool showInteractableHighlight = true;

    [Header("Engine Effects")]
    [SerializeField] private float engineStartShakeDuration = 1.5f;
    [SerializeField] private float engineStartShakeAmplitude = 0.01f;
    [SerializeField] private float startupSweepUpDuration = 0.28f;
    [SerializeField] private float startupSweepHoldDuration = 0.1f;
    [SerializeField] private float startupSweepDownDuration = 0.9f;
    [SerializeField] private float startupSpeedometerPeak = 20f;
    [SerializeField] private float startupTachometerPeak = 2800f;

    private ControlMode currentMode = ControlMode.FreeMovement;
    private CharacterController characterController;
    private Camera playerCamera;
    private Texture2D crosshairTexture;
    private Collider[] playerColliders;
    private Rigidbody kamazRigidbody;
    private bool kamazWasKinematic;
    private float defaultFieldOfView;

    private bool waitingControllerReenable;
    private bool isExitingDrivingMode;
    private bool engineRunning;
    private bool uiCursorActive;

    private Coroutine doorFreezeCoroutine;
    private Coroutine kabinaShakeCoroutine;
    private Coroutine startupNeedleSweepCoroutine;

    private readonly Dictionary<Renderer, Material> highlightedRenderers = new Dictionary<Renderer, Material>();
    private readonly Color highlightOverlayColor = new Color(230f / 255f, 230f / 255f, 230f / 255f, 1f);
    private const float highlightStrength = 80f / 255f;
    private Transform highlightedRoot;
    private string interactionHintText = string.Empty;
    private string temporaryHintText = string.Empty;
    private float temporaryHintUntilTime;
    private readonly List<Transform> shakeTargets = new List<Transform>();
    private readonly List<Vector3> shakeStartPositions = new List<Vector3>();

    public bool IsDrivingMode => currentMode == ControlMode.Driving;
    public bool IsEngineRunning => engineRunning;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        playerColliders = GetComponentsInChildren<Collider>(true);

        if (kamazContext != null)
        {
            if (kamazLightsController == null) kamazLightsController = kamazContext.LightsController;
            if (kamazCabinMechanismsController == null) kamazCabinMechanismsController = kamazContext.CabinMechanismsController;
            kamazRigidbody = kamazContext.KamazRigidbody;
        }

        if (kamazRigidbody != null)
        {
            kamazWasKinematic = kamazRigidbody.isKinematic;
        }

        if (playerCamera != null)
        {
            defaultFieldOfView = playerCamera.fieldOfView;
        }

        freeMovementMode.Initialize(characterController, playerCamera, transform);
        drivingMode.Initialize(characterController, transform, kamazContext);

        ConfigureNeedles();
        SyncLightsControllerState();
        CreateCrosshairTexture();
        SetUiCursorState(startWithUnlockedCursor);
        ApplyCurrentMode();
    }

    private void Update()
    {
        HandleUiCursorToggle();
        if (uiCursorActive)
        {
            return;
        }

        UpdateInteractionHighlight();
        UpdateInteractionHintText();

        if (currentMode == ControlMode.FreeMovement)
        {
            if (waitingControllerReenable)
            {
                freeMovementMode.TickLookOnly();
                return;
            }

            freeMovementMode.Tick();
            HandleFreeModeInteraction();
        }
        else
        {
            freeMovementMode.TickLookOnly();
            drivingMode.Tick();
            HandleDrivingModeInteraction();
        }

        UpdateDrivingZoom();
    }

    private void HandleFreeModeInteraction()
    {
        if (!Input.GetKeyDown(interactKey))
        {
            return;
        }

        if (!freeMovementMode.TryGetInteraction(kamazContext, out FreeMovementMode.InteractionResult interaction))
        {
            return;
        }

        if (interaction.Type == FreeMovementMode.InteractionType.Door)
        {
            ToggleDoor(interaction.DoorAnimator);
            return;
        }

        if (interaction.Type == FreeMovementMode.InteractionType.Steering)
        {
            EnterDrivingMode();
        }
    }

    private void HandleDrivingModeInteraction()
    {
        if (Input.GetKeyDown(engineToggleKey))
        {
            SetEngineRunning(!engineRunning);
        }

        if (isExitingDrivingMode || !Input.GetKeyDown(interactKey))
        {
            return;
        }

        if (!freeMovementMode.TryGetInteraction(kamazContext, out FreeMovementMode.InteractionResult interaction))
        {
            return;
        }

        if (interaction.Type != FreeMovementMode.InteractionType.Door || interaction.DoorRoot == null)
        {
            return;
        }

        float distanceToDoor = Vector3.Distance(transform.position, interaction.DoorRoot.position);
        if (distanceToDoor > switchModeDistance)
        {
            return;
        }

        if (kamazLightsController != null)
        {
            kamazLightsController.BlockInputForSeconds(exitToFreeModeDelay + 0.25f);
        }

        StartCoroutine(ExitDrivingMode(interaction.DoorAnimator));
    }

    private IEnumerator ExitDrivingMode(Animator doorAnimator)
    {
        isExitingDrivingMode = true;
        IgnoreKamazCollisions(true);
        FreezeKamaz(true);

        if (doorAnimator != null)
        {
            doorAnimator.SetBool("isOpen", true);
        }

        yield return new WaitForSeconds(exitToFreeModeDelay);

        waitingControllerReenable = true;
        SetMode(ControlMode.FreeMovement, true);

        yield return new WaitForSeconds(characterControllerEnableDelayAfterExit);
        drivingMode.SetCharacterControllerEnabled(true);

        yield return new WaitForSeconds(0.8f);
        IgnoreKamazCollisions(false);
        FreezeKamaz(false);

        waitingControllerReenable = false;
        isExitingDrivingMode = false;
    }

    private void EnterDrivingMode()
    {
        if (kamazContext == null)
        {
            return;
        }

        if (kamazContext.DoorLAnimator != null)
        {
            kamazContext.DoorLAnimator.SetBool("isOpen", false);
        }

        if (kamazContext.DoorRAnimator != null)
        {
            kamazContext.DoorRAnimator.SetBool("isOpen", false);
        }

        SetMode(ControlMode.Driving);
    }

    private void SetMode(ControlMode mode, bool keepControllerDisabledOnFree = false)
    {
        currentMode = mode;
        ApplyCurrentMode(keepControllerDisabledOnFree);
        SyncLightsControllerState();
    }

    private void ApplyCurrentMode(bool keepControllerDisabledOnFree = false)
    {
        if (currentMode == ControlMode.FreeMovement)
        {
            drivingMode.ExitMode(keepControllerDisabledOnFree);
            if (!keepControllerDisabledOnFree)
            {
                freeMovementMode.EnterMode();
            }
        }
        else
        {
            freeMovementMode.ExitMode();
            drivingMode.EnterMode();
        }
    }

    private void ToggleDoor(Animator doorAnimator)
    {
        if (doorAnimator == null)
        {
            return;
        }

        StartDoorFreeze();
        bool isOpen = doorAnimator.GetBool("isOpen");
        doorAnimator.SetBool("isOpen", !isOpen);
    }

    private void StartDoorFreeze()
    {
        if (doorFreezeCoroutine != null)
        {
            StopCoroutine(doorFreezeCoroutine);
            FreezeKamaz(false);
        }

        doorFreezeCoroutine = StartCoroutine(DoorFreezeRoutine());
    }

    private IEnumerator DoorFreezeRoutine()
    {
        FreezeKamaz(true);
        yield return new WaitForSeconds(doorFreezeDuration);
        FreezeKamaz(false);
        doorFreezeCoroutine = null;
    }

    private void FreezeKamaz(bool value)
    {
        if (kamazRigidbody == null)
        {
            return;
        }

        if (value)
        {
            kamazRigidbody.linearVelocity = Vector3.zero;
            kamazRigidbody.angularVelocity = Vector3.zero;
            kamazRigidbody.isKinematic = true;
            return;
        }

        kamazRigidbody.isKinematic = kamazWasKinematic;
        kamazRigidbody.linearVelocity = Vector3.zero;
        kamazRigidbody.angularVelocity = Vector3.zero;
    }

    private void IgnoreKamazCollisions(bool ignore)
    {
        if (playerColliders == null || kamazContext == null || kamazContext.AllKamazColliders == null)
        {
            return;
        }

        for (int i = 0; i < playerColliders.Length; i++)
        {
            Collider playerCollider = playerColliders[i];
            if (playerCollider == null)
            {
                continue;
            }

            for (int j = 0; j < kamazContext.AllKamazColliders.Length; j++)
            {
                Collider kamazCollider = kamazContext.AllKamazColliders[j];
                if (kamazCollider != null)
                {
                    Physics.IgnoreCollision(playerCollider, kamazCollider, ignore);
                }
            }
        }
    }

    public void SetEngineRunningState(bool value)
    {
        SetEngineRunning(value);
    }

    private void SetEngineRunning(bool value)
    {
        if (engineRunning == value)
        {
            return;
        }

        if (value && kamazCabinMechanismsController != null && kamazCabinMechanismsController.IsBodyRaised)
        {
            ShowTemporaryHint("Сначала опустите кузов (B), затем запускайте двигатель.");
            return;
        }

        engineRunning = value;

        if (kamazContext != null && kamazContext.KeyAnimator != null)
        {
            kamazContext.KeyAnimator.SetBool("turn", value);
        }

        if (engineRunning)
        {
            StartCabinShake();
            StartStartupNeedleSweep();
        }
        else
        {
            StopCabinShake();
            StopStartupNeedleSweep(true);
        }

        SyncLightsControllerState();
    }

    private void SyncLightsControllerState()
    {
        if (kamazLightsController == null)
        {
            return;
        }

        kamazLightsController.SetEngineRunning(engineRunning);
        kamazLightsController.SetInCabin(currentMode == ControlMode.Driving);
    }

    private void ConfigureNeedles()
    {
        if (kamazContext == null)
        {
            return;
        }

        if (kamazContext.SpidometerNeedle != null)
        {
            kamazContext.SpidometerNeedle.Configure(0f, 90f, 0.3f, 264f);
            kamazContext.SpidometerNeedle.SetValueImmediate(0f);
        }

        if (kamazContext.TachometerNeedle != null)
        {
            kamazContext.TachometerNeedle.Configure(0f, 3000f, 2f, 240f);
            kamazContext.TachometerNeedle.SetValueImmediate(0f);
        }
    }

    private void StartStartupNeedleSweep()
    {
        if (startupNeedleSweepCoroutine != null)
        {
            StopCoroutine(startupNeedleSweepCoroutine);
        }

        startupNeedleSweepCoroutine = StartCoroutine(StartupNeedleSweepRoutine());
    }

    private IEnumerator StartupNeedleSweepRoutine()
    {
        float elapsed = 0f;
        while (elapsed < startupSweepUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / startupSweepUpDuration);
            SetNeedlesImmediate(Mathf.Lerp(0f, startupSpeedometerPeak, t), Mathf.Lerp(0f, startupTachometerPeak, t));
            yield return null;
        }

        if (startupSweepHoldDuration > 0f)
        {
            yield return new WaitForSeconds(startupSweepHoldDuration);
        }

        elapsed = 0f;
        while (elapsed < startupSweepDownDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / startupSweepDownDuration);
            SetNeedlesImmediate(Mathf.Lerp(startupSpeedometerPeak, 0f, t), Mathf.Lerp(startupTachometerPeak, 0f, t));
            yield return null;
        }

        SetNeedlesImmediate(0f, 0f);
        startupNeedleSweepCoroutine = null;
    }

    private void StopStartupNeedleSweep(bool resetToZero)
    {
        if (startupNeedleSweepCoroutine != null)
        {
            StopCoroutine(startupNeedleSweepCoroutine);
            startupNeedleSweepCoroutine = null;
        }

        if (resetToZero)
        {
            SetNeedlesImmediate(0f, 0f);
        }
    }

    private void SetNeedlesImmediate(float speedValue, float tachValue)
    {
        if (kamazContext == null)
        {
            return;
        }

        if (kamazContext.SpidometerNeedle != null)
        {
            kamazContext.SpidometerNeedle.SetValueImmediate(speedValue);
        }

        if (kamazContext.TachometerNeedle != null)
        {
            kamazContext.TachometerNeedle.SetValueImmediate(tachValue);
        }
    }

    private void StartCabinShake()
    {
        StopCabinShake();
        if (kamazContext == null || kamazContext.Kabina == null)
        {
            return;
        }

        shakeTargets.Clear();
        shakeStartPositions.Clear();
        shakeTargets.Add(kamazContext.Kabina);
        shakeStartPositions.Add(kamazContext.Kabina.localPosition);
        kabinaShakeCoroutine = StartCoroutine(CabinShakeRoutine());
    }

    private IEnumerator CabinShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < engineStartShakeDuration)
        {
            elapsed += Time.deltaTime;

            for (int i = 0; i < shakeTargets.Count; i++)
            {
                Vector3 offset = Random.insideUnitSphere * engineStartShakeAmplitude;
                offset.y *= 0.35f;
                shakeTargets[i].localPosition = shakeStartPositions[i] + offset;
            }

            yield return null;
        }

        StopCabinShake();
    }

    private void StopCabinShake()
    {
        if (kabinaShakeCoroutine != null)
        {
            StopCoroutine(kabinaShakeCoroutine);
            kabinaShakeCoroutine = null;
        }

        for (int i = 0; i < shakeTargets.Count; i++)
        {
            if (shakeTargets[i] != null)
            {
                shakeTargets[i].localPosition = shakeStartPositions[i];
            }
        }

        shakeTargets.Clear();
        shakeStartPositions.Clear();
    }

    private void UpdateDrivingZoom()
    {
        if (playerCamera == null)
        {
            return;
        }

        float normalFov = defaultFieldOfView > 0f ? defaultFieldOfView : playerCamera.fieldOfView;
        float targetFov = currentMode == ControlMode.Driving && Input.GetMouseButton(1)
            ? normalFov / Mathf.Max(1f, drivingZoomMultiplier)
            : normalFov;

        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFov, drivingZoomLerpSpeed * Time.deltaTime);
    }

    private void UpdateInteractionHighlight()
    {
        if (!showInteractableHighlight || kamazContext == null)
        {
            ClearHighlight();
            return;
        }

        if (!freeMovementMode.TryGetInteraction(kamazContext, out FreeMovementMode.InteractionResult interaction))
        {
            ClearHighlight();
            return;
        }

        Transform target = null;
        if (interaction.Type == FreeMovementMode.InteractionType.Door)
        {
            target = interaction.DoorRoot;
        }
        else if (interaction.Type == FreeMovementMode.InteractionType.Steering && currentMode == ControlMode.FreeMovement)
        {
            target = kamazContext.Ryle;
        }

        if (target == null)
        {
            ClearHighlight();
            return;
        }

        if (highlightedRoot == target)
        {
            return;
        }

        ClearHighlight();
        highlightedRoot = target;
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null || highlightedRenderers.ContainsKey(renderer))
            {
                continue;
            }

            Material material = renderer.material;
            highlightedRenderers[renderer] = new Material(material);
            material.color = Color.Lerp(material.color, highlightOverlayColor, highlightStrength);
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
        highlightedRoot = null;
    }

    private void UpdateInteractionHintText()
    {
        interactionHintText = string.Empty;
        if (!showInteractionHint || kamazContext == null)
        {
            return;
        }

        if (!freeMovementMode.TryGetInteraction(kamazContext, out FreeMovementMode.InteractionResult interaction))
        {
            return;
        }

        if (interaction.Type == FreeMovementMode.InteractionType.Door)
        {
            if (currentMode == ControlMode.FreeMovement)
            {
                bool isOpen = interaction.DoorAnimator != null && interaction.DoorAnimator.GetBool("isOpen");
                interactionHintText = isOpen ? "Нажмите E чтобы закрыть дверь" : "Нажмите E чтобы открыть дверь";
            }
            else
            {
                interactionHintText = "Нажмите E чтобы выйти из кабины";
            }

            return;
        }

        if (interaction.Type == FreeMovementMode.InteractionType.Steering)
        {
            interactionHintText = currentMode == ControlMode.FreeMovement
                ? "Нажмите E чтобы сесть за руль"
                : engineRunning ? "Нажмите Tab чтобы заглушить машину" : "Нажмите Tab чтобы завести машину";
        }
    }

    private void OnGUI()
    {
        if (uiCursorActive)
        {
            return;
        }

        if (showCrosshairDot && crosshairTexture != null)
        {
            float x = (Screen.width - crosshairSize) * 0.5f;
            float y = (Screen.height - crosshairSize) * 0.5f;
            Color oldColor = GUI.color;
            GUI.color = crosshairColor;
            GUI.DrawTexture(new Rect(x, y, crosshairSize, crosshairSize), crosshairTexture);
            GUI.color = oldColor;
        }

        DrawInteractionHint();
    }

    private void DrawInteractionHint()
    {
        string text = GetActiveHintText();
        if (!showInteractionHint || string.IsNullOrEmpty(text))
        {
            return;
        }

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = interactionHintFontSize;
        style.normal.textColor = interactionHintColor;

        float width = Mathf.Min(Screen.width - 40f, 900f);
        float x = (Screen.width - width) * 0.5f;
        float y = Screen.height - 52f;
        GUI.Label(new Rect(x, y, width, 28f), text, style);
    }

    private string GetActiveHintText()
    {
        if (!string.IsNullOrEmpty(temporaryHintText) && Time.unscaledTime < temporaryHintUntilTime)
        {
            return temporaryHintText;
        }

        return interactionHintText;
    }

    private void ShowTemporaryHint(string text, float duration = 2.5f)
    {
        temporaryHintText = text;
        temporaryHintUntilTime = Time.unscaledTime + duration;
    }

    private void HandleUiCursorToggle()
    {
        if (allowUiCursorToggle && Input.GetKeyDown(toggleUiCursorKey))
        {
            SetUiCursorState(!uiCursorActive);
        }
    }

    private void SetUiCursorState(bool enabled)
    {
        uiCursorActive = enabled;
        Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enabled;
    }

    private void CreateCrosshairTexture()
    {
        crosshairTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        crosshairTexture.SetPixel(0, 0, Color.white);
        crosshairTexture.Apply();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            SetUiCursorState(uiCursorActive);
        }
    }

    private void OnDisable()
    {
        SetEngineRunning(false);
        StopCabinShake();
        StopStartupNeedleSweep(true);
        ClearHighlight();
        IgnoreKamazCollisions(false);
        FreezeKamaz(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}


