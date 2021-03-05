using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSFXHandler : MonoBehaviour
{
    [Header("Audio sources")]
    public AudioSource tiresScreeachingAudioSource;
    public AudioSource engineAudioSource;

    //Local variables
    float tireScreechPitch = 1.0f;
    float enginePitch = 1.0f;

    //Components
    TopDownCarController topDownCarController;

    //Awake is called when the script instance is being loaded.
    void Awake()
    {
        topDownCarController = GetComponentInParent<TopDownCarController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Handle engine SFX
        float velocityMagnitude = topDownCarController.GetVelocityMagnitude();

        //Increase the engine volume as the car goes faster
        float engineVolume = Mathf.Abs(velocityMagnitude * 0.05f);
        engineAudioSource.volume = Mathf.Lerp(engineAudioSource.volume, engineVolume, Time.deltaTime * 10);

        //To add more variation to the engine sound we also change the pitch
        enginePitch = Mathf.Lerp(enginePitch, velocityMagnitude * 0.1f, Time.deltaTime*2);
        enginePitch = Mathf.Clamp(enginePitch, 0.5f, 3.0f);
        engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, enginePitch, Time.deltaTime * 10);

        //Handle tire screeching SFX
        if (topDownCarController.IsTireScreeching(out float lateralVelocity, out bool isBraking))
        {
            //If the car is braking we want the tire screech to be louder and also change the pitch.
            if (isBraking)
            {
                tiresScreeachingAudioSource.volume = Mathf.Lerp(tiresScreeachingAudioSource.volume, 1.0f, Time.deltaTime * 10);
                tireScreechPitch = Mathf.Lerp(tireScreechPitch, 0.5f, Time.deltaTime * 10);
            }
            else
            {
                //If we are not braking we still want to play this screech sound if the player is drifting.
                tiresScreeachingAudioSource.volume = Mathf.Abs(lateralVelocity) * 0.05f;
                tireScreechPitch = Mathf.Abs(lateralVelocity) * 0.1f;
            }

            tireScreechPitch = Mathf.Clamp(tireScreechPitch, 0.5f, 1.1f);
            tiresScreeachingAudioSource.pitch = tireScreechPitch;
        }
        //Fade out the tire screech SFX if we are not screeching. 
        else tiresScreeachingAudioSource.volume = Mathf.Lerp(tiresScreeachingAudioSource.volume, 0, Time.deltaTime * 10);

    }
}
