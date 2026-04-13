using UnityEngine;

public class KamazDrivingInputController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private KamazContext kamazContext;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Rigidbody kamazRigidbody;
    [SerializeField] private Transform steeringWheel;

    [Header("Activation")]
    [SerializeField] private bool onlyWhenPlayerInCabin = true;
    [SerializeField] private bool requireEngineRunningForMotion = true;

    [Header("Input")]
    [SerializeField] private KeyCode throttleKey = KeyCode.W;
    [SerializeField] private KeyCode brakeKey = KeyCode.S;
    [SerializeField] private KeyCode steerLeftKey = KeyCode.A;
    [SerializeField] private KeyCode steerRightKey = KeyCode.D;
    [SerializeField] private KeyCode clutchKey = KeyCode.LeftShift;

    [Header("Gear Keys")]
    [SerializeField] private KeyCode neutralKey = KeyCode.N;
    [SerializeField] private KeyCode gear1Key = KeyCode.Alpha1;
    [SerializeField] private KeyCode gear2Key = KeyCode.Alpha2;
    [SerializeField] private KeyCode gear3Key = KeyCode.Alpha3;
    [SerializeField] private KeyCode gear4Key = KeyCode.Alpha4;
    [SerializeField] private KeyCode gear5Key = KeyCode.Alpha5;
    [SerializeField] private KeyCode reverseKey = KeyCode.R;

    [Header("Clutch")]
    [SerializeField] private float clutchPressSpeed = 10f;
    [SerializeField] private float clutchReleaseDuration = 0.3f;
    [SerializeField] private float clutchRequiredForShift = 0.85f;

    [Header("Transmission")]
    [SerializeField] private int currentGear = 0;
    [SerializeField] private float reverseGearRatio = -7.38f;
    [SerializeField] private float gear1Ratio = 7.82f;
    [SerializeField] private float gear2Ratio = 4.03f;
    [SerializeField] private float gear3Ratio = 2.50f;
    [SerializeField] private float gear4Ratio = 1.53f;
    [SerializeField] private float gear5Ratio = 1.0f;
    [SerializeField] private float reverseMaxSpeedKmh = 15f;
    [SerializeField] private float gear1MaxSpeedKmh = 15f;
    [SerializeField] private float gear2MaxSpeedKmh = 30f;
    [SerializeField] private float gear3MaxSpeedKmh = 45f;
    [SerializeField] private float gear4MaxSpeedKmh = 60f;
    [SerializeField] private float gear5MaxSpeedKmh = 90f;

    [Header("Engine")]
    [SerializeField] private float idleRpm = 650f;
    [SerializeField] private float maxRpm = 3000f;
    [SerializeField] private float stallRpm = 450f;
    [SerializeField] private float rpmRiseSpeed = 1400f;
    [SerializeField] private float rpmDropSpeed = 1000f;
    [SerializeField] private float clutchPressedRpmDropMultiplier = 0.4f;
    [SerializeField] private float stallCheckDelay = 0.6f;
    [SerializeField] private float stallClutchEngagement = 0.9f;
    [SerializeField] private float stallThrottleBypass = 0.3f;

    [Header("Movement")]
    [SerializeField] private float baseDriveForce = 26000f;
    [SerializeField] private float brakeForce = 15000f;
    [SerializeField] private float rollingDragForceCoast = 0.4f;
    [SerializeField] private float rollingDragForceThrottle = 0.08f;
    [SerializeField] private float engineBrakeForce = 40f;
    [SerializeField] private float engineBrakeThrottleThreshold = 0.05f;
    [SerializeField] private float maxForwardSpeedKmh = 90f;
    [SerializeField] private float maxReverseSpeedKmh = 15f;
    [SerializeField] private float lowRpmTorqueBoost = 2.6f;
    [SerializeField] private float lowRpmBoostEnd = 2300f;

    [Header("Vehicle Steering")]
    [SerializeField] private bool invertSteering;
    [SerializeField] private float minTurnRadiusAtFullSteer = 180f;
    [SerializeField] private float maxTurnRadiusAtLowSteer = 1200f;
    [SerializeField] private float steeringMinSpeedMps = 2.5f;
    [SerializeField] private float steeringYawMultiplier = 0.3f;

    [Header("Steering Wheel Visual")]
    [SerializeField] private float steeringWheelSpeed = 230f;
    [SerializeField] private float steeringWheelReturnSpeed = 300f;
    [SerializeField] private float steeringMinAngle = -110f;
    [SerializeField] private float steeringMaxAngle = 110f;

    [Header("Wheel Visuals")]
    [SerializeField] private float wheelRadiusMeters = 0.55f;
    [SerializeField] private float minWheelSpinSpeedMps = 0.01f;
    [SerializeField] private float frontWheelMaxSteerAngle = 28f;
    [SerializeField] private float frontWheelSteerSpeed = 140f;
    [SerializeField] private Vector3 wheelSpinAxisLocal = Vector3.right;

    [Header("HUD")]
    [SerializeField] private bool showDrivingHud = true;
    [SerializeField] private float hudLeftMargin = 20f;
    [SerializeField] private float hudBottomMargin = 20f;
    [SerializeField] private float hudWidth = 420f;
    [SerializeField] private float hudHeight = 150f;
    [SerializeField] private int hudFontSize = 16;
    [SerializeField] private float recommendUpshiftRpm = 2500f;
    [SerializeField] private float recommendDownshiftRpm = 1100f;

    [Header("Runtime (Debug)")]
    [SerializeField] private float engineRpm;
    [SerializeField] private float clutchPedal;
    [SerializeField] private float speedKmh;
    [SerializeField] private bool engineStalled;

    private float steerInput;
    private float throttleInput;
    private float brakeInput;
    private float clutchTarget;
    private float wheelSteeringAngle;
    private float stallTimer;

    private Quaternion neutralWheelLocalRotation = Quaternion.identity;
    private bool neutralWheelRotationCached;
    private GaugeNeedle speedometerNeedle;
    private GaugeNeedle tachometerNeedle;

    private Transform wheelFrontLeft;
    private Transform wheelFrontRight;
    private Transform wheelMiddle;
    private Transform wheelBack;

    private Quaternion wheelFrontLeftBaseLocalRotation = Quaternion.identity;
    private Quaternion wheelFrontRightBaseLocalRotation = Quaternion.identity;
    private Quaternion wheelMiddleBaseLocalRotation = Quaternion.identity;
    private Quaternion wheelBackBaseLocalRotation = Quaternion.identity;
    private bool wheelBaseRotationsCached;
    private float wheelSpinAngle;
    private float frontWheelSteerVisualAngle;

    private void Awake()
    {
        ResolveReferences();
        CacheNeutralSteeringWheelRotation();
        engineRpm = idleRpm;
        UpdateGaugeOutput();
    }

    private void OnEnable()
    {
        ResolveReferences();
        CacheNeutralSteeringWheelRotation();
        CacheWheelBaseRotations();
        ApplySteeringWheelRotation();
        ApplyWheelVisuals();
        UpdateGaugeOutput();
    }

    private void Update()
    {
        ResolveReferencesIfMissing();

        bool canReadDriverInput = !onlyWhenPlayerInCabin || IsPlayerInCabin();
        ReadDriverInput(canReadDriverInput);
        HandleGearInput(canReadDriverInput);

        UpdateClutch(Time.deltaTime);
        UpdateSteeringWheelVisual(Time.deltaTime);
        UpdateWheelVisuals(Time.deltaTime);
        UpdateEngineRpm(Time.deltaTime);
        UpdateGaugeOutput();
    }

    private void OnGUI()
    {
        if (!showDrivingHud)
        {
            return;
        }

        if (onlyWhenPlayerInCabin && !IsPlayerInCabin())
        {
            return;
        }

        DrawDrivingHud();
    }

    private void FixedUpdate()
    {
        ResolveReferencesIfMissing();
        if (kamazRigidbody == null)
        {
            return;
        }

        float forwardSpeedMps = GetForwardSpeedMps();
        speedKmh = GetVehicleGroundSpeedKmh();

        ApplyVehicleSteering(forwardSpeedMps);
        ApplyForces(forwardSpeedMps);
        UpdateGaugeOutput();
    }

    [ContextMenu("Capture Current Wheel Rotation As Neutral")]
    public void CaptureCurrentRotationAsNeutral()
    {
        if (steeringWheel == null)
        {
            ResolveReferences();
        }

        if (steeringWheel == null)
        {
            return;
        }

        neutralWheelLocalRotation = steeringWheel.localRotation;
        neutralWheelRotationCached = true;
        wheelSteeringAngle = 0f;
        ApplySteeringWheelRotation();
    }

    private void ReadDriverInput(bool canReadDriverInput)
    {
        if (!canReadDriverInput)
        {
            steerInput = 0f;
            throttleInput = 0f;
            brakeInput = 0f;
            clutchTarget = 0f;
            return;
        }

        throttleInput = Input.GetKey(throttleKey) ? 1f : 0f;
        brakeInput = Input.GetKey(brakeKey) ? 1f : 0f;

        float left = Input.GetKey(steerLeftKey) ? 1f : 0f;
        float right = Input.GetKey(steerRightKey) ? 1f : 0f;
        steerInput = right - left;

        if (invertSteering)
        {
            steerInput *= -1f;
        }

        clutchTarget = Input.GetKey(clutchKey) ? 1f : 0f;
    }

    private void HandleGearInput(bool canReadDriverInput)
    {
        if (!canReadDriverInput)
        {
            return;
        }

        if (Input.GetKeyDown(neutralKey))
        {
            currentGear = 0;
            return;
        }

        if (Input.GetKeyDown(gear1Key))
        {
            TryShiftToGear(1);
            return;
        }

        if (Input.GetKeyDown(gear2Key))
        {
            TryShiftToGear(2);
            return;
        }

        if (Input.GetKeyDown(gear3Key))
        {
            TryShiftToGear(3);
            return;
        }

        if (Input.GetKeyDown(gear4Key))
        {
            TryShiftToGear(4);
            return;
        }

        if (Input.GetKeyDown(gear5Key))
        {
            TryShiftToGear(5);
            return;
        }

        if (Input.GetKeyDown(reverseKey))
        {
            TryShiftToGear(-1);
        }
    }

    private void TryShiftToGear(int targetGear)
    {
        if (targetGear == currentGear)
        {
            return;
        }

        if (targetGear == 0)
        {
            currentGear = 0;
            return;
        }

        bool clutchPressedEnough = clutchPedal >= clutchRequiredForShift;
        if (!clutchPressedEnough)
        {
            return;
        }

        currentGear = targetGear;
    }

    private void UpdateClutch(float deltaTime)
    {
        float releaseSpeed = clutchReleaseDuration > 0.0001f ? 1f / clutchReleaseDuration : 100f;
        float speed = clutchTarget > clutchPedal ? clutchPressSpeed : releaseSpeed;
        clutchPedal = Mathf.MoveTowards(clutchPedal, clutchTarget, speed * deltaTime);
    }

    private void UpdateSteeringWheelVisual(float deltaTime)
    {
        if (steeringWheel == null)
        {
            return;
        }

        if (Mathf.Abs(steerInput) > 0.001f)
        {
            wheelSteeringAngle += steerInput * steeringWheelSpeed * deltaTime;
        }
        else
        {
            wheelSteeringAngle = Mathf.MoveTowards(wheelSteeringAngle, 0f, steeringWheelReturnSpeed * deltaTime);
        }

        float minAngle = Mathf.Min(steeringMinAngle, steeringMaxAngle);
        float maxAngle = Mathf.Max(steeringMinAngle, steeringMaxAngle);
        wheelSteeringAngle = Mathf.Clamp(wheelSteeringAngle, minAngle, maxAngle);

        ApplySteeringWheelRotation();
    }

    private void ApplySteeringWheelRotation()
    {
        if (steeringWheel == null)
        {
            return;
        }

        if (!neutralWheelRotationCached)
        {
            CacheNeutralSteeringWheelRotation();
        }

        steeringWheel.localRotation = neutralWheelLocalRotation * Quaternion.AngleAxis(wheelSteeringAngle, Vector3.up);
    }

    private void UpdateWheelVisuals(float deltaTime)
    {
        if (kamazRigidbody == null)
        {
            return;
        }

        if (!wheelBaseRotationsCached)
        {
            CacheWheelBaseRotations();
        }

        float forwardSpeedMps = GetForwardSpeedMps();
        float absSpeed = Mathf.Abs(forwardSpeedMps);

        if (absSpeed >= minWheelSpinSpeedMps)
        {
            float radius = Mathf.Max(0.01f, wheelRadiusMeters);
            float circumference = Mathf.Max(0.05f, 2f * Mathf.PI * radius);
            float degreesPerSecond = (absSpeed / circumference) * 360f;
            float directionSign = Mathf.Sign(forwardSpeedMps);
            wheelSpinAngle += directionSign * degreesPerSecond * deltaTime;
            wheelSpinAngle = Mathf.Repeat(wheelSpinAngle, 360f);
        }

        bool canReadDriverInput = !onlyWhenPlayerInCabin || IsPlayerInCabin();
        float targetSteerAngle = canReadDriverInput ? steerInput * frontWheelMaxSteerAngle : 0f;
        frontWheelSteerVisualAngle = Mathf.MoveTowards(frontWheelSteerVisualAngle, targetSteerAngle, frontWheelSteerSpeed * deltaTime);

        ApplyWheelVisuals();
    }

    private void ApplyWheelVisuals()
    {
        Quaternion spinRotation = Quaternion.AngleAxis(wheelSpinAngle, GetNormalizedAxis(wheelSpinAxisLocal, Vector3.right));
        Quaternion steerRotation = Quaternion.AngleAxis(frontWheelSteerVisualAngle, Vector3.forward);

        if (wheelFrontLeft != null)
        {
            wheelFrontLeft.localRotation = wheelFrontLeftBaseLocalRotation * steerRotation * spinRotation;
        }

        if (wheelFrontRight != null)
        {
            wheelFrontRight.localRotation = wheelFrontRightBaseLocalRotation * steerRotation * spinRotation;
        }

        if (wheelMiddle != null)
        {
            wheelMiddle.localRotation = wheelMiddleBaseLocalRotation * spinRotation;
        }

        if (wheelBack != null)
        {
            wheelBack.localRotation = wheelBackBaseLocalRotation * spinRotation;
        }
    }

    private void CacheNeutralSteeringWheelRotation()
    {
        if (steeringWheel == null)
        {
            return;
        }

        neutralWheelLocalRotation = steeringWheel.localRotation;
        neutralWheelRotationCached = true;
    }

    private void UpdateEngineRpm(float deltaTime)
    {
        bool engineRunning = IsEngineRunning();

        if (engineRunning && engineStalled)
        {
            engineStalled = false;
            stallTimer = 0f;
            engineRpm = Mathf.Max(idleRpm, engineRpm);
        }

        if (!engineRunning)
        {
            engineRpm = Mathf.MoveTowards(engineRpm, 0f, rpmDropSpeed * deltaTime);
            stallTimer = 0f;
            return;
        }

        float speedAbsKmh = Mathf.Abs(GetForwardSpeedMps()) * 3.6f;
        float freeRevTarget = Mathf.Lerp(idleRpm, maxRpm, throttleInput);
        float clutchEngagement = GetClutchEngagement();
        float coupledRpm = speedAbsKmh * Mathf.Abs(GetCurrentGearRpmPerKmh());

        float targetRpm = currentGear == 0
            ? freeRevTarget
            : Mathf.Lerp(freeRevTarget, Mathf.Max(idleRpm * 0.35f, coupledRpm), clutchEngagement);

        float response = targetRpm > engineRpm ? rpmRiseSpeed : rpmDropSpeed;
        if (targetRpm < engineRpm && clutchPedal > 0.6f)
        {
            response *= clutchPressedRpmDropMultiplier;
        }

        engineRpm = Mathf.MoveTowards(engineRpm, targetRpm, response * deltaTime);
        engineRpm = Mathf.Clamp(engineRpm, 0f, maxRpm);

        bool canStall = currentGear != 0
            && clutchEngagement >= stallClutchEngagement
            && throttleInput <= stallThrottleBypass;

        if (canStall && engineRpm < stallRpm)
        {
            stallTimer += deltaTime;
            if (stallTimer >= stallCheckDelay)
            {
                StallEngine();
            }
        }
        else
        {
            stallTimer = 0f;
        }
    }

    private void StallEngine()
    {
        if (!IsEngineRunning())
        {
            return;
        }

        engineStalled = true;
        stallTimer = 0f;
        engineRpm = 0f;
        ResetDrivingStateAfterStall();

        if (cameraController != null)
        {
            cameraController.SetEngineRunningState(false);
            return;
        }

        Animator keyAnimator = kamazContext != null ? kamazContext.KeyAnimator : null;
        if (keyAnimator != null && HasAnimatorBool(keyAnimator, "turn"))
        {
            keyAnimator.SetBool("turn", false);
        }
    }

    private void ApplyVehicleSteering(float forwardSpeedMps)
    {
        if (kamazRigidbody == null)
        {
            return;
        }

        bool canControl = !onlyWhenPlayerInCabin || IsPlayerInCabin();
        if (!canControl)
        {
            return;
        }

        if (Mathf.Abs(steerInput) < 0.001f)
        {
            return;
        }

        float absForwardSpeed = Mathf.Abs(forwardSpeedMps);
        if (absForwardSpeed < steeringMinSpeedMps)
        {
            return;
        }

        float directionFactor = forwardSpeedMps >= 0f ? 1f : -1f;
        float steerAmount = Mathf.Clamp01(Mathf.Abs(steerInput));
        float radius = Mathf.Lerp(maxTurnRadiusAtLowSteer, Mathf.Max(0.5f, minTurnRadiusAtFullSteer), steerAmount);

        // Поворот дугой: omega = v / R. Если нет скорости, поворота нет.
        float yawRateRad = absForwardSpeed / Mathf.Max(0.1f, radius);
        float yawDelta = yawRateRad * Mathf.Rad2Deg * Mathf.Sign(steerInput) * directionFactor * Time.fixedDeltaTime;
        yawDelta *= Mathf.Max(0f, steeringYawMultiplier);

        Quaternion localTurn = Quaternion.AngleAxis(yawDelta, Vector3.forward);
        Quaternion targetRotation = kamazRigidbody.rotation * localTurn;
        kamazRigidbody.MoveRotation(targetRotation);
    }

    private void ApplyForces(float forwardSpeedMps)
    {
        if (kamazRigidbody == null)
        {
            return;
        }

        Vector3 velocity = kamazRigidbody.linearVelocity;
        float drag = throttleInput > engineBrakeThrottleThreshold ? rollingDragForceThrottle : rollingDragForceCoast;
        float massScale = Mathf.Max(1f, kamazRigidbody.mass / 1000f);
        kamazRigidbody.AddForce(-velocity * (drag * massScale), ForceMode.Force);

        if (brakeInput > 0.001f)
        {
            if (velocity.sqrMagnitude > 0.0001f)
            {
                kamazRigidbody.AddForce(-velocity.normalized * (brakeForce * brakeInput), ForceMode.Force);
            }
        }

        float clutchEngagement = GetClutchEngagement();
        float gearRatio = GetCurrentGearRatio();
        Vector3 driveAxis = GetDriveDirectionWorld();

        bool shouldApplyEngineBraking = throttleInput <= engineBrakeThrottleThreshold;
        if (currentGear != 0 && clutchEngagement > 0.001f && shouldApplyEngineBraking)
        {
            kamazRigidbody.AddForce(-driveAxis * (forwardSpeedMps * engineBrakeForce * clutchEngagement * Mathf.Abs(gearRatio) * massScale), ForceMode.Force);
        }

        bool engineRunning = IsEngineRunning();
        if (requireEngineRunningForMotion && !engineRunning)
        {
            return;
        }

        if (currentGear == 0 || throttleInput <= 0.001f)
        {
            return;
        }

        float speedAbsKmh = Mathf.Abs(forwardSpeedMps) * 3.6f;
        float gearMaxSpeed = GetCurrentGearMaxSpeedKmh();
        if (gearMaxSpeed > 0f && speedAbsKmh >= gearMaxSpeed)
        {
            return;
        }

        if (gearRatio < 0f && speedAbsKmh >= maxReverseSpeedKmh)
        {
            return;
        }

        float rpmFactor = Mathf.Clamp01(engineRpm / Mathf.Max(maxRpm, 1f));
        float torqueFactor = Mathf.Lerp(0.55f, 1f, rpmFactor);

        if (engineRpm < lowRpmBoostEnd && lowRpmBoostEnd > 1f)
        {
            float lowRpmT = Mathf.Clamp01(engineRpm / lowRpmBoostEnd);
            torqueFactor *= Mathf.Lerp(lowRpmTorqueBoost, 1f, lowRpmT);
        }

        float driveForce = baseDriveForce * Mathf.Abs(gearRatio) * throttleInput * clutchEngagement * torqueFactor;
        Vector3 driveDirection = driveAxis * Mathf.Sign(gearRatio);
        kamazRigidbody.AddForce(driveDirection * (driveForce * massScale), ForceMode.Force);
    }

    private float GetCurrentGearRatio()
    {
        if (currentGear < 0)
        {
            return reverseGearRatio;
        }

        switch (currentGear)
        {
            case 1:
                return gear1Ratio;
            case 2:
                return gear2Ratio;
            case 3:
                return gear3Ratio;
            case 4:
                return gear4Ratio;
            case 5:
                return gear5Ratio;
            default:
                return 0f;
        }
    }

    private float GetClutchEngagement()
    {
        return Mathf.Clamp01(1f - clutchPedal);
    }

    private float GetCurrentGearRpmPerKmh()
    {
        float maxSpeedForGear = GetCurrentGearMaxSpeedKmh();
        if (maxSpeedForGear <= 0.01f)
        {
            return 0f;
        }

        // Для стабильной игровой логики: каждая передача выходит к redline примерно в конце своего диапазона скорости.
        return maxRpm / maxSpeedForGear;
    }

    private float GetCurrentGearMaxSpeedKmh()
    {
        if (currentGear < 0)
        {
            return reverseMaxSpeedKmh;
        }

        switch (currentGear)
        {
            case 1:
                return gear1MaxSpeedKmh;
            case 2:
                return gear2MaxSpeedKmh;
            case 3:
                return gear3MaxSpeedKmh;
            case 4:
                return gear4MaxSpeedKmh;
            case 5:
                return gear5MaxSpeedKmh;
            default:
                return maxForwardSpeedKmh;
        }
    }

    private float GetForwardSpeedMps()
    {
        if (kamazRigidbody == null)
        {
            return 0f;
        }

        Vector3 localVelocity = kamazRigidbody.transform.InverseTransformDirection(kamazRigidbody.linearVelocity);
        return -localVelocity.y;
    }

    private Vector3 GetDriveDirectionWorld()
    {
        // Фиксированно: движение вперед по локальной оси -Y.
        Transform axisSource = kamazRigidbody != null ? kamazRigidbody.transform : transform;
        return axisSource.TransformDirection(-Vector3.up).normalized;
    }

    private float GetVehicleGroundSpeedKmh()
    {
        if (kamazRigidbody == null)
        {
            return 0f;
        }

        // Учитываем движение в плоскости локальных X/Y (без локальной "вертикали" Z).
        Vector3 localVelocity = kamazRigidbody.transform.InverseTransformDirection(kamazRigidbody.linearVelocity);
        float planarSpeedMps = Mathf.Sqrt(localVelocity.x * localVelocity.x + localVelocity.y * localVelocity.y);
        return planarSpeedMps * 3.6f;
    }

    private void ResetDrivingStateAfterStall()
    {
        // Полный сброс, чтобы повторный запуск всегда был возможен и предсказуем.
        currentGear = 0;
        throttleInput = 0f;
        brakeInput = 0f;
        steerInput = 0f;
        clutchTarget = 0f;
        clutchPedal = 0f;
        wheelSteeringAngle = 0f;
        ApplySteeringWheelRotation();
    }

    private bool IsPlayerInCabin()
    {
        return cameraController != null && cameraController.IsDrivingMode;
    }

    private bool IsEngineRunning()
    {
        if (cameraController != null)
        {
            return cameraController.IsEngineRunning;
        }

        Animator keyAnimator = kamazContext != null ? kamazContext.KeyAnimator : null;
        if (keyAnimator != null && HasAnimatorBool(keyAnimator, "turn"))
        {
            return keyAnimator.GetBool("turn");
        }

        return false;
    }

    private bool HasAnimatorBool(Animator animator, string parameterName)
    {
        if (animator == null)
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == AnimatorControllerParameterType.Bool)
            {
                return true;
            }
        }

        return false;
    }

    private void ResolveReferencesIfMissing()
    {
        if (kamazContext != null && cameraController != null && kamazRigidbody != null && steeringWheel != null
            && wheelFrontLeft != null && wheelFrontRight != null)
        {
            return;
        }

        ResolveReferences();
    }

    private void ResolveReferences()
    {
        if (kamazContext == null)
        {
            kamazContext = GetComponent<KamazContext>();
        }

        if (kamazContext == null)
        {
            kamazContext = KamazContext.Instance;
        }

        if (kamazContext != null)
        {
            kamazContext.AutoResolve();
        }

        if (cameraController == null)
        {
#if UNITY_2023_1_OR_NEWER
            cameraController = FindFirstObjectByType<CameraController>();
#else
            cameraController = FindObjectOfType<CameraController>();
#endif
        }

        if (steeringWheel == null && kamazContext != null)
        {
            steeringWheel = kamazContext.Ryle;
        }

        if (kamazRigidbody == null)
        {
            if (kamazContext != null && kamazContext.Root != null)
            {
                kamazRigidbody = kamazContext.Root.GetComponent<Rigidbody>();
                if (kamazRigidbody == null)
                {
                    kamazRigidbody = kamazContext.Root.GetComponentInChildren<Rigidbody>(true);
                }
            }

            if (kamazRigidbody == null)
            {
                kamazRigidbody = GetComponent<Rigidbody>();
            }
        }

        if (speedometerNeedle == null && kamazContext != null)
        {
            speedometerNeedle = kamazContext.SpidometerNeedle;
        }

        if (tachometerNeedle == null && kamazContext != null)
        {
            tachometerNeedle = kamazContext.TachometerNeedle;
        }

        if (kamazContext != null)
        {
            if (wheelFrontLeft == null)
            {
                wheelFrontLeft = kamazContext.GetNode("wheelLF");
            }

            if (wheelFrontRight == null)
            {
                wheelFrontRight = kamazContext.GetNode("wheelRF");
            }

            if (wheelMiddle == null)
            {
                wheelMiddle = kamazContext.GetNode("wheel_midlle");
            }

            if (wheelBack == null)
            {
                wheelBack = kamazContext.GetNode("wheel_back");
            }
        }

        CacheWheelBaseRotations();
    }

    private void CacheWheelBaseRotations()
    {
        if (wheelFrontLeft != null)
        {
            wheelFrontLeftBaseLocalRotation = wheelFrontLeft.localRotation;
        }

        if (wheelFrontRight != null)
        {
            wheelFrontRightBaseLocalRotation = wheelFrontRight.localRotation;
        }

        if (wheelMiddle != null)
        {
            wheelMiddleBaseLocalRotation = wheelMiddle.localRotation;
        }

        if (wheelBack != null)
        {
            wheelBackBaseLocalRotation = wheelBack.localRotation;
        }

        wheelBaseRotationsCached = wheelFrontLeft != null || wheelFrontRight != null || wheelMiddle != null || wheelBack != null;
    }

    private Vector3 GetNormalizedAxis(Vector3 axis, Vector3 fallback)
    {
        if (axis.sqrMagnitude < 0.000001f)
        {
            return fallback;
        }

        return axis.normalized;
    }

    private void UpdateGaugeOutput()
    {
        if (speedometerNeedle != null)
        {
            speedometerNeedle.SetValue(speedKmh);
        }

        if (tachometerNeedle != null)
        {
            tachometerNeedle.SetValue(engineRpm);
        }
    }

    private void DrawDrivingHud()
    {
        float width = Mathf.Max(300f, hudWidth);
        float height = Mathf.Max(120f, hudHeight);
        float x = Mathf.Max(0f, hudLeftMargin);
        float y = Mathf.Max(0f, Screen.height - height - hudBottomMargin);

        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.alignment = TextAnchor.UpperLeft;
        boxStyle.fontSize = Mathf.Max(12, hudFontSize);
        boxStyle.normal.textColor = Color.white;
        boxStyle.padding = new RectOffset(12, 12, 10, 10);

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = Mathf.Max(12, hudFontSize);
        labelStyle.normal.textColor = Color.white;

        string engineText = IsEngineRunning() ? "Двигатель: ВКЛ" : "Двигатель: ВЫКЛ";
        string gearText = $"Передача: {GetGearLabel(currentGear)}";
        string rpmText = $"Обороты: {Mathf.RoundToInt(engineRpm)} RPM";
        string speedText = $"Скорость: {Mathf.RoundToInt(speedKmh)} км/ч";
        string clutchText = $"Сцепление: {Mathf.RoundToInt(clutchPedal * 100f)}%";
        string adviceText = $"Подсказка: {GetShiftAdvice()}";

        GUI.Box(new Rect(x, y, width, height), $"{engineText}\n{gearText}\n{speedText}\n{rpmText}\n{clutchText}\n{adviceText}", boxStyle);

        float helperY = y + height - 24f;
        GUI.Label(new Rect(x + 12f, helperY, width - 24f, 22f), "Повышение: зажми Left Shift и нажми следующую цифру (например 2, 3, 4...)", labelStyle);
    }

    private string GetGearLabel(int gear)
    {
        if (gear == 0)
        {
            return "N";
        }

        if (gear < 0)
        {
            return "R";
        }

        return gear.ToString();
    }

    private string GetShiftAdvice()
    {
        if (!IsEngineRunning())
        {
            return "Нажми Tab, чтобы запустить двигатель.";
        }

        if (currentGear == 0)
        {
            return "Включи 1 передачу: удерживай Left Shift + 1.";
        }

        if (currentGear < 0)
        {
            return "Задний ход включен.";
        }

        if (engineRpm >= recommendUpshiftRpm)
        {
            if (currentGear < 5)
            {
                return "Повышай передачу.";
            }

            return "Максимальная передача, держи газ аккуратно.";
        }

        if (engineRpm <= recommendDownshiftRpm && currentGear > 1)
        {
            return "Понижай передачу.";
        }

        if (engineRpm <= stallRpm + 150f && currentGear > 0)
        {
            return "Обороты низкие: добавь газ или выжми сцепление.";
        }

        return "Передача выбрана нормально.";
    }
}
