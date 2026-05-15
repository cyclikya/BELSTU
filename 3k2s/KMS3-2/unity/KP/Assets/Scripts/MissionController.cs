using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Controls the mission flow on the practice scene without owning vehicle logic.
public class MissionController : MonoBehaviour
{
    public enum MissionStage
    {
        GetInTruck,
        StartEngine,
        RevEngine,
        ShiftToFirst,
        UseSteering,
        DriveToForwardCheckpoint,
        ShiftToReverse,
        UseMiniCamera,
        DriveToReverseCheckpoint,
        ShiftToNeutral,
        StopEngine,
        RaiseBody,
        UnloadRock,
        Completed
    }

    [Header("References")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private KamazDrivingInputController drivingController;
    [SerializeField] private KamazCabinMechanismsController cabinMechanismsController;
    [SerializeField] private Transform kamazRoot;
    [SerializeField] private MissionUI missionUI;
    [SerializeField] private MissionTriggerZone forwardCheckpointZone;
    [SerializeField] private MissionTriggerZone reverseCheckpointZone;
    [SerializeField] private MissionTriggerZone unloadPitZone;

    [Header("Scenes")]
    [SerializeField] private int setupSceneBuildIndex;

    [Header("Thresholds")]
    [SerializeField] private float rpmThresholdMin = 1400f;

    private MissionStage currentStage = MissionStage.GetInTruck;
    private bool missionCompleted;
    private bool timerStarted;
    private float missionTimer;
    private float maxSpeedKmh;
    private int maxForwardGear;

    public MissionStage CurrentStage => currentStage;

    private void Start()
    {
        if (missionUI != null)
        {
            missionUI.SetController(this);
            missionUI.HideResults();
            missionUI.SetObjective(GetStageText(currentStage));
        }

        RefreshZoneStates();
    }

    private void Update()
    {
        if (missionCompleted)
        {
            return;
        }

        if (timerStarted)
        {
            missionTimer += Time.deltaTime;
        }

        UpdateStatistics();
        UpdateStageState();
    }

    public void HandleZoneEntered(MissionTriggerZone.ZoneType zoneType, Collider other)
    {
        if (missionCompleted || other == null)
        {
            return;
        }

        if (zoneType == MissionTriggerZone.ZoneType.UnloadPit)
        {
            if (currentStage == MissionStage.UnloadRock && other.CompareTag("rock"))
            {
                CompleteMission();
            }

            return;
        }

        if (!IsKamazCollider(other))
        {
            return;
        }

        if (zoneType == MissionTriggerZone.ZoneType.ForwardCheckpoint && currentStage == MissionStage.DriveToForwardCheckpoint)
        {
            AdvanceStage(MissionStage.ShiftToReverse);
            return;
        }

        if (zoneType == MissionTriggerZone.ZoneType.ReverseCheckpoint && currentStage == MissionStage.DriveToReverseCheckpoint)
        {
            AdvanceStage(MissionStage.ShiftToNeutral);
        }
    }

    public void OpenResultsTable()
    {
        if (missionUI == null)
        {
            return;
        }

        missionUI.ShowResults(MissionResultsStorage.LoadResults());
    }

    public void CloseResultsTable()
    {
        if (missionUI != null)
        {
            missionUI.HideResults();
        }
    }

    public void RestartMission()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void LoadSetupScene()
    {
        SceneManager.LoadScene(setupSceneBuildIndex);
    }

    private void UpdateStatistics()
    {
        if (drivingController == null)
        {
            return;
        }

        if (!timerStarted && cameraController != null && cameraController.IsEngineRunning)
        {
            timerStarted = true;
        }

        if (!timerStarted)
        {
            return;
        }

        maxSpeedKmh = Mathf.Max(maxSpeedKmh, drivingController.SpeedKmh);
        if (drivingController.CurrentGear > maxForwardGear)
        {
            maxForwardGear = drivingController.CurrentGear;
        }
    }

    private void UpdateStageState()
    {
        switch (currentStage)
        {
            case MissionStage.GetInTruck:
                if (cameraController != null && cameraController.IsDrivingMode)
                {
                    AdvanceStage(MissionStage.StartEngine);
                }
                break;

            case MissionStage.StartEngine:
                if (cameraController != null && cameraController.IsEngineRunning)
                {
                    AdvanceStage(MissionStage.RevEngine);
                }
                break;

            case MissionStage.RevEngine:
                if (drivingController != null && drivingController.EngineRpm >= rpmThresholdMin)
                {
                    AdvanceStage(MissionStage.ShiftToFirst);
                }
                break;

            case MissionStage.ShiftToFirst:
                if (drivingController != null && drivingController.CurrentGear == 1)
                {
                    AdvanceStage(MissionStage.UseSteering);
                }
                break;

            case MissionStage.UseSteering:
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                {
                    AdvanceStage(MissionStage.DriveToForwardCheckpoint);
                }
                break;

            case MissionStage.ShiftToReverse:
                if (drivingController != null && drivingController.CurrentGear == -1)
                {
                    AdvanceStage(MissionStage.UseMiniCamera);
                }
                break;

            case MissionStage.UseMiniCamera:
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) ||
                    Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    AdvanceStage(MissionStage.DriveToReverseCheckpoint);
                }
                break;

            case MissionStage.ShiftToNeutral:
                if (drivingController != null && drivingController.CurrentGear == 0)
                {
                    AdvanceStage(MissionStage.StopEngine);
                }
                break;

            case MissionStage.StopEngine:
                if (cameraController != null && !cameraController.IsEngineRunning)
                {
                    AdvanceStage(MissionStage.RaiseBody);
                }
                break;

            case MissionStage.RaiseBody:
                if (cabinMechanismsController != null && cabinMechanismsController.IsBodyRaised)
                {
                    AdvanceStage(MissionStage.UnloadRock);
                }
                break;
        }
    }

    private void AdvanceStage(MissionStage nextStage)
    {
        currentStage = nextStage;
        RefreshZoneStates();
        if (missionUI != null)
        {
            missionUI.SetObjective(GetStageText(currentStage));
        }
    }

    private void CompleteMission()
    {
        missionCompleted = true;
        currentStage = MissionStage.Completed;

        MissionResultRecord result = new MissionResultRecord
        {
            timeSeconds = missionTimer,
            maxSpeedKmh = maxSpeedKmh,
            maxForwardGear = Mathf.Max(0, maxForwardGear)
        };

        MissionResultsStorage.AddResult(result);

        if (missionUI != null)
        {
            missionUI.SetObjective(GetStageText(currentStage));
            missionUI.ShowResults(MissionResultsStorage.LoadResults());
        }

        RefreshZoneStates();
    }

    private bool IsKamazCollider(Collider other)
    {
        if (kamazRoot == null || other == null)
        {
            return false;
        }

        return other.transform.root == kamazRoot;
    }

    private string GetStageText(MissionStage stage)
    {
        switch (stage)
        {
            case MissionStage.GetInTruck:
                return "Сядьте в КамАЗ.";
            case MissionStage.StartEngine:
                return "Нажмите Tab, чтобы завести двигатель.";
            case MissionStage.RevEngine:
                return "Зажмите W, чтобы поднять обороты двигателя.";
            case MissionStage.ShiftToFirst:
                return "Не отпуская W, зажмите Left Shift, нажмите 1 и отпустите сцепление, чтобы включить первую передачу.";
            case MissionStage.UseSteering:
                return "Попробуйте руление: нажмите A или D. Тормоз: Space.";
            case MissionStage.DriveToForwardCheckpoint:
                return "Переключайте передачи и доедьте до контрольной точки.";
            case MissionStage.ShiftToReverse:
                return "Зажмите Left Shift и включите заднюю передачу: R.";
            case MissionStage.UseMiniCamera:
                return "Используйте стрелки, чтобы управлять миникамерой.";
            case MissionStage.DriveToReverseCheckpoint:
                return "Доедьте задним ходом до следующей контрольной точки.";
            case MissionStage.ShiftToNeutral:
                return "Отпустите W, включите нейтральную передачу: N. Сцепление не обязательно.";
            case MissionStage.StopEngine:
                return "Нажмите Tab, чтобы заглушить двигатель.";
            case MissionStage.RaiseBody:
                return "Поднимите кузов клавишей B.";
            case MissionStage.UnloadRock:
                return "Разгрузите щебень в яму.";
            case MissionStage.Completed:
                return "Миссия выполнена.";
            default:
                return string.Empty;
        }
    }

    private void RefreshZoneStates()
    {
        if (forwardCheckpointZone != null)
        {
            bool active = currentStage == MissionStage.DriveToForwardCheckpoint;
            forwardCheckpointZone.SetState(active, active);
        }

        if (reverseCheckpointZone != null)
        {
            bool active = currentStage == MissionStage.DriveToReverseCheckpoint;
            reverseCheckpointZone.SetState(active, active);
        }

        if (unloadPitZone != null)
        {
            bool active = currentStage == MissionStage.UnloadRock;
            unloadPitZone.SetState(active, false);
        }
    }
}
