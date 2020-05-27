using UnityEngine;
using System.Collections;

namespace VoxelImporter
{
    [RequireComponent(typeof(Animator))]
    public class FrameAnimationChanger : MonoBehaviour
    {
        public VoxelFrameAnimationObject frameAnimationObject;

        public void Awake()
        {
            if (frameAnimationObject == null)
            {
                frameAnimationObject = GetComponentInChildren<VoxelFrameAnimationObject>();
            }
        }

        public void ChangeFrame(string frameName)
        {
            if (frameAnimationObject == null) return;
            frameAnimationObject.ChangeFrame(frameName);
        }
    }
}
