using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;
using System.Collections.Generic;


public class GazeController : MonoBehaviour
{
   
    //Gaze point in screen coordination
    private GazePoint _lastHandledPoint = GazePoint.Invalid;
   

   //Gaze information in the procedure
    private float _pauseTimer;

    private float X = 0;
    private float Y = 0;
    void Update()
    {
        //Check time up
        if (_pauseTimer > 0)
        {
            _pauseTimer -= Time.deltaTime;
            return;
        }

        //Gaze operation
        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        if (gazePoint.IsValid)
        {
            //Calculate fixation points
            IEnumerable<GazePoint> pointsSinceLastHandled = TobiiAPI.GetGazePointsSince(_lastHandledPoint);
            int count = 0;
            using (IEnumerator<GazePoint> enumerator = pointsSinceLastHandled.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    count++;
            }
            float averX = 0;
            float averY = 0;
            int index = 0;
            foreach (var point in pointsSinceLastHandled)
            {

                if (index < count)
                {
                    averX = averX + point.Screen.x;
                    averY = averY + point.Screen.y;
                    index++;
                }
                else
                    break;
            }

            averX /= count;
            averY /= count;

            
            
            X = averX;
            Y = averY;
        }
    }
    public Vector3 GetGazePoint()
    {
        if (X != 0 && Y != 0)
            return new Vector3(X, UnityEngine.Screen.height-Y, 0);
        else
            return new Vector3(0, 0, 0);
    }
}

