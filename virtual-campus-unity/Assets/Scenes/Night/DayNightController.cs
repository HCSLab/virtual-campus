using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightController : MonoBehaviour
{
    public GameObject dayModel;
    public GameObject nightModel;

    public void OoNight()
    {
        dayModel.SetActive(false);
        nightModel.SetActive(true);
    }

    public void OnDay()
    {
        dayModel.SetActive(true);
        nightModel.SetActive(false);
    }
}
