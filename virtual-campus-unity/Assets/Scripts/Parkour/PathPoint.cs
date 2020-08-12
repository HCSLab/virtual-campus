using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint : MonoBehaviour
{
    public bool reached = false;
    //public List<GameObject> paths;
    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;

        if (other.tag == "Player")
        {
            reached = true;
            UIManager.Instance.pathPointSFXSource.PlayOneShot(UIManager.Instance.pathPointSFX);
            Parkour.Instance.NextPathPoint();
        }
    }

    private void OnEnable()
    {
        for (int i=0; i<transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            
        }
        while (transform.childCount > 0)
        {
            transform.GetChild(0).SetParent(Parkour.Instance.paths.transform);
        }
    }
}
