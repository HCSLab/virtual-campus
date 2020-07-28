﻿using UnityEngine;
using System.Collections;

namespace ThirdPersonCamera
{
    [RequireComponent(typeof(CameraController)), RequireComponent(typeof(Follow)), RequireComponent(typeof(FreeForm))]
    public class DisableFollow : MonoBehaviour
    {

        public bool activateMotionCheck = true;
        public bool activateTimeCheck = true;
        public bool activateMouseCheck = true;

        public float timeToActivate = 1.0f;
        public float motionThreshold = 0.05f;

        private CameraController cameraController;
        private FreeForm freeForm;
        private Follow follow;

        private bool followDisabled;
        private Vector3 prevPosition;

        /*
        [HideInInspector]
        public bool moving = false;
        */

        // Use this for initialization
        void Start()
        {
            cameraController = GetComponent<CameraController>();
            follow = GetComponent<Follow>();
            freeForm = GetComponent<FreeForm>();
            followDisabled = !follow.follow;
        }

        // Update is called once per frame
        void Update()
        {
            if (freeForm.x != 0 || freeForm.y != 0)
            {
                follow.follow = false;
                followDisabled = true;
            }

            /*
            if (followDisabled)
            {
                if (activateMotionCheck)
                {
                    Vector3 motionVector = cameraController.target.transform.position - prevPosition;

                    if (motionVector.magnitude > motionThreshold)
                    {
                        follow.follow = true;
                        followDisabled = false;
                    }
                }

                if (activateTimeCheck)
                    Invoke("ActivateFollow", timeToActivate);

                if (activateMouseCheck && (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)))
                {                    
                    follow.follow = true;
                    followDisabled = false;
                }
            }
            */

            prevPosition = cameraController.target.transform.position;
        }

        public void ActivateFollow()
        {
            if (freeForm.x == 0 && freeForm.y == 0)
            {
                follow.follow = true;
                followDisabled = false;
            }
            else
            {
                Invoke("ActivateFollow", timeToActivate);
            }
        }
    }
}