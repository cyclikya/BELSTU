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

    private bool wipersEnabled;
    private bool bodyRaised;

    public bool IsBodyRaised => bodyRaised;

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

        if (kamazContext.BodyAnimator != null)
        {
            kamazContext.BodyAnimator.SetBool("turnOn", bodyRaised);
        }

        if (kamazContext.HydraulicAnimator != null)
        {
            kamazContext.HydraulicAnimator.SetBool("turnOn", bodyRaised);
        }
    }
}
