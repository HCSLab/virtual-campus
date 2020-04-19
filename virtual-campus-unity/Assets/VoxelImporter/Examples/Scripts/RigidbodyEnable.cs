using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    public class RigidbodyEnable : MonoBehaviour
    {
        protected Rigidbody rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Destroy(this);
            }
        }

        void OnCollisionEnter()
        {
            rb.isKinematic = false;
        }
    }
}
