using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentRegion : MonoBehaviour
{
    private Transform player;
    public TMPro.TextMeshProUGUI tm;
    public int visitCount;
    public int regionCount;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public float GetExplorationPercentage()
    {
        return ((float)visitCount / regionCount) * 100;
    }
    void Update()
    {
        RaycastHit hit;
        bool inRegion = Physics.Raycast(player.position, Vector3.down, out hit, 100.0f, 1 << LayerMask.NameToLayer("CurrentRegion"));
        if (inRegion)
        {
            RegionQuad regionQuad = hit.collider.gameObject.GetComponent<RegionQuad>();
            if (!regionQuad.isVisited && regionQuad.tag != "Ignored")
            {
                regionQuad.isVisited = true;
                visitCount++;
                if (visitCount == regionCount)
                {
                    Debug.Log("All Explored!");
                    //TODO: Unlock the achievement
                }
            }
            string regionName = regionQuad.name;
            tm.text = regionName;
        }
        else
        {
            tm.text = "下园";
        }
    }
}
