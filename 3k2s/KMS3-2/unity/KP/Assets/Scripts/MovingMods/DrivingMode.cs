using System;
using UnityEngine;

// Сажает игрока в кабину и возвращает обратно при выходе.
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

    // Запоминает ссылки, с которыми режим вождения будет работать.
    public void Initialize(CharacterController targetController, Transform targetTransform, KamazContext context)
    {
        controller = targetController;
        playerTransform = targetTransform;
        kamazContext = context;
        originalParent = targetTransform != null ? targetTransform.parent : null;
    }

    // Позволяет обновить контекст, если он сменился.
    public void SetContext(KamazContext context)
    {
        kamazContext = context;
    }

    // Сажает игрока в точку seatPoint внутри КамАЗа.
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

    // Возвращает игрока в мир рядом с дверью выхода.
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
            Vector3 exitPosition = kamazContext.ExitPoint.position + kamazContext.ExitPoint.forward * exitForwardOffset + Vector3.up * exitUpOffset;
            playerTransform.SetPositionAndRotation(exitPosition, kamazContext.ExitPoint.rotation);
        }

        isActive = false;
        SetCharacterControllerEnabled(!keepControllerDisabled);
    }

    // Отдельной логики по кадрам в этом режиме нет.
    public void Tick()
    {
    }

    // Включает или выключает CharacterController игрока.
    public void SetCharacterControllerEnabled(bool value)
    {
        if (controller != null)
        {
            controller.enabled = value;
        }
    }
}
