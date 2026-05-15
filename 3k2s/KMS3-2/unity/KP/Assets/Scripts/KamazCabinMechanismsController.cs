using UnityEngine;

// Управляет дворниками и подъемом кузова.
public class KamazCabinMechanismsController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private KamazContext kamazContext;

    [Header("Keys")]
    [SerializeField] private KeyCode toggleWipersKey = KeyCode.V;
    [SerializeField] private KeyCode toggleBodyKey = KeyCode.B;

    [Header("Kryshka")]
    [SerializeField] private float kryshkaOpenAngle = 40f;
    [SerializeField] private float kryshkaRotateDuration = 3f;

    private bool wipersEnabled;
    private bool bodyRaised;
    private Quaternion kryshkaStartRotation;
    private Coroutine kryshkaCoroutine;

    public bool IsBodyRaised => bodyRaised;

    private void Awake()
    {
        if (kamazContext != null && kamazContext.Kryshka != null)
        {
            kryshkaStartRotation = kamazContext.Kryshka.localRotation;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleWipersKey))
        {
            ToggleWipers();
        }

        if (Input.GetKeyDown(toggleBodyKey))
        {
            ToggleBody();
        }
    }

    private void ToggleWipers()
    {
        if (cameraController == null || kamazContext == null)
        {
            return;
        }

        if (!cameraController.IsDrivingMode || !cameraController.IsEngineRunning)
        {
            return;
        }

        wipersEnabled = !wipersEnabled;

        if (kamazContext.AudioController != null)
        {
            kamazContext.AudioController.PlayWiperSwitch();
            kamazContext.AudioController.SetWiperLoop(wipersEnabled);
        }

        if (kamazContext.WiperLeftAnimator != null)
        {
            kamazContext.WiperLeftAnimator.SetBool("turnOn", wipersEnabled);
        }

        if (kamazContext.WiperRightAnimator != null)
        {
            kamazContext.WiperRightAnimator.SetBool("turnOn", wipersEnabled);
        }
    }

    private void ToggleBody()
    {
        if (cameraController == null || kamazContext == null)
        {
            return;
        }

        if (!cameraController.IsDrivingMode || cameraController.IsEngineRunning)
        {
            return;
        }

        bodyRaised = !bodyRaised;

        if (kamazContext.AudioController != null)
        {
            kamazContext.AudioController.PlayBodySwitch();
            kamazContext.AudioController.PlayHydraulicCycle();
        }

        if (kamazContext.BodyAnimator != null)
        {
            kamazContext.BodyAnimator.SetBool("turnOn", bodyRaised);
        }

        if (kamazContext.HydraulicAnimator != null)
        {
            kamazContext.HydraulicAnimator.SetBool("turnOn", bodyRaised);
        }

        AnimateKryshka(bodyRaised);
    }

    private void AnimateKryshka(bool open)
    {
        if (kamazContext == null || kamazContext.Kryshka == null)
        {
            return;
        }

        if (kryshkaCoroutine != null)
        {
            StopCoroutine(kryshkaCoroutine);
        }

        kryshkaCoroutine = StartCoroutine(AnimateKryshkaRoutine(open));
    }

    private System.Collections.IEnumerator AnimateKryshkaRoutine(bool open)
    {
        Transform kryshka = kamazContext.Kryshka;
        Quaternion startRotation = kryshka.localRotation;
        Quaternion targetRotation = kryshkaStartRotation * Quaternion.AngleAxis(open ? kryshkaOpenAngle : 0f, Vector3.right);
        float duration = Mathf.Max(0.01f, kryshkaRotateDuration);
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            kryshka.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        kryshka.localRotation = targetRotation;
        kryshkaCoroutine = null;
    }
}

