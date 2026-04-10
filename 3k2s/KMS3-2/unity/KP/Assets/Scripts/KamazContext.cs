using UnityEngine;

public class KamazContext : MonoBehaviour
{
    public static KamazContext Instance { get; private set; }

    private Transform kamazRoot;

    private Transform gidravl;
    private Transform wheelBack;
    private Transform wheelMidlle;
    private Transform wheelRF;
    private Transform wheelLF;
    private Transform kabina;
    private Transform kuzov;

    private Transform doorL;
    private Transform doorR;
    private Transform dwornikL;
    private Transform dwornikR;
    private Transform key;
    private Transform pedalGaz;
    private Transform pedalSceplenie;
    private Transform pedalTormoz;
    private Transform peredachi;
    private Transform ryle;
    private Transform spidometr;
    private Transform strelkaSpid;
    private Transform tachometr;
    private Transform strelkaTach;
    private Transform panel;

    private Transform btnL;
    private Transform btnR;
    private Transform switcherAvariyka;
    private Transform switcherDvorniki;
    private Transform switcherFary;
    private Transform switcherKuzov;

    private Transform kryshka;

    private Transform seatPoint;
    private Transform exitPoint;

    private Animator doorLAnimator;
    private Animator doorRAnimator;
    private Animator keyAnimator;
    private Collider doorLCollider;
    private Collider doorRCollider;
    private Collider ryleCollider;
    private Collider keyCollider;
    private Collider[] doorLColliders;
    private Collider[] doorRColliders;
    private Collider[] ryleColliders;
    private Collider[] keyColliders;
    private Collider[] allKamazColliders;
    private GaugeNeedle spidometerNeedle;
    private GaugeNeedle tachometerNeedle;

    public Transform Root => kamazRoot;
    public Transform DoorL => doorL;
    public Transform DoorR => doorR;
    public Transform Kabina => kabina;
    public Transform Ryle => ryle;
    public Transform Key => key;
    public Transform SeatPoint => seatPoint;
    public Transform ExitPoint => exitPoint;
    public GaugeNeedle SpidometerNeedle => spidometerNeedle;
    public GaugeNeedle TachometerNeedle => tachometerNeedle;
    public Animator DoorLAnimator => doorLAnimator;
    public Animator DoorRAnimator => doorRAnimator;
    public Animator KeyAnimator => keyAnimator;
    public Collider DoorLCollider => doorLCollider;
    public Collider DoorRCollider => doorRCollider;
    public Collider RyleCollider => ryleCollider;
    public Collider KeyCollider => keyCollider;
    public Collider[] DoorLColliders => doorLColliders;
    public Collider[] DoorRColliders => doorRColliders;
    public Collider[] RyleColliders => ryleColliders;
    public Collider[] KeyColliders => keyColliders;
    public Collider[] AllKamazColliders => allKamazColliders;

    public Transform GetNode(string nodeName)
    {
        if (string.IsNullOrEmpty(nodeName))
        {
            return null;
        }

        return FindByName(nodeName);
    }

    public Collider GetNodeCollider(string nodeName)
    {
        return ResolveCollider(GetNode(nodeName));
    }

    public Animator GetNodeAnimator(string nodeName)
    {
        return ResolveAnimator(GetNode(nodeName));
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        Instance = this;
        AutoResolve();
    }

    [ContextMenu("Auto Resolve")]
    public void AutoResolve()
    {
        if (kamazRoot == null)
        {
            kamazRoot = transform;
        }

        gidravl = FindByName("gidravl");
        wheelBack = FindByName("wheel_back");
        wheelMidlle = FindByName("wheel_midlle");
        wheelRF = FindByName("wheelRF");
        wheelLF = FindByName("wheelLF");
        kabina = FindByName("kabina");
        kuzov = FindByName("kuzov");

        doorL = FindByName("doorL", kabina);
        doorR = FindByName("doorR", kabina);
        dwornikL = FindByName("dwornikL", kabina);
        dwornikR = FindByName("dwornikR", kabina);
        key = FindByName("key", kabina);
        pedalGaz = FindByName("pedal_gaz", kabina);
        pedalSceplenie = FindByName("pedal_sceplenie", kabina);
        pedalTormoz = FindByName("pedal_tormoz", kabina);
        peredachi = FindByName("peredachi", kabina);
        ryle = FindByName("ryle", kabina);
        spidometr = FindByName("spidometr", kabina);
        tachometr = FindByName("tachometr", kabina);
        panel = FindByName("panel", kabina);

        strelkaSpid = FindByName("strelka_spid", spidometr);
        strelkaTach = FindByName("strelka_tach", tachometr);

        btnL = FindByName("btn_L", panel);
        btnR = FindByName("btn_R", panel);
        switcherAvariyka = FindByName("switcher_avariyka", panel);
        switcherDvorniki = FindByName("switcher_dvorniki", panel);
        switcherFary = FindByName("switcher_fary", panel);
        switcherKuzov = FindByName("switcher_kuzov", panel);

        kryshka = FindByName("kryshka", kuzov);

        seatPoint = FindByName("SeatPoint");
        exitPoint = FindByName("ExitPoint");

        doorLAnimator = ResolveAnimator(doorL);
        doorRAnimator = ResolveAnimator(doorR);
        keyAnimator = ResolveAnimator(key);
        doorLCollider = ResolveCollider(doorL);
        doorRCollider = ResolveCollider(doorR);
        ryleCollider = ResolveCollider(ryle);
        keyCollider = ResolveCollider(key);
        doorLColliders = ResolveAllColliders(doorL);
        doorRColliders = ResolveAllColliders(doorR);
        ryleColliders = ResolveAllColliders(ryle);
        keyColliders = ResolveAllColliders(key);
        spidometerNeedle = ResolveNeedle(strelkaSpid);
        tachometerNeedle = ResolveNeedle(strelkaTach);

        allKamazColliders = kamazRoot != null ? kamazRoot.GetComponentsInChildren<Collider>(true) : System.Array.Empty<Collider>();
    }

    public bool TryGetDoorFromChild(Transform target, out Transform doorRoot, out Animator doorAnimator)
    {
        doorRoot = null;
        doorAnimator = null;

        if (target == null)
        {
            return false;
        }

        if (doorL != null && IsRelated(target, doorL))
        {
            doorRoot = doorL;
            doorAnimator = doorLAnimator != null ? doorLAnimator : ResolveAnimator(doorL);
            return true;
        }

        if (doorR != null && IsRelated(target, doorR))
        {
            doorRoot = doorR;
            doorAnimator = doorRAnimator != null ? doorRAnimator : ResolveAnimator(doorR);
            return true;
        }

        return false;
    }

    public bool TryGetDoorFromCollider(Collider hitCollider, out Transform doorRoot, out Animator doorAnimator)
    {
        doorRoot = null;
        doorAnimator = null;

        if (hitCollider == null)
        {
            return false;
        }

        if (ContainsCollider(doorLColliders, hitCollider))
        {
            doorRoot = doorL;
            doorAnimator = doorLAnimator != null ? doorLAnimator : ResolveAnimator(doorL);
            return true;
        }

        if (ContainsCollider(doorRColliders, hitCollider))
        {
            doorRoot = doorR;
            doorAnimator = doorRAnimator != null ? doorRAnimator : ResolveAnimator(doorR);
            return true;
        }

        // Fallback на иерархию hit-коллайдера (если кэш коллайдеров устарел).
        Transform hitTransform = hitCollider.transform;
        if (doorL != null && (hitTransform == doorL || hitTransform.IsChildOf(doorL)))
        {
            doorRoot = doorL;
            doorAnimator = doorLAnimator != null ? doorLAnimator : ResolveAnimator(doorL);
            return true;
        }

        if (doorR != null && (hitTransform == doorR || hitTransform.IsChildOf(doorR)))
        {
            doorRoot = doorR;
            doorAnimator = doorRAnimator != null ? doorRAnimator : ResolveAnimator(doorR);
            return true;
        }

        return false;
    }

    public bool IsRyleOrChild(Transform target)
    {
        if (target == null || ryle == null)
        {
            return false;
        }

        return IsRelated(target, ryle);
    }

    public bool IsRyleCollider(Collider hitCollider)
    {
        if (ContainsCollider(ryleColliders, hitCollider))
        {
            return true;
        }

        if (hitCollider == null)
        {
            return false;
        }

        if (ryle != null)
        {
            Transform hitTransform = hitCollider.transform;
            if (hitTransform == ryle || hitTransform.IsChildOf(ryle))
            {
                return true;
            }
        }

        return false;
    }

    public bool TryGetKeyFromCollider(Collider hitCollider, out Transform keyRoot, out Animator keyAnimatorOut)
    {
        keyRoot = null;
        keyAnimatorOut = null;

        if (hitCollider == null)
        {
            return false;
        }

        if (ContainsCollider(keyColliders, hitCollider))
        {
            keyRoot = key;
            keyAnimatorOut = keyAnimator != null ? keyAnimator : ResolveAnimator(key);
            return true;
        }

        Transform hitTransform = hitCollider.transform;
        if (key != null && (hitTransform == key || hitTransform.IsChildOf(key)))
        {
            keyRoot = key;
            keyAnimatorOut = keyAnimator != null ? keyAnimator : ResolveAnimator(key);
            return true;
        }

        return false;
    }

    private Transform FindByName(string objectName, Transform scope = null)
    {
        Transform root = scope != null ? scope : kamazRoot;
        if (root == null)
        {
            return null;
        }

        if (NamesEqual(root.name, objectName))
        {
            return root;
        }

        return FindDeepChild(root, objectName);
    }

    private Transform FindDeepChild(Transform parent, string objectName)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (NamesEqual(child.name, objectName))
            {
                return child;
            }

            Transform found = FindDeepChild(child, objectName);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private Animator ResolveAnimator(Transform target)
    {
        if (target == null)
        {
            return null;
        }

        Animator animator = target.GetComponent<Animator>();
        if (animator != null)
        {
            return animator;
        }

        animator = target.GetComponentInChildren<Animator>(true);
        if (animator != null)
        {
            return animator;
        }

        return target.GetComponentInParent<Animator>();
    }

    private Collider ResolveCollider(Transform target)
    {
        if (target == null)
        {
            return null;
        }

        Collider collider = target.GetComponent<Collider>();
        if (collider != null)
        {
            return collider;
        }

        return target.GetComponentInChildren<Collider>(true);
    }

    private Collider[] ResolveAllColliders(Transform target)
    {
        if (target == null)
        {
            return System.Array.Empty<Collider>();
        }

        return target.GetComponentsInChildren<Collider>(true);
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

    private bool IsRelated(Transform a, Transform b)
    {
        if (a == null || b == null)
        {
            return false;
        }

        // Взаимодействие считается только при попадании в сам объект
        // или в один из его дочерних объектов.
        return a == b || a.IsChildOf(b);
    }

    private bool NamesEqual(string a, string b)
    {
        return string.Equals(a, b, System.StringComparison.OrdinalIgnoreCase);
    }

    private GaugeNeedle ResolveNeedle(Transform target)
    {
        if (target == null)
        {
            return null;
        }

        GaugeNeedle needle = target.GetComponent<GaugeNeedle>();
        if (needle != null)
        {
            return needle;
        }

        return target.GetComponentInChildren<GaugeNeedle>(true);
    }

}
