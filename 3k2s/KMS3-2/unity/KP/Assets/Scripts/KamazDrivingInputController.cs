using UnityEngine;

public class KamazDrivingInputController : MonoBehaviour
{
    [Header("References")]
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

    [Header("Input")]
    [SerializeField] private KeyCode throttleKey = KeyCode.W;        // Газ.
    [SerializeField] private KeyCode brakeKey = KeyCode.S;           // Тормоз.
    [SerializeField] private KeyCode brakeAltKey = KeyCode.Space;    // Дополнительный тормоз.
    [SerializeField] private KeyCode steerLeftKey = KeyCode.A;       // Поворот влево.
    [SerializeField] private KeyCode steerRightKey = KeyCode.D;      // Поворот вправо.
    [SerializeField] private KeyCode clutchKey = KeyCode.LeftShift;  // Сцепление.
    [SerializeField] private KeyCode neutralKey = KeyCode.N;         // Нейтраль.
    [SerializeField] private KeyCode gear1Key = KeyCode.Alpha1;      // 1-я передача.
    [SerializeField] private KeyCode gear2Key = KeyCode.Alpha2;      // 2-я передача.
    [SerializeField] private KeyCode gear3Key = KeyCode.Alpha3;      // 3-я передача.
    [SerializeField] private KeyCode gear4Key = KeyCode.Alpha4;      // 4-я передача.
    [SerializeField] private KeyCode gear5Key = KeyCode.Alpha5;      // 5-я передача.
    [SerializeField] private KeyCode reverseKey = KeyCode.R;         // Задний ход.
    [SerializeField] private KeyCode hornKey = KeyCode.G;            // Звуковой сигнал.

    [Header("Clutch")]
    [SerializeField] private float clutchPressSpeed = 10f;

    [SerializeField] private float clutchReleaseDuration = 0.3f;

    // Минимальное значение сцепления, при котором разрешено включать передачу.
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
    [SerializeField] private float idleRpm = 650f;       // Холостые обороты.
    [SerializeField] private float maxRpm = 3000f;       // Максимальные обороты.
    [SerializeField] private float stallRpm = 450f;      // Обороты, ниже которых двигатель может заглохнуть.
    [SerializeField] private float rpmRiseSpeed = 1400f; // Скорость роста оборотов.
    [SerializeField] private float rpmDropSpeed = 350f;  // Скорость падения оборотов.

    // При выжатом сцеплении обороты падают медленнее, чтобы движение выглядело плавнее.
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
    [SerializeField] private float wheelSpinSpeed = 120f;
    [SerializeField] private float turnSpeed = 10f;

    [Header("Vehicle Steering")]
    [SerializeField] private bool invertSteering;
    [SerializeField] private float steeringInputChangeSpeed = 1.4f;
    [SerializeField] private float minTurnRadiusAtFullSteer = 70f;
    [SerializeField] private float maxTurnRadiusAtLowSteer = 500f;
    [SerializeField] private float steeringMinSpeedMps = 0.8f;
    [SerializeField] private float steeringYawMultiplier = 0.75f;

    [Header("Visuals")]
    [SerializeField] private float steeringWheelSpeed = 230f;
    [SerializeField] private float steeringMinAngle = -110f;
    [SerializeField] private float steeringMaxAngle = 110f;
    [SerializeField] private float wheelRadiusMeters = 0.55f;
    [SerializeField] private float minWheelSpinSpeedMps = 0.01f;
    [SerializeField] private float frontWheelMaxSteerAngle = 28f;
    [SerializeField] private float frontWheelSteerSpeed = 140f;
    [SerializeField] private Vector3 wheelSpinAxisLocal = Vector3.right;

    [Header("Runtime")]
    [SerializeField] private float engineRpm;
    [SerializeField] private float clutchPedal;
    [SerializeField] private float speedKmh;
    [SerializeField] private bool engineStalled;

    private float steerInput;      // Значение руля от -1 до 1.
    private float throttleInput;   // Газ: 0 или 1.
    private float brakeInput;      // Тормоз: 0 или 1.
    private float clutchTarget;    // Целевое значение сцепления.

    private float wheelSteeringAngle;
    private float wheelSpinAngle;
    private float frontWheelSteerVisualAngle;

    private float stallTimer;

    private Quaternion steeringWheelStartRotation;
    private Quaternion wheelFrontLeftStartRotation;
    private Quaternion wheelFrontRightStartRotation;
    private Quaternion wheelMiddleStartRotation;
    private Quaternion wheelBackStartRotation;

    public int CurrentGear => currentGear;
    public float EngineRpm => engineRpm;
    public float SpeedKmh => speedKmh;

    private void Awake()
    {
        steeringWheelStartRotation = steeringWheel.localRotation;
        wheelFrontLeftStartRotation = wheelFrontLeft.localRotation;
        wheelFrontRightStartRotation = wheelFrontRight.localRotation;
        wheelMiddleStartRotation = wheelMiddle.localRotation;
        wheelBackStartRotation = wheelBack.localRotation;

        engineRpm = idleRpm;
        UpdateGaugeOutput();
    }

    private void Update()
    {
        bool canControl = cameraController.IsDrivingMode;

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
        float forwardSpeed = GetForwardSpeedMps();

        speedKmh = GetVehicleGroundSpeedKmh();

        ApplyVehicleSteering(forwardSpeed);
        ApplyForces(forwardSpeed);
        UpdateGaugeOutput();
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
        brakeInput = Input.GetKey(brakeKey) || Input.GetKey(brakeAltKey) ? 1f : 0f;

        float steerDirection = 0f;
        if (Input.GetKey(steerLeftKey)) steerDirection -= 1f;
        if (Input.GetKey(steerRightKey)) steerDirection += 1f;
        if (invertSteering) steerDirection *= -1f;

        if (steerDirection != 0f)
        {
            steerInput = Mathf.Clamp(
                steerInput + steerDirection * steeringInputChangeSpeed * Time.deltaTime,
                -1f,
                1f
            );
        }

        clutchTarget = Input.GetKey(clutchKey) ? 1f : 0f;
    }

    private void HandleGearInput(bool canControl)
    {
        if (!canControl) return;

        if (Input.GetKeyDown(neutralKey)) TryShiftToGear(0);
        else if (Input.GetKeyDown(gear1Key)) TryShiftToGear(1);
        else if (Input.GetKeyDown(gear2Key)) TryShiftToGear(2);
        else if (Input.GetKeyDown(gear3Key)) TryShiftToGear(3);
        else if (Input.GetKeyDown(gear4Key)) TryShiftToGear(4);
        else if (Input.GetKeyDown(gear5Key)) TryShiftToGear(5);
        else if (Input.GetKeyDown(reverseKey)) TryShiftToGear(-1);

        kamazAudioController.SetHornLoop(Input.GetKey(hornKey));

        kamazAudioController.SetReverseLoop(currentGear == -1);
    }

    private void TryShiftToGear(int targetGear)
    {
        if (targetGear == currentGear) return;

        if (targetGear != 0 && clutchPedal < clutchRequiredForShift) return;

        currentGear = targetGear;
        kamazAudioController.PlayGearSwitch();
    }

    private void UpdateClutch(float deltaTime)
    {
        // Плавное переключение сцепления
        float releaseSpeed = 1f / clutchReleaseDuration;
        float speed = clutchTarget > clutchPedal ? clutchPressSpeed : releaseSpeed;

        clutchPedal = Mathf.MoveTowards(clutchPedal, clutchTarget, speed * deltaTime);
    }

    private void UpdateSteeringWheel(float deltaTime)
    {
        // Переводим steerInput из диапазона -1...1 в угол поворота руля
        float targetAngle = Mathf.Lerp(steeringMinAngle, steeringMaxAngle, (steerInput + 1f) * 0.5f);

        wheelSteeringAngle = Mathf.MoveTowards(wheelSteeringAngle, targetAngle, steeringWheelSpeed * deltaTime);
        steeringWheel.localRotation = steeringWheelStartRotation * Quaternion.AngleAxis(wheelSteeringAngle, Vector3.up);
    }

    private void UpdateWheelVisuals(float deltaTime)
    {
        float forwardSpeed = GetForwardSpeedMps();

        wheelSpinAngle += forwardSpeed * wheelSpinSpeed * deltaTime;

        Quaternion spin = Quaternion.AngleAxis(wheelSpinAngle, wheelSpinAxisLocal);
        Quaternion steer = Quaternion.AngleAxis(steerInput * frontWheelMaxSteerAngle, Vector3.forward);

        wheelFrontLeft.localRotation = wheelFrontLeftStartRotation * steer * spin;
        wheelFrontRight.localRotation = wheelFrontRightStartRotation * steer * spin;

        wheelMiddle.localRotation = wheelMiddleStartRotation * spin;
        wheelBack.localRotation = wheelBackStartRotation * spin;
    }
    private void UpdateEngineRpm(float deltaTime)
    {
        if (!cameraController.IsEngineRunning)
        {
            engineRpm = Mathf.MoveTowards(engineRpm, 0f, rpmDropSpeed * deltaTime);
            stallTimer = 0f;
            return;
        }

        if (engineStalled)
        {
            engineStalled = false;
            engineRpm = idleRpm;
        }

        float targetRpm = Mathf.Lerp(idleRpm, maxRpm, throttleInput);

        // Синхронизыция со скоростью
        if (currentGear != 0)
        {
            float speedKmh = Mathf.Abs(GetForwardSpeedMps()) * 3.6f;
            float speedFactor = Mathf.Clamp01(speedKmh / GetCurrentGearMaxSpeedKmh());

            float rpmFromSpeed = Mathf.Lerp(idleRpm, maxRpm, speedFactor);

            targetRpm = Mathf.Max(targetRpm, rpmFromSpeed);
        }
 
        if (currentGear != 0 &&
            clutchPedal < 0.2f &&
            throttleInput < stallThrottleBypass &&
            Mathf.Abs(GetForwardSpeedMps()) < 1f)
        {
            targetRpm = 0f;
        }

        float rpmSpeed = targetRpm > engineRpm ? rpmRiseSpeed : rpmDropSpeed;

        engineRpm = Mathf.MoveTowards(
            engineRpm,
            targetRpm,
            rpmSpeed * deltaTime
        );

        if (badStart && engineRpm < stallRpm)
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
        steeringWheel.localRotation = steeringWheelStartRotation;

        cameraController.HandleEngineStall();
    }

    private void ApplyVehicleSteering(float forwardSpeed)
    {
        if (!cameraController.IsDrivingMode) return;

        if (Mathf.Abs(forwardSpeed) < steeringMinSpeedMps) return;

        float direction = forwardSpeed >= 0f ? 1f : -1f;

        float turnAngle = steerInput * turnSpeed * direction * Time.fixedDeltaTime;

        kamazRigidbody.MoveRotation(
            kamazRigidbody.rotation * Quaternion.AngleAxis(turnAngle, Vector3.forward)
        );
    }

    private void ApplyForces(float forwardSpeed)
    {
        Vector3 velocity = kamazRigidbody.linearVelocity;

        // Учитываем массу
        float massScale = Mathf.Max(1f, kamazRigidbody.mass / 1000f);

        // Сопротивление
        kamazRigidbody.AddForce(
            -velocity * rollingDragForceCoast * massScale,
            ForceMode.Force
        );

        if (brakeInput > 0f && velocity.sqrMagnitude > 0.01f)
        {
            kamazRigidbody.AddForce(
                -velocity.normalized * brakeForce * brakeInput * massScale,
                ForceMode.Force
            );
        }

        if (!cameraController.IsEngineRunning) return;

        if (currentGear == 0) return;

        if (throttleInput <= 0f) return;

        float speedKmh = Mathf.Abs(forwardSpeed) * 3.6f;
        if (speedKmh >= GetCurrentGearMaxSpeedKmh()) return;

        float gearRatio = GetCurrentGearRatio();

        float clutchEngagement = GetClutchEngagement();

        if (clutchEngagement <= 0f) return;

        // Сила тяги
        float driveForce =
            baseDriveForce *
            Mathf.Abs(gearRatio) *
            throttleInput *
            clutchEngagement *
            massScale;

        kamazRigidbody.AddForce(
            GetDriveDirectionWorld() * Mathf.Sign(gearRatio) * driveForce,
            ForceMode.Force
        );
    }
    
    // Возвращает передаточное число текущей передачи.
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
        return gear5MaxSpeedKmh;
    }

    // Связь оборотов и скорости
    private float GetRpmPerKmhForCurrentGear()
    {
        float maxSpeed = GetCurrentGearMaxSpeedKmh();
        return maxRpm / maxSpeed;
    }

    // Возвращает выжато ли сцепление   
    private float GetClutchEngagement()
    {
        return Mathf.Clamp01(1f - clutchPedal);
    }

    // Берем скорость Rigidbody в мировых координатах и переводим ее в локальные координаты КАМАЗа.
    private float GetForwardSpeedMps()
    {
        Vector3 localVelocity = kamazRigidbody.transform.InverseTransformDirection(kamazRigidbody.linearVelocity);
        return -localVelocity.y;
    }

    // Считаем скорость по плоскости движения
    private float GetVehicleGroundSpeedKmh()
    {
        Vector3 localVelocity = kamazRigidbody.transform.InverseTransformDirection(kamazRigidbody.linearVelocity);
        float planarSpeed = Mathf.Sqrt(localVelocity.x * localVelocity.x + localVelocity.y * localVelocity.y);
        return planarSpeed * 3.6f;
    }

    // Возвращает направление, в котором должна прикладываться сила тяги
    private Vector3 GetDriveDirectionWorld()
    {
        return kamazRigidbody.transform.TransformDirection(-Vector3.up).normalized;
    }

    // Передаем рассчитанные значения на стрелки приборной панели.
    private void UpdateGaugeOutput()
    {
        speedometerNeedle.SetValue(speedKmh);
        tachometerNeedle.SetValue(engineRpm);
    }
}