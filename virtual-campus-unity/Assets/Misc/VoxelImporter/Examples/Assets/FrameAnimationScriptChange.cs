using UnityEngine;
using System.Collections;

namespace VoxelImporter
{
    [RequireComponent(typeof(VoxelFrameAnimationObject))]
    public class FrameAnimationScriptChange : MonoBehaviour
    {
        private VoxelFrameAnimationObject frameAnimationObject;

        public void Awake()
        {
            frameAnimationObject = GetComponent<VoxelFrameAnimationObject>();
        }

        public void OnClick(string frameName)
        {
            frameAnimationObject.ChangeFrame(frameName);
        }
    }
}
