using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSettings : MonoBehaviour
{
    public AudioSource bgm;

    public void AdjustMasterVolume(int volume)
    {
        AudioListener.volume = volume / 100f;
    }

    public void AdjustBGMVolume(int volume)
    {
        bgm.volume = volume / 100f;
    }

    public void AdjustSoundFXVolume(int volume)
    {
        foreach (GameObject rootObj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            AudioSource[] audioSources = rootObj.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.volume = volume / 100f;
            }
        }
    }
}
