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

    protected AudioSource m_AudioSource;
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        if (m_AudioSource == null)
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.playOnAwake = false;
        }
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
        m_AudioSource.PlayOneShot(ac);
    }
}