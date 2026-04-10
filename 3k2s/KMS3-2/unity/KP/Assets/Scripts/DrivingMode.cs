using System;
using UnityEngine;

[Serializable]
public class DrivingMode
{
    [SerializeField] private float exitForwardOffset = 0.75f;
    [SerializeField] private float exitUpOffset = 0.05f;

    private CharacterController controller;
    private Transform playerTransform;
    private bool isActive;
    private KamazContext kamazContext;

    public void Initialize(CharacterController targetController, Transform targetTransform, KamazContext context)
    {
        controller = targetController;
        playerTransform = targetTransform;
        kamazContext = context;
    }

    public void SetContext(KamazContext context)
    {
        kamazContext = context;
    }

    public void EnterMode()
    {
        ResolveContextIfNeeded();
        Transform seatPoint = kamazContext != null ? kamazContext.SeatPoint : null;

        if (playerTransform == null)
        {
            return;
        }

        if (seatPoint == null)
        {
            return;
        }

        TeleportTo(seatPoint);
        isActive = true;

        if (controller != null)
        {
            controller.enabled = false;
        }
    }

    public void ExitMode(bool keepControllerDisabled = false)
    {
        ResolveContextIfNeeded();
        Transform exitPoint = kamazContext != null ? kamazContext.ExitPoint : null;

        if (playerTransform == null)
        {
            return;
        }

        if (!isActive)
        {
            SetCharacterControllerEnabled(!keepControllerDisabled);

            return;
        }

        if (exitPoint != null)
        {
            TeleportToExitPoint();
        }

        SetCharacterControllerEnabled(!keepControllerDisabled);

        isActive = false;
    }

    public void Tick()
    {
        // Заглушка режима "за рулем": логика управления КамАЗом будет добавлена позже.
    }

    private void TeleportTo(Transform targetPoint)
    {
        bool wasEnabled = controller != null && controller.enabled;

        if (wasEnabled)
        {
            controller.enabled = false;
        }

        playerTransform.SetPositionAndRotation(targetPoint.position, targetPoint.rotation);

        if (wasEnabled)
        {
            controller.enabled = true;
        }
    }

    private void TeleportToExitPoint()
    {
        ResolveContextIfNeeded();
        Transform exitPoint = kamazContext != null ? kamazContext.ExitPoint : null;
        if (exitPoint == null)
        {
            return;
        }

        bool wasEnabled = controller != null && controller.enabled;

        if (wasEnabled)
        {
            controller.enabled = false;
        }

        Vector3 targetPosition = exitPoint.position + exitPoint.forward * exitForwardOffset + Vector3.up * exitUpOffset;
        playerTransform.SetPositionAndRotation(targetPosition, exitPoint.rotation);

        if (wasEnabled)
        {
            controller.enabled = true;
        }
    }

    public void SetCharacterControllerEnabled(bool value)
    {
        if (controller != null)
        {
            controller.enabled = value;
        }
    }

    private void ResolveContextIfNeeded()
    {
        if (kamazContext == null)
        {
            kamazContext = KamazContext.Instance;
        }
    }
}
