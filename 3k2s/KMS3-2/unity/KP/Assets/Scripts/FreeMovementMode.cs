using System;
using UnityEngine;

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

    [Header("Движение")]
    [SerializeField] private float walkSpeed = 40f;
    [SerializeField] private float runSpeed = 60f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float gravity = -150f;

    [Header("Поворот камеры")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("Взаимодействие")]
    [SerializeField] private float interactionDistance = 20f;

    public float InteractionDistance => interactionDistance;

    private CharacterController controller;
    private Camera playerCamera;
    private Transform playerTransform;
    private Vector3 velocity;
    private float verticalRotation;

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
        if (controller == null || playerCamera == null || playerTransform == null)
        {
            return;
        }

        if (!controller.enabled)
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
        result = new InteractionResult
        {
            Type = InteractionType.None,
            HitObject = null,
            DoorRoot = null,
            DoorAnimator = null,
            KeyRoot = null,
            KeyAnimator = null
        };

        if (playerCamera == null)
        {
            return false;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            interactionDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide
        );
        if (hits == null || hits.Length == 0)
        {
            return false;
        }

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        Transform firstHit = hits[0].transform;

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (kamazContext != null && TryResolveKnownInteraction(kamazContext, hit.transform, hit.collider, out result))
            {
                return true;
            }
        }

        result.HitObject = firstHit;
        result.Type = InteractionType.Other;
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

        if (Input.GetKey(KeyCode.A))
        {
            horizontal -= 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontal += 1f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            vertical += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            vertical -= 1f;
        }

        Vector3 move = playerTransform.right * horizontal + playerTransform.forward * vertical;
        move = move.normalized;
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private bool TryResolveKnownInteraction(KamazContext kamazContext, Transform hitTransform, Collider hitCollider, out InteractionResult result)
    {
        result = new InteractionResult
        {
            Type = InteractionType.None,
            HitObject = hitTransform,
            DoorRoot = null,
            DoorAnimator = null,
            KeyRoot = null,
            KeyAnimator = null
        };

        if (kamazContext.TryGetDoorFromCollider(hitCollider, out Transform doorByCollider, out Animator doorAnimatorByCollider))
        {
            result.Type = InteractionType.Door;
            result.DoorRoot = doorByCollider;
            result.DoorAnimator = doorAnimatorByCollider;
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

        return false;
    }

}
