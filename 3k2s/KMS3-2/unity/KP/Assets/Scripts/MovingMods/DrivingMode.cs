using System;
using UnityEngine;

// Отвечает за посадку игрока в кабину и выход из нее.
[Serializable]
public class DrivingMode
{
    [SerializeField] private float exitForwardOffset = 0.75f;
    [SerializeField] private float exitUpOffset = 0.05f;

    private CharacterController controller;
    private Transform playerTransform;
    private KamazContext kamazContext;
    private Transform originalParent;
    private bool isActive;

    public void Initialize(CharacterController targetController, Transform targetTransform, KamazContext context)
    {
        controller = targetController;
        playerTransform = targetTransform;
        kamazContext = context;
        originalParent = targetTransform != null ? targetTransform.parent : null;
    }

    public void SetContext(KamazContext context)
    {
        kamazContext = context;
    }

    public void EnterMode()
    {
        if (playerTransform == null || kamazContext == null || kamazContext.SeatPoint == null)
        {
            return;
        }

        SetCharacterControllerEnabled(false);
        playerTransform.SetParent(kamazContext.SeatPoint, false);
        playerTransform.localPosition = Vector3.zero;
        playerTransform.localRotation = Quaternion.identity;
        isActive = true;
    }

    public void ExitMode(bool keepControllerDisabled = false)
    {
        if (playerTransform == null)
        {
            return;
        }

        SetCharacterControllerEnabled(false);
        playerTransform.SetParent(originalParent, true);

        if (isActive && kamazContext != null && kamazContext.ExitPoint != null)
        {
            Vector3 targetPosition = kamazContext.ExitPoint.position + kamazContext.ExitPoint.forward * exitForwardOffset + Vector3.up * exitUpOffset;
            playerTransform.SetPositionAndRotation(targetPosition, kamazContext.ExitPoint.rotation);
        }

        isActive = false;
        SetCharacterControllerEnabled(!keepControllerDisabled);
    }

    public void Tick()
    {
    }

    public void SetCharacterControllerEnabled(bool value)
    {
        if (controller != null)
        {
            controller.enabled = value;
        }
    }
}
