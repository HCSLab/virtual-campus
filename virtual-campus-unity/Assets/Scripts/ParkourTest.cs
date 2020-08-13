using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourTest : MonoBehaviour
{
    public List<string> parkours;
    public GameObject talkToSelect;
    public GameObject pressToTalk;

    public void Test(int i)
    {
        Object cubePreb = Resources.Load(parkours[i], typeof(GameObject));
        GameObject parkour = Instantiate(cubePreb) as GameObject;
    }
}
