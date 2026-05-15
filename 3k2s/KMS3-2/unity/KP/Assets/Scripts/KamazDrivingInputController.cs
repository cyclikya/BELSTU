using UnityEngine;

// Управляет движением КамАЗа, передачами, оборотами, рулем и колесами.
public class KamazDrivingInputController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private KamazContext kamazContext;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private KamazAudioController kamazAudioController;
    [SerializeField] private Rigidbody kamazRigidbody;
    [SerializeField] private Transform steeringWheel;
    [SerializeField] private Transform wheelFrontLeft;
    [SerializeField] private Transform wheelFrontRight;
    [SerializeField] private Transform wheelMiddle;
    [SerializeField] private Transform wheelBack;
    [SerializeField] private GaugeNeedle speedometerNeedle;
    [SerializeField] private GaugeNeedle tachometerNeedle;

    [Header("Activation")]
    [SerializeField] private bool onlyWhenPlayerInCabin = true;
    [SerializeField] private bool requireEngineRunningForMotion = true;

    [Header("Input")]
    [SerializeField] private KeyCode throttleKey = KeyCode.W;
    [SerializeField] private KeyCode brakeKey = KeyCode.S;
    [SerializeField] private KeyCode brakeAltKey = KeyCode.Space;
    [SerializeField] private KeyCode steerLeftKey = KeyCode.A;
    [SerializeField] private KeyCode steerRightKey = KeyCode.D;
    [SerializeField] private KeyCode clutchKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode neutralKey = KeyCode.N;
    [SerializeField] private KeyCode gear1Key = KeyCode.Alpha1;
    [SerializeField] private KeyCode gear2Key = KeyCode.Alpha2;
    [SerializeField] private KeyCode gear3Key = KeyCode.Alpha3;
    [SerializeField] private KeyCode gear4Key = KeyCode.Alpha4;
    [SerializeField] private KeyCode gear5Key = KeyCode.Alpha5;
    [SerializeField] private KeyCode reverseKey = KeyCode.R;
    [SerializeField] private KeyCode hornKey = KeyCode.G;

    [Header("Clutch")]
    [SerializeField] private float clutchPressSpeed = 10f;
    [SerializeField] private float clutchReleaseDuration = 0.3f;
    [SerializeField] private float clutchRequiredForShift = 0.85f;

    [Header("Transmission")]
    [SerializeField] private int currentGear;
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
    [SerializeField] private float rpmDropSpeed = 350f;
    [SerializeField] private float clutchPressedRpmDropMultiplier = 0.15f;
    [SerializeField] private float stallCheckDelay = 0.6f;
    [SerializeField] private float stallClutchEngagement = 0.9f;
    [SerializeField] private float stallThrottleBypass = 0.3f;

    [Header("Movement")]
    [SerializeField] private float baseDriveForce = 26000f;
    [SerializeField] private float brakeForce = 15000f;
    [SerializeField] private float rollingDragForceCoast = 0.015f;
    [SerializeField] private float rollingDragForceThrottle = 0.008f;
    [SerializeField] private float engineBrakeForce = 3.5f;
    [SerializeField] private float engineBrakeThrottleThreshold = 0.05f;
    [SerializeField] private float maxReverseSpeedKmh = 15f;
    [SerializeField] private float lowRpmTorqueBoost = 2.6f;
    [SerializeField] private float lowRpmBoostEnd = 2300f;

    [Header("Vehicle Steering")]
    [SerializeField] private bool invertSteering;
    [SerializeField] private float steeringInputChangeSpeed = 1.4f;
    [SerializeField] private float minTurnRadiusAtFullSteer = 70f;
    [SerializeField] private float maxTurnRadiusAtLowSteer = 500f;
    [SerializeField] private float steeringMinSpeedMps = 0.8f;
    [SerializeField] private float steeringYawMultiplier = 0.75f;

    [Header("Steering Wheel")]
    [SerializeField] private float steeringWheelSpeed = 230f;
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

    [Header("Runtime")]
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
    private float wheelSpinAngle;
    private float frontWheelSteerVisualAngle;

    public int CurrentGear => currentGear;
    public float EngineRpm => engineRpm;
    public float SpeedKmh => speedKmh;

    private Quaternion steeringWheelStartRotation;
    private Quaternion wheelFrontLeftStartRotation;
    private Quaternion wheelFrontRightStartRotation;
    private Quaternion wheelMiddleStartRotation;
    private Quaternion wheelBackStartRotation;

    private void Awake()
    {
        if (kamazContext != null)
        {
            if (kamazAudioController == null) kamazAudioController = kamazContext.AudioController;
            if (kamazRigidbody == null) kamazRigidbody = kamazContext.KamazRigidbody;
            if (steeringWheel == null) steeringWheel = kamazContext.Ryle;
            if (wheelFrontLeft == null) wheelFrontLeft = kamazContext.WheelFrontLeft;
            if (wheelFrontRight == null) wheelFrontRight = kamazContext.WheelFrontRight;
            if (wheelMiddle == null) wheelMiddle = kamazContext.WheelMiddle;
            if (wheelBack == null) wheelBack = kamazContext.WheelBack;
            if (speedometerNeedle == null) speedometerNeedle = kamazContext.SpidometerNeedle;
            if (tachometerNeedle == null) tachometerNeedle = kamazContext.TachometerNeedle;
        }

        if (steeringWheel != null) steeringWheelStartRotation = steeringWheel.localRotation;
        if (wheelFrontLeft != null) wheelFrontLeftStartRotation = wheelFrontLeft.localRotation;
        if (wheelFrontRight != null) wheelFrontRightStartRotation = wheelFrontRight.localRotation;
        if (wheelMiddle != null) wheelMiddleStartRotation = wheelMiddle.localRotation;
        if (wheelBack != null) wheelBackStartRotation = wheelBack.localRotation;

        engineRpm = idleRpm;
        UpdateGaugeOutput();
    }

    private void Update()
    {
        bool canControl = !onlyWhenPlayerInCabin || IsPlayerInCabin();

        ReadInput(canControl);
        HandleGearInput(canControl);
        UpdateClutch(Time.deltaTime);
        UpdateSteeringWheel(Time.deltaTime);
        UpdateWheelVisuals(Time.deltaTime);
        UpdateEngineRpm(Time.deltaTime);
        UpdateGaugeOutput();
    }

    private void FixedUpdate()
    {
        if (kamazRigidbody == null)
        {
            return;
        }

        float forwardSpeed = GetForwardSpeedMps();
        speedKmh = GetVehicleGroundSpeedKmh();
        ApplyVehicleSteering(forwardSpeed);
        ApplyForces(forwardSpeed);
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

    private void ReadInput(bool canControl)
    {
        if (!canControl)
        {
            throttleInput = 0f;
            brakeInput = 0f;
            clutchTarget = 0f;
            return;
        }

        throttleInput = Input.GetKey(throttleKey) ? 1f : 0f;
        brakeInput = (Input.GetKey(brakeKey) || Input.GetKey(brakeAltKey)) ? 1f : 0f;

        float steerDirection = 0f;
        if (Input.GetKey(steerLeftKey)) steerDirection -= 1f;
        if (Input.GetKey(steerRightKey)) steerDirection += 1f;
        if (invertSteering) steerDirection *= -1f;

        steerInput = Mathf.Clamp(steerInput + steerDirection * steeringInputChangeSpeed * Time.deltaTime, -1f, 1f);
        clutchTarget = Input.GetKey(clutchKey) ? 1f : 0f;
    }

    private void HandleGearInput(bool canControl)
    {
        if (!canControl)
        {
            return;
        }

        if (Input.GetKeyDown(neutralKey)) TryShiftToGear(0);
        if (Input.GetKeyDown(gear1Key)) TryShiftToGear(1);
        if (Input.GetKeyDown(gear2Key)) TryShiftToGear(2);
        if (Input.GetKeyDown(gear3Key)) TryShiftToGear(3);
        if (Input.GetKeyDown(gear4Key)) TryShiftToGear(4);
        if (Input.GetKeyDown(gear5Key)) TryShiftToGear(5);
        if (Input.GetKeyDown(reverseKey)) TryShiftToGear(-1);

        if (kamazAudioController != null)
        {
            bool hornEnabled = IsPlayerInCabin() && Input.GetKey(hornKey);
            kamazAudioController.SetHornLoop(hornEnabled);
            kamazAudioController.SetReverseLoop(currentGear == -1);
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
            if (kamazAudioController != null)
            {
                kamazAudioController.PlayGearSwitch();
            }
            return;
        }

        if (clutchPedal >= clutchRequiredForShift)
        {
            currentGear = targetGear;
            if (kamazAudioController != null)
            {
                kamazAudioController.PlayGearSwitch();
            }
        }
    }

    private void UpdateClutch(float deltaTime)
    {
        float releaseSpeed = clutchReleaseDuration > 0f ? 1f / clutchReleaseDuration : 100f;
        float speed = clutchTarget > clutchPedal ? clutchPressSpeed : releaseSpeed;
        clutchPedal = Mathf.MoveTowards(clutchPedal, clutchTarget, speed * deltaTime);
    }

    private void UpdateSteeringWheel(float deltaTime)
    {
        if (steeringWheel == null)
        {
            return;
        }

        float targetAngle = Mathf.Lerp(steeringMinAngle, steeringMaxAngle, (steerInput + 1f) * 0.5f);
        wheelSteeringAngle = Mathf.MoveTowards(wheelSteeringAngle, targetAngle, steeringWheelSpeed * deltaTime);
        steeringWheel.localRotation = steeringWheelStartRotation * Quaternion.AngleAxis(wheelSteeringAngle, Vector3.up);
    }

    private void UpdateWheelVisuals(float deltaTime)
    {
        if (kamazRigidbody == null)
        {
            return;
        }

        float forwardSpeed = GetForwardSpeedMps();
        float absSpeed = Mathf.Abs(forwardSpeed);

        if (absSpeed >= minWheelSpinSpeedMps)
        {
            float circumference = 2f * Mathf.PI * Mathf.Max(0.01f, wheelRadiusMeters);
            float degreesPerSecond = absSpeed / circumference * 360f;
            wheelSpinAngle += Mathf.Sign(forwardSpeed) * degreesPerSecond * deltaTime;
            wheelSpinAngle = Mathf.Repeat(wheelSpinAngle, 360f);
        }

        float targetFrontSteer = (!onlyWhenPlayerInCabin || IsPlayerInCabin()) ? steerInput * frontWheelMaxSteerAngle : 0f;
        frontWheelSteerVisualAngle = Mathf.MoveTowards(frontWheelSteerVisualAngle, targetFrontSteer, frontWheelSteerSpeed * deltaTime);

        Quaternion spinRotation = Quaternion.AngleAxis(wheelSpinAngle, wheelSpinAxisLocal.normalized);
        Quaternion steerRotation = Quaternion.AngleAxis(frontWheelSteerVisualAngle, Vector3.forward);

        if (wheelFrontLeft != null) wheelFrontLeft.localRotation = wheelFrontLeftStartRotation * steerRotation * spinRotation;
        if (wheelFrontRight != null) wheelFrontRight.localRotation = wheelFrontRightStartRotation * steerRotation * spinRotation;
        if (wheelMiddle != null) wheelMiddle.localRotation = wheelMiddleStartRotation * spinRotation;
        if (wheelBack != null) wheelBack.localRotation = wheelBackStartRotation * spinRotation;
    }

    private void UpdateEngineRpm(float deltaTime)
    {
        if (IsEngineRunning() && engineStalled)
        {
            engineStalled = false;
            stallTimer = 0f;
            engineRpm = Mathf.Max(idleRpm, engineRpm);
        }

        if (!IsEngineRunning())
        {
            engineRpm = Mathf.MoveTowards(engineRpm, 0f, rpmDropSpeed * deltaTime);
            stallTimer = 0f;
            return;
        }

        float speedAbsKmh = Mathf.Abs(GetForwardSpeedMps()) * 3.6f;
        float freeRevTarget = Mathf.Lerp(idleRpm, maxRpm, throttleInput);
        float clutchEngagement = GetClutchEngagement();
        float coupledRpm = speedAbsKmh * GetRpmPerKmhForCurrentGear();

        float targetRpm = currentGear == 0
            ? freeRevTarget
            : Mathf.Lerp(freeRevTarget, Mathf.Max(idleRpm * 0.35f, coupledRpm), clutchEngagement);

        float speed = targetRpm > engineRpm ? rpmRiseSpeed : rpmDropSpeed;
        if (targetRpm < engineRpm && clutchPedal > 0.6f)
        {
            speed *= clutchPressedRpmDropMultiplier;
        }

        engineRpm = Mathf.MoveTowards(engineRpm, targetRpm, speed * deltaTime);
        engineRpm = Mathf.Clamp(engineRpm, 0f, maxRpm);

        bool canStall = currentGear != 0 && clutchEngagement >= stallClutchEngagement && throttleInput <= stallThrottleBypass;
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
        currentGear = 0;
        throttleInput = 0f;
        brakeInput = 0f;
        steerInput = 0f;
        clutchTarget = 0f;
        clutchPedal = 0f;
        wheelSteeringAngle = 0f;
        frontWheelSteerVisualAngle = 0f;

        if (steeringWheel != null)
        {
            steeringWheel.localRotation = steeringWheelStartRotation;
        }

        if (cameraController != null)
        {
            cameraController.HandleEngineStall();
        }
    }

    private void ApplyVehicleSteering(float forwardSpeed)
    {
        if (kamazRigidbody == null || Mathf.Abs(steerInput) < 0.001f)
        {
            return;
        }

        if (onlyWhenPlayerInCabin && !IsPlayerInCabin())
        {
            return;
        }

        float absSpeed = Mathf.Abs(forwardSpeed);
        if (absSpeed < steeringMinSpeedMps)
        {
            return;
        }

        float steerAmount = Mathf.Clamp01(Mathf.Abs(steerInput));
        float radius = Mathf.Lerp(maxTurnRadiusAtLowSteer, minTurnRadiusAtFullSteer, steerAmount);
        float yawRate = absSpeed / Mathf.Max(0.1f, radius);
        float direction = forwardSpeed >= 0f ? 1f : -1f;
        float yawDelta = yawRate * Mathf.Rad2Deg * Mathf.Sign(steerInput) * direction * Time.fixedDeltaTime * steeringYawMultiplier;

        Quaternion turn = Quaternion.AngleAxis(yawDelta, Vector3.forward);
        kamazRigidbody.MoveRotation(kamazRigidbody.rotation * turn);
    }

    private void ApplyForces(float forwardSpeed)
    {
        if (kamazRigidbody == null)
        {
            return;
        }

        Vector3 velocity = kamazRigidbody.linearVelocity;
        float massScale = Mathf.Max(1f, kamazRigidbody.mass / 1000f);
        float clutchEngagement = GetClutchEngagement();
        float drag = throttleInput > engineBrakeThrottleThreshold ? rollingDragForceThrottle : rollingDragForceCoast;
        if (clutchPedal > 0.6f)
        {
            drag *= 0.2f;
        }

        kamazRigidbody.AddForce(-velocity * (drag * massScale), ForceMode.Force);

        if (brakeInput > 0.001f)
        {
            kamazRigidbody.AddForce(-velocity.normalized * brakeForce * brakeInput, ForceMode.Force);
        }

        float gearRatio = GetCurrentGearRatio();
        Vector3 driveAxis = GetDriveDirectionWorld();

        if (currentGear != 0 && clutchEngagement > 0f && throttleInput <= engineBrakeThrottleThreshold)
        {
            float engineBrakeRpmFactor = Mathf.Clamp01(engineRpm / Mathf.Max(idleRpm, 1f));
            kamazRigidbody.AddForce(-driveAxis * forwardSpeed * engineBrakeForce * clutchEngagement * engineBrakeRpmFactor * Mathf.Abs(gearRatio) * massScale, ForceMode.Force);
        }

        if (requireEngineRunningForMotion && !IsEngineRunning())
        {
            return;
        }

        if (currentGear == 0 || throttleInput <= 0.001f)
        {
            return;
        }

        float speedAbsKmh = Mathf.Abs(forwardSpeed) * 3.6f;
        if (speedAbsKmh >= GetCurrentGearMaxSpeedKmh())
        {
            return;
        }

        if (gearRatio < 0f && speedAbsKmh >= maxReverseSpeedKmh)
        {
            return;
        }

        float rpmFactor = Mathf.Clamp01(engineRpm / maxRpm);
        float torqueFactor = Mathf.Lerp(0.55f, 1f, rpmFactor);
        if (engineRpm < lowRpmBoostEnd)
        {
            float lowRpmT = Mathf.Clamp01(engineRpm / lowRpmBoostEnd);
            torqueFactor *= Mathf.Lerp(lowRpmTorqueBoost, 1f, lowRpmT);
        }

        float driveForce = baseDriveForce * Mathf.Abs(gearRatio) * throttleInput * clutchEngagement * torqueFactor * massScale;
        Vector3 driveDirection = driveAxis * Mathf.Sign(gearRatio);
        kamazRigidbody.AddForce(driveDirection * driveForce, ForceMode.Force);
    }

    private float GetCurrentGearRatio()
    {
        if (currentGear == -1) return reverseGearRatio;
        if (currentGear == 1) return gear1Ratio;
        if (currentGear == 2) return gear2Ratio;
        if (currentGear == 3) return gear3Ratio;
        if (currentGear == 4) return gear4Ratio;
        if (currentGear == 5) return gear5Ratio;
        return 0f;
    }

    private float GetCurrentGearMaxSpeedKmh()
    {
        if (currentGear == -1) return reverseMaxSpeedKmh;
        if (currentGear == 1) return gear1MaxSpeedKmh;
        if (currentGear == 2) return gear2MaxSpeedKmh;
        if (currentGear == 3) return gear3MaxSpeedKmh;
        if (currentGear == 4) return gear4MaxSpeedKmh;
        if (currentGear == 5) return gear5MaxSpeedKmh;
        return 90f;
    }

    private float GetRpmPerKmhForCurrentGear()
    {
        float maxSpeed = GetCurrentGearMaxSpeedKmh();
        if (maxSpeed <= 0f)
        {
            return 0f;
        }

        return maxRpm / maxSpeed;
    }

    private float GetClutchEngagement()
    {
        return Mathf.Clamp01(1f - clutchPedal);
    }

    private float GetForwardSpeedMps()
    {
        Vector3 localVelocity = kamazRigidbody.transform.InverseTransformDirection(kamazRigidbody.linearVelocity);
        return -localVelocity.y;
    }

    private float GetVehicleGroundSpeedKmh()
    {
        Vector3 localVelocity = kamazRigidbody.transform.InverseTransformDirection(kamazRigidbody.linearVelocity);
        float planarSpeed = Mathf.Sqrt(localVelocity.x * localVelocity.x + localVelocity.y * localVelocity.y);
        return planarSpeed * 3.6f;
    }

    private Vector3 GetDriveDirectionWorld()
    {
        return kamazRigidbody.transform.TransformDirection(-Vector3.up).normalized;
    }

    private bool IsPlayerInCabin()
    {
        return cameraController != null && cameraController.IsDrivingMode;
    }

    private bool IsEngineRunning()
    {
        return cameraController != null && cameraController.IsEngineRunning;
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
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.alignment = TextAnchor.UpperLeft;
        boxStyle.fontSize = hudFontSize;
        boxStyle.normal.textColor = Color.white;
        boxStyle.padding = new RectOffset(12, 12, 10, 10);

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = hudFontSize;
        labelStyle.normal.textColor = Color.white;

        float x = hudLeftMargin;
        float y = Screen.height - hudHeight - hudBottomMargin;

        string text =
            $"Двигатель: {(IsEngineRunning() ? "ВКЛ" : "ВЫКЛ")}\n" +
            $"Передача: {GetGearLabel()}\n" +
            $"Скорость: {Mathf.RoundToInt(speedKmh)} км/ч\n" +
            $"Обороты: {Mathf.RoundToInt(engineRpm)} RPM\n" +
            $"Сцепление: {Mathf.RoundToInt(clutchPedal * 100f)}%\n" +
            $"Подсказка: {GetShiftAdvice()}";

        GUI.Box(new Rect(x, y, hudWidth, hudHeight), text, boxStyle);
        GUI.Label(new Rect(x + 12f, y + hudHeight - 24f, hudWidth - 24f, 22f), "Повышение: зажми Left Shift и нажми следующую цифру", labelStyle);
    }

    private string GetGearLabel()
    {
        if (currentGear == 0) return "N";
        if (currentGear < 0) return "R";
        return currentGear.ToString();
    }

    private string GetShiftAdvice()
    {
        if (!IsEngineRunning()) return "Нажми Tab, чтобы запустить двигатель.";
        if (currentGear == 0) return "Включи 1 передачу: удерживай Left Shift + 1.";
        if (currentGear < 0) return "Задний ход включен.";
        if (engineRpm >= recommendUpshiftRpm && currentGear < 5) return "Повышай передачу.";
        if (engineRpm <= recommendDownshiftRpm && currentGear > 1) return "Понижай передачу.";
        if (engineRpm <= stallRpm + 150f) return "Обороты низкие: добавь газ или выжми сцепление.";
        return "Передача выбрана нормально.";
    }
}


