using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GestureControl4Demo: MonoBehaviour {
    public SingleHandOpenTrack handDir;
    public HandFaceToF handFace;
    public HandOpenToFist handFist;
    private string gesture;
	// Update is called once per frame
	void Update () {     
        string DirZ = handDir.DirZ;
        string DirX = handDir.DirX;
        string DirY = handDir.DirY;
        string DirS = handDir.DirS;
      //  bool FTF = handFace.IsEntered;
        bool Fist = handFist.Fist;
        if (DirZ != "None")
        {
            gesture = DirZ;

        }
        else if (DirX != "None")
        {
            gesture = DirX;
        }
        else if (DirY != "None")
        {
            gesture = DirY;
        }
        else if (DirS != "None")
        {
            gesture = DirS;
        }
        //else if (Fist)
        //{
        //    gesture = "Fist";
        //}
        else
        {
            gesture = "None";
        }
        
     
    }

    public string GetGesture()
    {
        if (gesture!="None")
            return gesture;
        else
            return "None";
    }
}
