using UnityEngine;
using System.Collections;

namespace VoxelImporter
{
    public class VoxelExplosionUI : MonoBehaviour
    {
        public UnityEngine.UI.Toggle toggleRebirth;
        public UnityEngine.UI.Toggle toggleRealtimeBake;

        public void Awake()
        {
            var touchVoxelExplosion = GetComponent<TouchVoxelExplosion>();
            if (touchVoxelExplosion == null) return;

            toggleRebirth.isOn = touchVoxelExplosion.rebirth;
            toggleRealtimeBake.isOn = touchVoxelExplosion.realTimeBake;
        }

        public void ChangeToggle_Rebirth()
        {
            var touchVoxelExplosion = GetComponent<TouchVoxelExplosion>();
            if (touchVoxelExplosion == null) return;

            touchVoxelExplosion.rebirth = toggleRebirth.isOn;
        }

        public void ChangeToggle_RealTimeBake()
        {
            var touchVoxelExplosion = GetComponent<TouchVoxelExplosion>();
            if (touchVoxelExplosion == null) return;

            touchVoxelExplosion.realTimeBake = toggleRealtimeBake.isOn;
        }
    }
}
