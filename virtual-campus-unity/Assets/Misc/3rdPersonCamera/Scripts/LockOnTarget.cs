using UnityEngine;
using System.Collections;

namespace ThirdPersonCamera
{
    [RequireComponent(typeof(CameraController)), RequireComponent(typeof(FreeForm))]
    public class LockOnTarget : MonoBehaviour
    {
        public Targetable followTarget = null;
        public float rotationSpeed = 3.0f;
        public Vector3 tiltVector;

        private CameraController cc;
        private FreeForm ff;

        void Start()
        {
            cc = GetComponent<CameraController>();
            ff = GetComponent<FreeForm>();
        }

        void Update()
        {
            if (Input.GetMouseButton(1)) // right mouse click starts lock on mode
            {
                if (followTarget == null)
                {
                    // find a viable target

                    Targetable[] targets = FindObjectsOfType<Targetable>();

                    if (targets != null && targets.Length > 0)
                    {
                        // add target acquiring by distance and angle to find the best target
                        // also add target switching via input

                        followTarget = targets[0];
                    }
                }

                if (followTarget != null)
                {
                    ff.cameraEnabled = false; // disable freeform control
                    Vector3 dirToTarget = (followTarget.transform.position + followTarget.offset) - transform.position;
                    Quaternion toRotation = Quaternion.LookRotation(dirToTarget, Vector3.up);

                    cc.transform.rotation = Quaternion.Slerp(cc.transform.rotation, toRotation, Time.deltaTime * rotationSpeed);
                }
            }
            else if (followTarget != null)
            {
                followTarget = null;
                ff.cameraEnabled = true; // enable freeform control again
            }
        }
    }
}