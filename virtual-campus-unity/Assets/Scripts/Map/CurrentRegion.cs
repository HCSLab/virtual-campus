using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentRegion : MonoBehaviour
{
    private Transform player;
    public TMPro.TextMeshProUGUI tm;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        RaycastHit hit;
        bool inRegion = Physics.Raycast(player.position, Vector3.down, out hit, 100.0f, 1 << LayerMask.NameToLayer("CurrentRegion"));
        if (inRegion)
        {
            string regionName = hit.collider.gameObject.GetComponent<RegionQuad>().name;
            tm.text = regionName;
        }
        else
        {
            tm.text = "下园";
        }
    }
}
