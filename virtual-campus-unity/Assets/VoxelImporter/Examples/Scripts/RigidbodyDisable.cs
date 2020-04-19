using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    public class RigidbodyDisable : MonoBehaviour
    {
        public bool massSet = true;
        public float massRate = 0.1f;

        void Awake()
        {
            Action<Rigidbody> Set = (rb) =>
            {
                rb.isKinematic = true;

                if(massSet)
                {
                    var meshFilter = rb.GetComponent<MeshFilter>();
                    if(meshFilter != null && meshFilter.mesh)
                    {
                        var size = meshFilter.mesh.bounds.size.x * meshFilter.mesh.bounds.size.y * meshFilter.mesh.bounds.size.z;
                        rb.mass = size * massRate;
                    }
                }
            };

            {
                var rigidbody = GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    Set(rigidbody);
                }
            }
            {
                var rigidbodys = GetComponentsInChildren<Rigidbody>();
                for (int i = 0; i < rigidbodys.Length; i++)
                {
                    Set(rigidbodys[i]);
                }
            }
            Destroy(this);
        }
    }
}
