using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public AudioClip enterClip;
    public AudioClip clickClip;
    public AudioClip exitClip;

    public AudioSource audioSource;
    void Start()
    {
        audioSource = UIManager.Instance.sfxSource;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayAudio(clickClip);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayAudio(enterClip);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        PlayAudio(exitClip);
    }
    private void PlayAudio(AudioClip ac)
    {
        if (ac == null)
        {
            //Debug.LogError(name + ":audioClip is Null !");
        }
        audioSource.PlayOneShot(ac);
    }
}