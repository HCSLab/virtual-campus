using UnityEngine;
using System.Collections;

public class Footstep : MonoBehaviour
{
    private bool walk = false;
    private bool run = false;
    public AudioSource audioSource;
    public AudioClip[] walkClips;
    public AudioClip[] runClips;

    public KeyCode forward;
    public KeyCode left;
    public KeyCode right;
    public KeyCode backward;
    public KeyCode sprint;

    private bool isGrounded;
    public bool enableSound;
    public float playerToGround;

    private float remainingTime;
    void Start()
    {
        enableSound = true;
    }

    void Update()
    {
        enableSound = gameObject.GetComponent<PlayerController>().enabled;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerToGround, 1 << LayerMask.NameToLayer("VoxWorld"));
        if (!enableSound)
        {
            audioSource.Stop();
            walk = false;
            run = false;
            return;
        }

        if ((Input.GetKey(forward) || Input.GetKey(left) || Input.GetKey(right) || Input.GetKey(backward)) && isGrounded)
        {

            if (Input.GetKey(sprint))
            {
                if (run == true)
                {
                    remainingTime -= Time.deltaTime;
                    if (remainingTime < 0)
                    {
                        RandomPlay(1);
                    }
                    return;
                }
                RandomPlay(1);
                walk = false;
                run = true;
            }

            else
            {
                if (walk == true)
                {
                    remainingTime -= Time.deltaTime;
                    if (remainingTime < 0)
                    {
                        RandomPlay(0);
                    }
                    return;
                }
                RandomPlay(0);
                walk = true;
                run = false;
            }

        }

        else
        {
            audioSource.Stop();
            walk = false;
            run = false;
        }
    }

    public void RandomPlay(int mode)
    {
        int randInt;
        int minRan = 0;
        int maxRan = 0;
        if (mode == 0)
        {
            maxRan = walkClips.Length;
        }
        else if (mode == 1)
        {
            maxRan = runClips.Length;
        }
        randInt = Random.Range(minRan, maxRan);
        Play(randInt, mode);
    }
    public void Play(int index, int mode)
    {
        if (mode == 0)
        {
            audioSource.clip = walkClips[index];
            remainingTime = walkClips[index].length;
        }
        else if (mode == 1)
        {
            audioSource.clip = runClips[index];
            remainingTime = runClips[index].length;
        }
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Play();
    }
}