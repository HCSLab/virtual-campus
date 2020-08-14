using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    public bool reached = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;

        if (other.tag == "Player")
        {
            reached = true;
            UIManager.Instance.pathPointSFXSource.PlayOneShot(UIManager.Instance.pathPointSFX);
            Parkour.Instance.StartParkour();

            Parkour.Instance.resetPosition = transform.position;
            Parkour.Instance.resetPosition.y = other.transform.position.y + 0.25f;
        }
    }
}
