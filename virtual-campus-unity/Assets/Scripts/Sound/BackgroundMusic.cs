using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour
{
    public AudioClip[] audios;
    public int currentMusic;
    [HideInInspector]
    public float remainingTime;
    private bool switchMusic;
    public KeyCode switchMusicKey;
    public bool switchable;

    void Start()
    {
        currentMusic = -1;
        switchMusic = false;
        RandomPlay();
        StartCoroutine(DelayedEnableSwitch());
    }

    public void RandomPlay()
    {
        int randInt;
        var minRan = 0;
        var maxRan = audios.Length;
        while (true)
        {
            randInt = Random.Range(minRan, maxRan);
            if (randInt != currentMusic)
            {
                break;
            }
        }
        Play(randInt);
    }
    public void Play(int index)
    {
        this.GetComponent<AudioSource>().clip = audios[index];
        currentMusic = index;
        this.GetComponent<AudioSource>().Play();
        remainingTime = audios[index].length;
    }

    public IEnumerator DelayedEnableSwitch()
    {
        yield return new WaitForSeconds(2f);
        EnableSwitch();
    }
    private void EnableSwitch()
    {
        switchMusic = true;
    }

    void Update()
    {
        if (Input.GetKey(switchMusicKey) && switchMusic && switchable)
        {
            switchMusic = false;
            RandomPlay();
            StartCoroutine(DelayedEnableSwitch());
        }
        remainingTime -= Time.deltaTime;
        //Debug.Log(remainingTime);
        if (remainingTime < 0)
        {
            RandomPlay();
        }
    }
}