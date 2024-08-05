using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public  AudioSource AudioSourceEffects;

    public  AudioClip soundCardPlay;
    public  AudioClip soundError;
    public  AudioClip buttonSound;

    

    public void PlayCardSound()
    {
        AudioSourceEffects.clip = soundCardPlay;
        AudioSourceEffects.Play();
    }

    public void PlayErrorSound()
    {
        AudioSourceEffects.clip = soundError;
        AudioSourceEffects.Play();
    }

    public void PlayButtonSound()
    {
        AudioSourceEffects.clip = buttonSound;
        AudioSourceEffects.Play();
    }
}
