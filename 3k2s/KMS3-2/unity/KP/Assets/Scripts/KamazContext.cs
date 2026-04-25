using UnityEngine;

public class KamazContext : MonoBehaviour
{
    public enum SetupSection
    {
        Kuzov,
        Door,
        Lights,
        Panel,
        Steering,
        Key
    }

    [Header("Root")]
    [SerializeField] private Transform root;

    [Header("Main Objects")]
    [SerializeField] private Transform kabina;
    [SerializeField] private Transform fary;
    [SerializeField] private Transform panel;

    [Header("Points")]
    [SerializeField] private Transform seatPoint;
    [SerializeField] private Transform exitPoint;

    [Header("Doors")]
    [SerializeField] private Transform doorL;
    [SerializeField] private Transform doorR;

    [Header("Driving")]
    [SerializeField] private Transform ryle;
    [SerializeField] private Transform key;
    [SerializeField] private Transform speedometerNeedleObject;
    [SerializeField] private Transform tachometerNeedleObject;

    [Header("Mechanisms")]
    [SerializeField] private Transform wiperLeftObject;
    [SerializeField] private Transform wiperRightObject;
    [SerializeField] private Transform bodyObject;
    [SerializeField] private Transform hydraulicObject;

    [Header("Wheels")]
    [SerializeField] private Transform wheelFrontLeft;
    [SerializeField] private Transform wheelFrontRight;
    [SerializeField] private Transform wheelMiddle;
    [SerializeField] private Transform wheelBack;

    private Rigidbody kamazRigidbody;
    private Collider[] allKamazColliders;

    private Animator doorLAnimator;
    private Animator doorRAnimator;
    private Collider[] doorLColliders;
    private Collider[] doorRColliders;

    private Collider[] ryleColliders;
    private Animator keyAnimator;
    private Collider[] keyColliders;
    private GaugeNeedle spidometerNeedle;
    private GaugeNeedle tachometerNeedle;

    private Animator wiperLeftAnimator;
    private Animator wiperRightAnimator;
    private Animator bodyAnimator;
    private Animator hydraulicAnimator;
    private KamazLightsController lightsController;
    private KamazCabinMechanismsController cabinMechanismsController;

    private Transform[] kuzovSetupTargets;
    private Transform[] doorSetupTargets;
    private Transform[] lightsSetupTargets;
    private Transform[] panelSetupTargets;
    private Transform[] steeringSetupTargets;
    private Transform[] keySetupTargets;

    public Transform Root => root;
    public Rigidbody KamazRigidbody => kamazRigidbody;
    public Collider[] AllKamazColliders => allKamazColliders;
    public Transform Kabina => kabina;
    public Transform Kuzov => bodyObject;
    public Transform Fary => fary;
    public Transform Panel => panel;
    public Transform SeatPoint => seatPoint;
    public Transform ExitPoint => exitPoint;
    public Transform DoorL => doorL;
    public Transform DoorR => doorR;
    public Animator DoorLAnimator => doorLAnimator;
    public Animator DoorRAnimator => doorRAnimator;
    public Transform Ryle => ryle;
    public Collider[] RyleColliders => ryleColliders;
    public Transform Key => key;
    public Animator KeyAnimator => keyAnimator;
    public Collider[] KeyColliders => keyColliders;
    public GaugeNeedle SpidometerNeedle => spidometerNeedle;
    public GaugeNeedle TachometerNeedle => tachometerNeedle;
    public Animator WiperLeftAnimator => wiperLeftAnimator;
    public Animator WiperRightAnimator => wiperRightAnimator;
    public Animator BodyAnimator => bodyAnimator;
    public Animator HydraulicAnimator => hydraulicAnimator;
    public Transform WheelFrontLeft => wheelFrontLeft;
    public Transform WheelFrontRight => wheelFrontRight;
    public Transform WheelMiddle => wheelMiddle;
    public Transform WheelBack => wheelBack;
    public KamazLightsController LightsController => lightsController;
    public KamazCabinMechanismsController CabinMechanismsController => cabinMechanismsController;

    private void Awake()
    {
        CacheComponents();
    }

    private void OnValidate()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        if (root == null)
        {
            root = transform;
        }

        kamazRigidbody = GetComponentFromObject<Rigidbody>(root);
        allKamazColliders = GetComponentsFromObject<Collider>(root);

        doorLAnimator = GetAnimatorFromObject(doorL);
        doorRAnimator = GetAnimatorFromObject(doorR);
        doorLColliders = GetComponentsFromObject<Collider>(doorL);
        doorRColliders = GetComponentsFromObject<Collider>(doorR);

        ryleColliders = GetComponentsFromObject<Collider>(ryle);
        keyAnimator = GetAnimatorFromObject(key);
        keyColliders = GetComponentsFromObject<Collider>(key);
        spidometerNeedle = GetComponentFromObject<GaugeNeedle>(speedometerNeedleObject);
        tachometerNeedle = GetComponentFromObject<GaugeNeedle>(tachometerNeedleObject);

        wiperLeftAnimator = GetAnimatorFromObject(wiperLeftObject);
        wiperRightAnimator = GetAnimatorFromObject(wiperRightObject);
        bodyAnimator = GetAnimatorFromObject(bodyObject);
        hydraulicAnimator = GetAnimatorFromObject(hydraulicObject);

        lightsController = GetComponentFromObject<KamazLightsController>(root);
        cabinMechanismsController = GetComponentFromObject<KamazCabinMechanismsController>(root);

        kuzovSetupTargets = BuildTargets(bodyObject);
        doorSetupTargets = BuildTargets(doorL, doorR);
        lightsSetupTargets = BuildTargets(fary);
        panelSetupTargets = BuildTargets(panel);
        steeringSetupTargets = BuildTargets(ryle);
        keySetupTargets = BuildTargets(key);
    }

    public Transform[] GetSetupTargets(SetupSection section)
    {
        if (section == SetupSection.Kuzov) return kuzovSetupTargets;
        if (section == SetupSection.Door) return doorSetupTargets;
        if (section == SetupSection.Lights) return lightsSetupTargets;
        if (section == SetupSection.Panel) return panelSetupTargets;
        if (section == SetupSection.Steering) return steeringSetupTargets;
        return keySetupTargets;
    }

    public bool TryGetDoorFromCollider(Collider hitCollider, out Transform doorRoot, out Animator doorAnimator)
    {
        doorRoot = null;
        doorAnimator = null;

        if (ContainsCollider(doorLColliders, hitCollider))
        {
            doorRoot = doorL;
            doorAnimator = doorLAnimator;
            return true;
        }

        if (ContainsCollider(doorRColliders, hitCollider))
        {
            doorRoot = doorR;
            doorAnimator = doorRAnimator;
            return true;
        }

        return false;
    }

    public bool IsRyleCollider(Collider hitCollider)
    {
        return ContainsCollider(ryleColliders, hitCollider);
    }

    public bool TryGetKeyFromCollider(Collider hitCollider, out Transform keyRoot, out Animator keyAnimatorOut)
    {
        keyRoot = null;
        keyAnimatorOut = null;

        if (!ContainsCollider(keyColliders, hitCollider))
        {
            return false;
        }

        keyRoot = key;
        keyAnimatorOut = keyAnimator;
        return true;
    }

    private Transform[] BuildTargets(params Transform[] targets)
    {
        int count = 0;
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
            {
                count++;
            }
        }

        Transform[] result = new Transform[count];
        int index = 0;
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
            {
                result[index] = targets[i];
                index++;
            }
        }

        return result;
    }

    private T GetComponentFromObject<T>(Transform source) where T : Component
    {
        if (source == null)
        {
            return null;
        }

        T component = source.GetComponent<T>();
        if (component != null)
        {
            return component;
        }

        return source.GetComponentInChildren<T>(true);
    }

    private T[] GetComponentsFromObject<T>(Transform source) where T : Component
    {
        if (source == null)
        {
            return System.Array.Empty<T>();
        }

        return source.GetComponentsInChildren<T>(true);
    }

    private Animator GetAnimatorFromObject(Transform source)
    {
        return GetComponentFromObject<Animator>(source);
    }

    private bool ContainsCollider(Collider[] colliders, Collider target)
    {
        if (colliders == null || target == null)
        {
            return false;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == target)
            {
                return true;
            }
        }

        return false;
    }
}
