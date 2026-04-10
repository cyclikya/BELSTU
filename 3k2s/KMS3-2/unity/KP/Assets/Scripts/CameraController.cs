using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum ControlMode
    {
        FreeMovement,
        Driving
    }

    private ControlMode currentMode = ControlMode.FreeMovement;
    private KeyCode interactKey = KeyCode.E;

    private FreeMovementMode freeMovementMode = new FreeMovementMode();

    private DrivingMode drivingMode = new DrivingMode();

    private string doorOpenBoolName = "isOpen";
    private string keyTurnBoolName = "turn";
    private float switchModeDistance = 20f;
    private float engineStartShakeDuration = 1.5f;
    private float engineStartShakeAmplitude = 0.01f;
    private float drivingZoomMultiplier = 2f;
    private float drivingZoomLerpSpeed = 12f;
    private float startupSweepUpDuration = 0.28f;
    private float startupSweepHoldDuration = 0.1f;
    private float startupSweepDownDuration = 0.9f;
    private float startupSpeedometerPeak = 20f;
    private float startupTachometerPeak = 7.6f;

    private float exitToFreeModeDelay = 0.35f;
    private float characterControllerEnableDelayAfterExit = 0.2f;
    private float ignoreKamazCollisionAfterExit = 0.8f;

    private bool showCrosshairDot = true;
    private int crosshairSize = 3;
    private Color crosshairColor = Color.white;
    private KamazContext kamazContext;

    private bool showInteractableHighlight = true;
    private Color interactableHighlightColor = new Color(230f / 255f, 230f / 255f, 230f / 255f, 45f / 255f);

    public bool IsDrivingMode => currentMode == ControlMode.Driving;

    private Camera playerCamera;
    private Texture2D crosshairTexture;
    private bool isExitingDrivingMode;
    private bool waitingControllerReenable;
    private Collider[] playerColliders;
    private Coroutine kabinaShakeCoroutine;
    private Coroutine startupNeedleSweepCoroutine;
    private readonly List<Transform> kabinaShakeTargets = new List<Transform>();
    private readonly List<Vector3> kabinaShakeOriginalLocalPositions = new List<Vector3>();
    private GaugeNeedle speedometerNeedle;
    private GaugeNeedle tachometerNeedle;
    private bool speedometerNeedleConfigured;
    private bool tachometerNeedleConfigured;

    private Transform highlightedRoot;
    private readonly Dictionary<Renderer, RendererColorState> highlightedRenderers = new Dictionary<Renderer, RendererColorState>();

    private KamazContext Kamaz => kamazContext;
    private float defaultFieldOfView;

    private void Start()
    {
        currentMode = ControlMode.FreeMovement;

        CharacterController controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            defaultFieldOfView = playerCamera.fieldOfView;
        }

        ResolveKamazContext();

        freeMovementMode.Initialize(controller, playerCamera, transform);
        drivingMode.Initialize(controller, transform, Kamaz);

        Kamaz?.AutoResolve();
        TryInitializeNeedles();

        ApplyCurrentMode();
        CreateCrosshairTexture();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Kamaz == null)
        {
            ResolveKamazContext();
            drivingMode.SetContext(Kamaz);
            Kamaz?.AutoResolve();
        }

        TryInitializeNeedles();

        UpdateInteractionHighlight();

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

        if (!freeMovementMode.TryGetInteraction(Kamaz, out FreeMovementMode.InteractionResult interaction))
        {
            return;
        }

        switch (interaction.Type)
        {
            case FreeMovementMode.InteractionType.Door:
                HandleDoorInteraction(interaction.DoorAnimator);
                return;

            case FreeMovementMode.InteractionType.Steering:
                HandleSteeringInteraction();
                return;

            case FreeMovementMode.InteractionType.Key:
                HandleOtherInteraction(interaction.HitObject);
                return;

            case FreeMovementMode.InteractionType.Other:
                HandleOtherInteraction(interaction.HitObject);
                return;
        }
    }

    private void HandleDrivingModeInteraction()
    {
        if (!Input.GetKeyDown(interactKey) || isExitingDrivingMode)
        {
            return;
        }

        if (!freeMovementMode.TryGetInteraction(Kamaz, out FreeMovementMode.InteractionResult interaction))
        {
            return;
        }

        if (interaction.Type == FreeMovementMode.InteractionType.Key)
        {
            HandleKeyInteraction(interaction.KeyAnimator);
            return;
        }

        if (interaction.Type == FreeMovementMode.InteractionType.Steering)
        {
            HandleKeyInteraction(Kamaz != null ? Kamaz.KeyAnimator : null);
            return;
        }

        if (interaction.Type != FreeMovementMode.InteractionType.Door)
        {
            return;
        }

        Transform targetDoor = interaction.DoorRoot;
        if (targetDoor == null)
        {
            return;
        }

        float distanceToDoor = Vector3.Distance(transform.position, targetDoor.position);
        if (distanceToDoor > switchModeDistance)
        {
            return;
        }

        StartCoroutine(ExitDrivingModeWithDelay(interaction.DoorAnimator));
    }

    private IEnumerator ExitDrivingModeWithDelay(Animator exitDoorAnimator)
    {
        isExitingDrivingMode = true;

        if (exitDoorAnimator == null)
        {
            exitDoorAnimator = Kamaz != null ? Kamaz.DoorLAnimator : null;
        }

        if (exitDoorAnimator != null)
        {
            SetDoorOpen(exitDoorAnimator, true);
        }

        yield return new WaitForSeconds(exitToFreeModeDelay);

        waitingControllerReenable = true;
        SetMode(ControlMode.FreeMovement, true);

        SetIgnoreKamazCollisions(true);
        yield return new WaitForSeconds(characterControllerEnableDelayAfterExit);
        drivingMode.SetCharacterControllerEnabled(true);
        RefreshPlayerColliders();

        yield return new WaitForSeconds(ignoreKamazCollisionAfterExit);
        SetIgnoreKamazCollisions(false);

        waitingControllerReenable = false;
        isExitingDrivingMode = false;
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

    private void SetMode(ControlMode mode, bool keepControllerDisabledOnFree = false)
    {
        if (currentMode == mode)
        {
            return;
        }

        currentMode = mode;
        ApplyCurrentMode(keepControllerDisabledOnFree);
    }

    private bool IsDoorOpen()
    {
        Animator doorAnimator = Kamaz != null ? Kamaz.DoorLAnimator : null;
        if (doorAnimator == null || !HasAnimatorBool(doorAnimator, doorOpenBoolName))
        {
            return false;
        }

        return doorAnimator.GetBool(doorOpenBoolName);
    }

    private void EnsureDoorClosed()
    {
        Transform[] doors = { Kamaz != null ? Kamaz.DoorL : null, Kamaz != null ? Kamaz.DoorR : null };
        Animator[] doorAnimators = { Kamaz != null ? Kamaz.DoorLAnimator : null, Kamaz != null ? Kamaz.DoorRAnimator : null };

        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i] != null && doorAnimators[i] != null)
            {
                SetDoorOpen(doorAnimators[i], false);
            }
        }
    }

    private void SetDoorOpen(Animator doorAnimator, bool value)
    {
        if (doorAnimator == null || !HasAnimatorBool(doorAnimator, doorOpenBoolName))
        {
            return;
        }

        doorAnimator.SetBool(doorOpenBoolName, value);
    }

    private void RefreshPlayerColliders()
    {
        playerColliders = GetComponentsInChildren<Collider>(true);
    }

    private void SetIgnoreKamazCollisions(bool ignore)
    {
        if (playerColliders == null || playerColliders.Length == 0)
        {
            RefreshPlayerColliders();
        }

        Collider[] kamazColliders = Kamaz != null ? Kamaz.AllKamazColliders : null;
        if (playerColliders == null || kamazColliders == null)
        {
            return;
        }

        foreach (Collider playerCollider in playerColliders)
        {
            if (playerCollider == null)
            {
                continue;
            }

            foreach (Collider kamazCollider in kamazColliders)
            {
                if (kamazCollider == null)
                {
                    continue;
                }

                Physics.IgnoreCollision(playerCollider, kamazCollider, ignore);
            }
        }
    }

    private bool HasAnimatorBool(Animator animator, string parameterName)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == AnimatorControllerParameterType.Bool)
            {
                return true;
            }
        }

        return false;
    }

    private void HandleDoorInteraction(Animator doorAnimator)
    {
        if (doorAnimator == null)
        {
            return;
        }

        bool isOpen = HasAnimatorBool(doorAnimator, doorOpenBoolName) && doorAnimator.GetBool(doorOpenBoolName);
        SetDoorOpen(doorAnimator, !isOpen);
    }

    private void HandleSteeringInteraction()
    {
        EnsureDoorClosed();
        SetMode(ControlMode.Driving);
    }

    private void HandleKeyInteraction(Animator keyAnimator)
    {
        if (keyAnimator == null || !HasAnimatorBool(keyAnimator, keyTurnBoolName))
        {
            return;
        }

        bool alreadyTurned = keyAnimator.GetBool(keyTurnBoolName);
        keyAnimator.SetBool(keyTurnBoolName, true);

        if (!alreadyTurned)
        {
            StartKabinaStartShake();
            StartStartupNeedleSweep();
        }
    }

    private void HandleOtherInteraction(Transform hitObject)
    {
        // Заглушка для будущих интерактивных объектов (пульты, кнопки и т.д.)
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnDisable()
    {
        StopStartupNeedleSweep(true);
        StopKabinaShakeAndRestore();
        ClearHighlight();
    }

    private void OnGUI()
    {
        if (!showCrosshairDot || crosshairTexture == null)
        {
            return;
        }

        float x = (Screen.width - crosshairSize) * 0.5f;
        float y = (Screen.height - crosshairSize) * 0.5f;

        Color previousColor = GUI.color;
        GUI.color = crosshairColor;
        GUI.DrawTexture(new Rect(x, y, crosshairSize, crosshairSize), crosshairTexture);
        GUI.color = previousColor;
    }

    private void CreateCrosshairTexture()
    {
        crosshairTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        crosshairTexture.SetPixel(0, 0, Color.white);
        crosshairTexture.Apply();
    }

    private void ResolveKamazContext()
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

    private void UpdateInteractionHighlight()
    {
        if (!showInteractableHighlight || Kamaz == null)
        {
            ClearHighlight();
            return;
        }

        if (!freeMovementMode.TryGetInteraction(Kamaz, out FreeMovementMode.InteractionResult interaction))
        {
            ClearHighlight();
            return;
        }

        Transform target = null;
        if (interaction.Type == FreeMovementMode.InteractionType.Door)
        {
            target = interaction.DoorRoot;
        }
        else if (interaction.Type == FreeMovementMode.InteractionType.Steering)
        {
            target = Kamaz.Ryle != null ? Kamaz.Ryle : interaction.HitObject;
        }
        else if (interaction.Type == FreeMovementMode.InteractionType.Key)
        {
            if (currentMode == ControlMode.Driving)
            {
                target = interaction.KeyRoot != null ? interaction.KeyRoot : interaction.HitObject;
            }
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
        ApplyHighlight(target);
    }

    private void ApplyHighlight(Transform root)
    {
        if (root == null)
        {
            return;
        }

        highlightedRoot = root;
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null)
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
                float t = Mathf.Clamp01(interactableHighlightColor.a);
                Color target = Color.Lerp(state.baseColor, Color.white, t);
                material.SetColor("_BaseColor", target);
            }

            if (state.hasColor)
            {
                float t = Mathf.Clamp01(interactableHighlightColor.a);
                Color target = Color.Lerp(state.color, Color.white, t);
                material.SetColor("_Color", target);
            }

            if (state.hasEmission)
            {
                float t = Mathf.Clamp01(interactableHighlightColor.a);
                Color target = Color.Lerp(state.emissionColor, state.emissionColor + Color.white * 0.15f, t);
                material.SetColor("_EmissionColor", target);
            }
        }
    }

    private void ClearHighlight()
    {
        if (highlightedRenderers.Count == 0)
        {
            highlightedRoot = null;
            return;
        }

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
        highlightedRoot = null;
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

    private void StartKabinaStartShake()
    {
        if (kabinaShakeCoroutine != null)
        {
            StopKabinaShakeAndRestore();
        }

        if (!TryCollectKabinaShakeTargets())
        {
            return;
        }

        kabinaShakeCoroutine = StartCoroutine(ShakeKabinaMeshRenderersForSeconds(engineStartShakeDuration));
    }

    private IEnumerator ShakeKabinaMeshRenderersForSeconds(float durationSeconds)
    {
        float elapsed = 0f;
        while (elapsed < durationSeconds)
        {
            elapsed += Time.deltaTime;

            for (int i = 0; i < kabinaShakeTargets.Count; i++)
            {
                Transform target = kabinaShakeTargets[i];
                if (target == null)
                {
                    continue;
                }

                Vector3 offset = Random.insideUnitSphere * engineStartShakeAmplitude;
                offset.y *= 0.35f;
                target.localPosition = kabinaShakeOriginalLocalPositions[i] + offset;
            }

            yield return null;
        }

        kabinaShakeCoroutine = null;
        RestoreKabinaShakeTargets();
    }

    private bool TryCollectKabinaShakeTargets()
    {
        kabinaShakeTargets.Clear();
        kabinaShakeOriginalLocalPositions.Clear();

        Transform kabinaRoot = Kamaz != null ? Kamaz.Kabina : null;
        if (kabinaRoot == null)
        {
            return false;
        }

        kabinaShakeTargets.Add(kabinaRoot);
        kabinaShakeOriginalLocalPositions.Add(kabinaRoot.localPosition);

        return kabinaShakeTargets.Count > 0;
    }

    private void StopKabinaShakeAndRestore()
    {
        if (kabinaShakeCoroutine != null)
        {
            StopCoroutine(kabinaShakeCoroutine);
            kabinaShakeCoroutine = null;
        }

        RestoreKabinaShakeTargets();
    }

    private void RestoreKabinaShakeTargets()
    {
        for (int i = 0; i < kabinaShakeTargets.Count; i++)
        {
            Transform target = kabinaShakeTargets[i];
            if (target == null)
            {
                continue;
            }

            target.localPosition = kabinaShakeOriginalLocalPositions[i];
        }

        kabinaShakeTargets.Clear();
        kabinaShakeOriginalLocalPositions.Clear();
    }

    private void UpdateDrivingZoom()
    {
        if (playerCamera == null)
        {
            return;
        }

        float normalFov = defaultFieldOfView > 0f ? defaultFieldOfView : playerCamera.fieldOfView;
        float zoomedFov = normalFov / Mathf.Max(1f, drivingZoomMultiplier);
        bool isZooming = currentMode == ControlMode.Driving && Input.GetMouseButton(1);
        float targetFov = isZooming ? zoomedFov : normalFov;

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFov,
            drivingZoomLerpSpeed * Time.deltaTime
        );
    }

    private void TryInitializeNeedles()
    {
        if (Kamaz == null)
        {
            return;
        }

        if (!speedometerNeedleConfigured)
        {
            if (speedometerNeedle == null)
            {
                speedometerNeedle = Kamaz.SpidometerNeedle;
            }

            if (speedometerNeedle != null)
            {
                speedometerNeedle.SetDebugMode(false);
                speedometerNeedle.Configure(0f, 180f, 0.3f, 264f);
                speedometerNeedle.SetValueImmediate(0f);
                speedometerNeedleConfigured = true;
            }
        }

        if (!tachometerNeedleConfigured)
        {
            if (tachometerNeedle == null)
            {
                tachometerNeedle = Kamaz.TachometerNeedle;
            }

            if (tachometerNeedle != null)
            {
                tachometerNeedle.SetDebugMode(false);
                tachometerNeedle.Configure(0f, 8f, 2f, 240f);
                tachometerNeedle.SetValueImmediate(0f);
                tachometerNeedleConfigured = true;
            }
        }
    }

    private void StartStartupNeedleSweep()
    {
        TryInitializeNeedles();

        if (speedometerNeedle == null && tachometerNeedle == null)
        {
            return;
        }

        StopStartupNeedleSweep(false);
        startupNeedleSweepCoroutine = StartCoroutine(StartupNeedleSweepRoutine());
    }

    private IEnumerator StartupNeedleSweepRoutine()
    {
        float elapsed = 0f;
        while (elapsed < startupSweepUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = startupSweepUpDuration > 0f ? Mathf.Clamp01(elapsed / startupSweepUpDuration) : 1f;
            float speedValue = Mathf.Lerp(0f, startupSpeedometerPeak, t);
            float tachValue = Mathf.Lerp(0f, startupTachometerPeak, t);
            ApplyNeedleValuesImmediate(speedValue, tachValue);
            yield return null;
        }

        if (startupSweepHoldDuration > 0f)
        {
            ApplyNeedleValuesImmediate(startupSpeedometerPeak, startupTachometerPeak);
            yield return new WaitForSeconds(startupSweepHoldDuration);
        }

        elapsed = 0f;
        while (elapsed < startupSweepDownDuration)
        {
            elapsed += Time.deltaTime;
            float t = startupSweepDownDuration > 0f ? Mathf.Clamp01(elapsed / startupSweepDownDuration) : 1f;
            float speedValue = Mathf.Lerp(startupSpeedometerPeak, 0f, t);
            float tachValue = Mathf.Lerp(startupTachometerPeak, 0f, t);
            ApplyNeedleValuesImmediate(speedValue, tachValue);
            yield return null;
        }

        ApplyNeedleValuesImmediate(0f, 0f);
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
            ApplyNeedleValuesImmediate(0f, 0f);
        }
    }

    private void ApplyNeedleValuesImmediate(float speedValue, float tachValue)
    {
        if (speedometerNeedle != null)
        {
            speedometerNeedle.SetValueImmediate(speedValue);
        }

        if (tachometerNeedle != null)
        {
            tachometerNeedle.SetValueImmediate(tachValue);
        }
    }
}
