using UnityEngine;

// Sends trigger events from mission zones to the mission controller.
public class MissionTriggerZone : MonoBehaviour
{
    public enum ZoneType
    {
        ForwardCheckpoint,
        ReverseCheckpoint,
        UnloadPit
    }

    [SerializeField] private MissionController missionController;
    [SerializeField] private ZoneType zoneType;
    [SerializeField] private GameObject visualRoot;

    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        ApplyState(false, false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (missionController != null)
        {
            missionController.HandleZoneEntered(zoneType, other);
        }
    }

    public void SetState(bool active, bool visible)
    {
        ApplyState(active, visible);
    }

    public ZoneType GetZoneType()
    {
        return zoneType;
    }

    private void ApplyState(bool active, bool visible)
    {
        if (triggerCollider != null)
        {
            triggerCollider.enabled = active;
        }

        if (visualRoot != null)
        {
            visualRoot.SetActive(visible);
            return;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = visible;
        }
    }
}
