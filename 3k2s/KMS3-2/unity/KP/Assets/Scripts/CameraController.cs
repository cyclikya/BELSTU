using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
    [SerializeField] private KamazAudioController kamazAudioController;
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
    [SerializeField] private Font interactionHintFont;
    [SerializeField] private int interactionHintFontSize = 27;

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
    private string interactionHintText = string.Empty;
    private string temporaryHintText = string.Empty;
    private float temporaryHintUntilTime;

    public bool IsDrivingMode => currentMode == ControlMode.Driving;
    public bool IsEngineRunning => engineRunning;

    // Собирает ссылки, инициализирует режимы и подготавливает UI.
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        playerColliders = GetComponentsInChildren<Collider>(true);

        if (kamazContext != null)
        {
            kamazLightsController ??= kamazContext.LightsController;
            kamazAudioController ??= kamazContext.AudioController;
            kamazCabinMechanismsController ??= kamazContext.CabinMechanismsController;
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

    // Каждый кадр обновляет активный режим игрока, подсказки и зум камеры.
    private void Update()
    {
        HandleUiCursorToggle();
        if (uiCursorActive)
        {
            return;
        }

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

    // Обрабатывает открытие двери и посадку в кабину в пешем режиме.
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

    // Обрабатывает запуск двигателя и выход из кабины в режиме вождения.
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

        if (Vector3.Distance(transform.position, interaction.DoorRoot.position) > switchModeDistance)
        {
            return;
        }

        kamazLightsController?.BlockInputForSeconds(exitToFreeModeDelay + 0.25f);
        StartCoroutine(ExitDrivingMode(interaction.DoorAnimator));
    }

    // Открывает дверь, выводит игрока из кабины и возвращает пеший режим.
    private IEnumerator ExitDrivingMode(Animator doorAnimator)
    {
        isExitingDrivingMode = true;
        IgnoreKamazCollisions(true);
        FreezeKamaz(true);
        SetDoorState(doorAnimator, true, true);

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

    // Закрывает двери и переводит игрока в режим вождения.
    private void EnterDrivingMode()
    {
        if (kamazContext == null)
        {
            return;
        }

        SetDoorState(kamazContext.DoorLAnimator, false, true);
        SetDoorState(kamazContext.DoorRAnimator, false, true);
        SetMode(ControlMode.Driving);
    }

    // Меняет режим игрока и синхронизирует состояние света.
    private void SetMode(ControlMode mode, bool keepControllerDisabledOnFree = false)
    {
        currentMode = mode;
        ApplyCurrentMode(keepControllerDisabledOnFree);
        SyncLightsControllerState();
    }

    // Включает нужный режим и отключает второй.
    private void ApplyCurrentMode(bool keepControllerDisabledOnFree = false)
    {
        if (currentMode == ControlMode.FreeMovement)
        {
            drivingMode.ExitMode(keepControllerDisabledOnFree);
            if (!keepControllerDisabledOnFree)
            {
                freeMovementMode.EnterMode();
            }
            return;
        }

        freeMovementMode.ExitMode();
        drivingMode.EnterMode();
    }

    // Переключает анимацию двери и временно замораживает КамАЗ.
    private void ToggleDoor(Animator doorAnimator)
    {
        if (doorAnimator == null)
        {
            return;
        }

        StartDoorFreeze();
        SetDoorState(doorAnimator, !doorAnimator.GetBool("isOpen"), true);
    }

    // Применяет состояние двери и при необходимости воспроизводит звук.
    private void SetDoorState(Animator doorAnimator, bool isOpen, bool playAudio)
    {
        if (doorAnimator == null)
        {
            return;
        }

        bool wasOpen = doorAnimator.GetBool("isOpen");
        doorAnimator.SetBool("isOpen", isOpen);

        if (!playAudio || wasOpen == isOpen || kamazAudioController == null)
        {
            return;
        }

        if (isOpen) kamazAudioController.PlayDoorOpen();
        else kamazAudioController.PlayDoorClose();
    }

    // Перезапускает короткую заморозку КамАЗа во время анимации двери.
    private void StartDoorFreeze()
    {
        if (doorFreezeCoroutine != null)
        {
            StopCoroutine(doorFreezeCoroutine);
            FreezeKamaz(false);
        }

        doorFreezeCoroutine = StartCoroutine(DoorFreezeRoutine());
    }

    // На короткое время останавливает машину, чтобы дверь открывалась стабильно.
    private IEnumerator DoorFreezeRoutine()
    {
        FreezeKamaz(true);
        yield return new WaitForSeconds(doorFreezeDuration);
        FreezeKamaz(false);
        doorFreezeCoroutine = null;
    }

    // Включает или выключает физику КамАЗа на время переходов и анимаций.
    private void FreezeKamaz(bool value)
    {
        if (kamazRigidbody == null)
        {
            return;
        }

        kamazRigidbody.linearVelocity = Vector3.zero;
        kamazRigidbody.angularVelocity = Vector3.zero;
        kamazRigidbody.isKinematic = value ? true : kamazWasKinematic;
    }

    // Включает или выключает столкновения игрока с КамАЗом.
    private void IgnoreKamazCollisions(bool ignore)
    {
        if (playerColliders == null || kamazContext == null || kamazContext.AllKamazColliders == null)
        {
            return;
        }

        for (int i = 0; i < playerColliders.Length; i++)
        {
            Collider playerCollider = playerColliders[i];
            if (playerCollider == null) continue;

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

    // Внешняя точка входа для запуска или остановки двигателя.
    public void SetEngineRunningState(bool value)
    {
        SetEngineRunning(value);
    }

    // Запускает или глушит двигатель, ключ и звуки.
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

        if (engineRunning) kamazAudioController?.StartEngineAudio();
        else kamazAudioController?.StopEngineAudio();

        SyncLightsControllerState();
    }

    // Вызывается, когда двигатель заглох во время движения.
    public void HandleEngineStall()
    {
        if (!engineRunning)
        {
            return;
        }

        engineRunning = false;
        if (kamazContext != null && kamazContext.KeyAnimator != null)
        {
            kamazContext.KeyAnimator.SetBool("turn", false);
        }

        kamazAudioController?.StallEngineAudio();
        SyncLightsControllerState();
    }

    // Передает свету состояние двигателя и факт, сидит ли игрок в кабине.
    private void SyncLightsControllerState()
    {
        if (kamazLightsController == null)
        {
            return;
        }

        kamazLightsController.SetEngineRunning(engineRunning);
        kamazLightsController.SetInCabin(currentMode == ControlMode.Driving);
    }

    // Один раз настраивает диапазоны стрелок приборов.
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

    // Плавно меняет угол обзора при зажатой правой кнопке мыши в режиме вождения.
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

    // Формирует текст подсказки в зависимости от того, на что смотрит игрок.
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

    // Рисует точку прицела и нижнюю подсказку.
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

    // Выводит на экран активную текстовую подсказку.
    private void DrawInteractionHint()
    {
        string text = GetActiveHintText();
        if (!showInteractionHint || string.IsNullOrEmpty(text))
        {
            return;
        }

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.font = interactionHintFont;
        style.fontSize = interactionHintFontSize;
        style.normal.textColor = interactionHintColor;

        float width = Mathf.Min(Screen.width - 40f, 900f);
        float x = (Screen.width - width) * 0.5f;
        float y = Screen.height - 52f;
        GUI.Label(new Rect(x, y, width, 28f), text, style);
    }

    // Возвращает временную подсказку, если она еще активна, иначе обычную.
    private string GetActiveHintText()
    {
        return !string.IsNullOrEmpty(temporaryHintText) && Time.unscaledTime < temporaryHintUntilTime
            ? temporaryHintText
            : interactionHintText;
    }

    // Показывает временное сообщение поверх обычной подсказки.
    private void ShowTemporaryHint(string text, float duration = 2.5f)
    {
        temporaryHintText = text;
        temporaryHintUntilTime = Time.unscaledTime + duration;
    }

    // Включает и выключает свободный курсор для работы с UI.
    private void HandleUiCursorToggle()
    {
        if (allowUiCursorToggle && Input.GetKeyDown(toggleUiCursorKey))
        {
            SetUiCursorState(!uiCursorActive);
        }
    }

    // Переключает видимость курсора и режим его блокировки.
    private void SetUiCursorState(bool enabled)
    {
        uiCursorActive = enabled;
        Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enabled;
    }

    // Создает простую белую текстуру для точки прицела.
    private void CreateCrosshairTexture()
    {
        crosshairTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        crosshairTexture.SetPixel(0, 0, Color.white);
        crosshairTexture.Apply();
    }

    // Восстанавливает режим курсора после возврата фокуса окну игры.
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            SetUiCursorState(uiCursorActive);
        }
    }

    // Отключает двигатель, звуки и временные состояния при выключении объекта.
    private void OnDisable()
    {
        SetEngineRunning(false);
        kamazAudioController?.StopAllLoops();
        IgnoreKamazCollisions(false);
        FreezeKamaz(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
