using System;
using UnityEngine;

// Отвечает за пешее передвижение игрока и определение объекта под курсором.
[Serializable]
public class FreeMovementMode
{
    public enum InteractionType
    {
        None,
        Door,
        Steering,
        Key,
        Other
    }

    public struct InteractionResult
    {
        public InteractionType Type;
        public Transform HitObject;
        public Transform DoorRoot;
        public Animator DoorAnimator;
        public Transform KeyRoot;
        public Animator KeyAnimator;
    }

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 40f;
    [SerializeField] private float runSpeed = 60f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float gravity = -150f;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 20f;

    private CharacterController controller;
    private Camera playerCamera;
    private Transform playerTransform;
    private Vector3 velocity;
    private float verticalRotation;

    public float InteractionDistance => interactionDistance;

    public void Initialize(CharacterController targetController, Camera targetCamera, Transform targetTransform)
    {
        controller = targetController;
        playerCamera = targetCamera;
        playerTransform = targetTransform;
    }

    public void EnterMode()
    {
        if (controller != null)
        {
            controller.enabled = true;
        }
    }

    public void ExitMode()
    {
        velocity = Vector3.zero;
    }

    public void Tick()
    {
        if (controller == null || playerCamera == null || playerTransform == null || !controller.enabled)
        {
            return;
        }

        HandleLook();
        HandleMovement();
    }

    public void TickLookOnly()
    {
        if (playerCamera == null || playerTransform == null)
        {
            return;
        }

        HandleLook();
    }

    public bool TryGetInteraction(KamazContext kamazContext, out InteractionResult result)
    {
        result = new InteractionResult();
        if (playerCamera == null || kamazContext == null)
        {
            return false;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit[] hits = Physics.RaycastAll(ray, interactionDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
        if (hits.Length == 0)
        {
            return false;
        }

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            Collider hitCollider = hits[i].collider;
            result.HitObject = hits[i].transform;

            if (kamazContext.TryGetDoorFromCollider(hitCollider, out Transform doorRoot, out Animator doorAnimator))
            {
                result.Type = InteractionType.Door;
                result.DoorRoot = doorRoot;
                result.DoorAnimator = doorAnimator;
                return true;
            }

            if (kamazContext.IsRyleCollider(hitCollider))
            {
                result.Type = InteractionType.Steering;
                return true;
            }

            if (kamazContext.TryGetKeyFromCollider(hitCollider, out Transform keyRoot, out Animator keyAnimator))
            {
                result.Type = InteractionType.Key;
                result.KeyRoot = keyRoot;
                result.KeyAnimator = keyAnimator;
                return true;
            }
        }

        result.Type = InteractionType.Other;
        result.HitObject = hits[0].transform;
        return true;
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        playerTransform.Rotate(Vector3.up * mouseX);
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.A)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontal += 1f;
        if (Input.GetKey(KeyCode.W)) vertical += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;

        Vector3 move = (playerTransform.right * horizontal + playerTransform.forward * vertical).normalized;
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
