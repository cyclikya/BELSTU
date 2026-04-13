using UnityEngine;

public class KamazCabinMechanismsController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode toggleWipersKey = KeyCode.V;
    [SerializeField] private KeyCode toggleBodyKey = KeyCode.B;

    [Header("Animator Bool")]
    [SerializeField] private string turnOnBoolName = "turnOn";

    private KamazContext kamazContext;
    private CameraController cameraController;

    private Animator wiperLeftAnimator;
    private Animator wiperRightAnimator;
    private Animator bodyAnimator;
    private Animator hydraulicAnimator;

    private bool wipersEnabled;
    private bool bodyRaised;

    public bool IsBodyRaised
    {
        get
        {
            bool byState = bodyRaised;
            bool byAnim = GetAnimatorBool(bodyAnimator, turnOnBoolName) || GetAnimatorBool(hydraulicAnimator, turnOnBoolName);
            return byState || byAnim;
        }
    }

    private void Awake()
    {
        ResolveReferences();
    }

    private void Update()
    {
        ResolveReferencesIfMissing();

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
        if (!IsPlayerInCabin())
        {
            return;
        }

        if (!IsEngineRunning())
        {
            return;
        }

        wipersEnabled = !wipersEnabled;
        SetAnimatorBool(wiperLeftAnimator, turnOnBoolName, wipersEnabled);
        SetAnimatorBool(wiperRightAnimator, turnOnBoolName, wipersEnabled);
    }

    private void ToggleBody()
    {
        if (!IsPlayerInCabin())
        {
            return;
        }

        if (IsEngineRunning())
        {
            return;
        }

        bodyRaised = !bodyRaised;
        SetAnimatorBool(bodyAnimator, turnOnBoolName, bodyRaised);
        SetAnimatorBool(hydraulicAnimator, turnOnBoolName, bodyRaised);
    }

    private bool IsPlayerInCabin()
    {
        return cameraController != null && cameraController.IsDrivingMode;
    }

    private bool IsEngineRunning()
    {
        return cameraController != null && cameraController.IsEngineRunning;
    }

    private void ResolveReferencesIfMissing()
    {
        if (kamazContext != null && cameraController != null && wiperLeftAnimator != null && wiperRightAnimator != null && bodyAnimator != null && hydraulicAnimator != null)
        {
            return;
        }

        ResolveReferences();
    }

    private void ResolveReferences()
    {
        if (kamazContext == null)
        {
            kamazContext = GetComponent<KamazContext>();
        }

        if (kamazContext == null)
        {
            kamazContext = KamazContext.Instance;
        }

        if (kamazContext != null)
        {
            kamazContext.AutoResolve();
        }

        if (cameraController == null)
        {
#if UNITY_2023_1_OR_NEWER
            cameraController = FindFirstObjectByType<CameraController>();
#else
            cameraController = FindObjectOfType<CameraController>();
#endif
        }

        if (kamazContext != null)
        {
            if (wiperLeftAnimator == null)
            {
                wiperLeftAnimator = kamazContext.GetNodeAnimator("dwornikL");
            }

            if (wiperRightAnimator == null)
            {
                wiperRightAnimator = kamazContext.GetNodeAnimator("dwornikR");
            }

            if (bodyAnimator == null)
            {
                bodyAnimator = kamazContext.GetNodeAnimator("kuzov");
            }

            if (hydraulicAnimator == null)
            {
                hydraulicAnimator = kamazContext.GetNodeAnimator("gidravl");
            }
        }
    }

    private void SetAnimatorBool(Animator animator, string parameterName, bool value)
    {
        if (animator == null || !HasAnimatorBool(animator, parameterName))
        {
            return;
        }

        animator.SetBool(parameterName, value);
    }

    private bool GetAnimatorBool(Animator animator, string parameterName)
    {
        if (animator == null || !HasAnimatorBool(animator, parameterName))
        {
            return false;
        }

        return animator.GetBool(parameterName);
    }

    private bool HasAnimatorBool(Animator animator, string parameterName)
    {
        if (animator == null)
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == AnimatorControllerParameterType.Bool)
            {
                return true;
            }
        }

        return false;
    }
}
