using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSound : MonoBehaviour
{
    
    public AudioSource audioSource;
    public AudioClip[] jumpClips;

    private bool wasGrounded = false;
    private bool isGrounded;

    public bool enableSound;
    public float playerToGround;
    void Start()
    {
        enableSound = true;
        wasGrounded = Physics.Raycast(transform.position, Vector3.down, playerToGround, 1 << LayerMask.NameToLayer("VoxWorld"));
    }


    void Update()
    {

        enableSound = gameObject.GetComponent<PlayerController>().enabled;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerToGround, 1 << LayerMask.NameToLayer("VoxWorld"));

        if (!enableSound)
        {
            audioSource.Stop();
            //air = false;
            wasGrounded = true;
            return;
        }

        if ((wasGrounded == false) && (isGrounded == true))
        {
            RandomPlay();
        }

        wasGrounded = isGrounded;

    }

    public void RandomPlay()
    {
        int randInt;
        int minRan = 0;
        int maxRan = jumpClips.Length;
        randInt = Random.Range(minRan, maxRan);
        Play(randInt);
    }
    public void Play(int index)
    {
        audioSource.clip = jumpClips[index];
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Play();
    }
}


