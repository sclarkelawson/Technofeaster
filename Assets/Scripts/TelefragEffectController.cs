using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using FMOD.Studio;
using FMODUnity;

public class TelefragEffectController : MonoBehaviour
{
    [SerializeField]
    private PostProcessVolume teleportVolume;
    [SerializeField]
    private PostProcessVolume chargingVolume;
    public PlayerAbilitiesController PlayerAbilities;
    public EventReference ChargingEvent;
    public EventReference DroningEvent;
    private EventInstance ChargingInstance;
    private EventInstance DroningInstance;

    private void Start()
    {
        ChargingInstance = RuntimeManager.CreateInstance(ChargingEvent);
        DroningInstance = RuntimeManager.CreateInstance(DroningEvent);
    }

    public void StartChargingEffect(float duration)
    {
        float completionTime = Time.time + duration;
        DroningInstance.start();
        ChargingInstance.start();
        LeanTween.value(gameObject, 0, 1, duration).setOnUpdate((float val) =>
        {
            chargingVolume.weight = val;
            DroningInstance.setParameterByName("TelefragCompletion", PlayerAbilities.TelefragCompletion);
        }).setOnComplete(() =>
        {
            ChargingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            DroningInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            chargingVolume.weight = 0;
        });
    }
    public void StopChargingEffect()
    {
        LeanTween.cancel(gameObject);
        chargingVolume.weight = 0;
        DroningInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        ChargingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void TeleportEffect()
    {
        LeanTween.value(gameObject, 1, 0, _teleportFadeDuration).setOnUpdate((float val) => teleportVolume.weight = val);
    }

    [SerializeField]
    private float _teleportFadeDuration;
}
