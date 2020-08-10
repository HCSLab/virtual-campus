using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HearthStoneScriptableObject : UsableItemScriptableObject
{
    public Vector3 posToTransfer;
    //StartUpZone = Vector3(5.4f, 2.1f, -13f)
    public override void Use()
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = posToTransfer;
    }
}
