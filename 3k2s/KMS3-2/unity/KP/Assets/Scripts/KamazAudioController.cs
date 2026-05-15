using UnityEngine;

// Центральный контроллер звуков КамАЗа.
// Все AudioSource и AudioClip задаются вручную через Inspector.
public class KamazAudioController : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private AudioSource oneShotSource;
    [SerializeField] private AudioSource engineLoopSource;
    [SerializeField] private AudioSource turnSignalLoopSource;
    [SerializeField] private AudioSource wiperLoopSource;
    [SerializeField] private AudioSource hydraulicLoopSource;
    [SerializeField] private AudioSource hornLoopSource;
    [SerializeField] private AudioSource reverseLoopSource;

    [Header("Door Clips")]
    [SerializeField] private AudioClip doorOpenClip;
    [SerializeField] private AudioClip doorCloseClip;

    [Header("Engine Clips")]
    [SerializeField] private AudioClip engineStartClip;
    [SerializeField] private AudioClip engineStopClip;
    [SerializeField] private AudioClip engineRunClip;

    [Header("Vehicle Clips")]
    [SerializeField] private AudioClip hornClip;
    [SerializeField] private AudioClip reverseClip;
    [SerializeField] private AudioClip gearSwitchClip;

    [Header("Switch Clips")]
    [SerializeField] private AudioClip switchClip;
    [SerializeField] private AudioClip turnSignalClip;
    [SerializeField] private AudioClip wiperLoopClip;
    [SerializeField] private AudioClip hydraulicLoopClip;

    [Header("Loop Durations")]
    [SerializeField] private float hydraulicLoopDuration = 3f;

    private bool engineLoopActive;
    private bool turnSignalLoopActive;
    private bool wiperLoopActive;
    private bool hydraulicLoopActive;
    private bool hornLoopActive;
    private bool reverseLoopActive;
    private Coroutine engineStartCoroutine;
    private Coroutine hydraulicLoopCoroutine;

    private void Awake()
    {
        PrepareLoopSource(engineLoopSource, engineRunClip);
        PrepareLoopSource(turnSignalLoopSource, turnSignalClip);
        PrepareLoopSource(wiperLoopSource, wiperLoopClip);
        PrepareLoopSource(hydraulicLoopSource, hydraulicLoopClip);
        PrepareLoopSource(hornLoopSource, hornClip);
        PrepareLoopSource(reverseLoopSource, reverseClip);
    }

    public void PlayDoorOpen()
    {
        PlayOneShot(doorOpenClip, "Door open");
    }

    public void PlayDoorClose()
    {
        PlayOneShot(doorCloseClip, "Door close");
    }

    public void StartEngineAudio()
    {
        PlayOneShot(engineStartClip, "Engine start");
        StopEngineStartCoroutine();
        SetEngineLoop(false);

        if (engineLoopSource == null || engineRunClip == null)
        {
            return;
        }

        float delay = engineStartClip != null ? engineStartClip.length : 0f;
        engineStartCoroutine = StartCoroutine(StartEngineLoopAfterDelay(delay));
    }

    public void StopEngineAudio()
    {
        StopEngineStartCoroutine();
        SetEngineLoop(false);
        PlayOneShot(engineStopClip, "Engine stop");
    }

    public void StallEngineAudio()
    {
        StopEngineStartCoroutine();
        SetEngineLoop(false);
        PlayOneShot(engineStopClip, "Engine stall");
    }

    public void SetEngineLoop(bool enabled)
    {
        SetLoopState(engineLoopSource, engineRunClip, enabled, "Engine loop", ref engineLoopActive);
    }

    public void UpdateEngineLoop(float rpmNormalized)
    {
        if (engineLoopSource == null)
        {
            return;
        }

        float t = Mathf.Clamp01(rpmNormalized);
        engineLoopSource.pitch = Mathf.Lerp(0.85f, 1.35f, t);
        engineLoopSource.volume = Mathf.Lerp(0.35f, 0.8f, t);
    }

    public void SetTurnSignalLoop(bool enabled)
    {
        SetLoopState(turnSignalLoopSource, turnSignalClip, enabled, "Turn signal loop", ref turnSignalLoopActive);
    }

    public void PlayHeadlightSwitch()
    {
        PlayOneShot(switchClip, "Headlight switch");
    }

    public void PlayWiperSwitch()
    {
        PlayOneShot(switchClip, "Wiper switch");
    }

    public void SetWiperLoop(bool enabled)
    {
        SetLoopState(wiperLoopSource, wiperLoopClip, enabled, "Wiper loop", ref wiperLoopActive);
    }

    public void PlayBodySwitch()
    {
        PlayOneShot(switchClip, "Body switch");
    }

    public void PlayHydraulicCycle()
    {
        if (hydraulicLoopCoroutine != null)
        {
            StopCoroutine(hydraulicLoopCoroutine);
            hydraulicLoopCoroutine = null;
        }

        SetHydraulicLoop(true);
        hydraulicLoopCoroutine = StartCoroutine(HydraulicLoopRoutine());
    }

    public void SetHornLoop(bool enabled)
    {
        SetLoopState(hornLoopSource, hornClip, enabled, "Horn loop", ref hornLoopActive);
    }

    public void SetReverseLoop(bool enabled)
    {
        SetLoopState(reverseLoopSource, reverseClip, enabled, "Reverse loop", ref reverseLoopActive);
    }

    public void PlayGearSwitch()
    {
        PlayOneShot(gearSwitchClip, "Gear switch");
    }

    public void StopAllLoops()
    {
        StopEngineStartCoroutine();
        SetEngineLoop(false);
        SetTurnSignalLoop(false);
        SetWiperLoop(false);
        SetHydraulicLoop(false);
        SetHornLoop(false);
        SetReverseLoop(false);
    }

    private void PlayOneShot(AudioClip clip, string label)
    {
        if (oneShotSource == null || clip == null)
        {
            return;
        }

        oneShotSource.PlayOneShot(clip);
    }

    private void SetLoopState(AudioSource source, AudioClip clip, bool enabled, string label, ref bool state)
    {
        if (source == null || clip == null)
        {
            return;
        }

        if (enabled)
        {
            if (state)
            {
                return;
            }

            source.clip = clip;
            source.loop = true;
            source.Play();
            state = true;
            return;
        }

        if (!state)
        {
            return;
        }

        source.Stop();
        state = false;
    }

    private void PrepareLoopSource(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null)
        {
            return;
        }

        source.clip = clip;
        source.loop = true;
    }

    private void SetHydraulicLoop(bool enabled)
    {
        SetLoopState(hydraulicLoopSource, hydraulicLoopClip, enabled, "Hydraulic loop", ref hydraulicLoopActive);
    }

    private System.Collections.IEnumerator StartEngineLoopAfterDelay(float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        SetEngineLoop(true);
        engineStartCoroutine = null;
    }

    private System.Collections.IEnumerator HydraulicLoopRoutine()
    {
        yield return new WaitForSeconds(hydraulicLoopDuration);
        SetHydraulicLoop(false);
        hydraulicLoopCoroutine = null;
    }

    private void StopEngineStartCoroutine()
    {
        if (engineStartCoroutine == null)
        {
            return;
        }

        StopCoroutine(engineStartCoroutine);
        engineStartCoroutine = null;
    }
}
