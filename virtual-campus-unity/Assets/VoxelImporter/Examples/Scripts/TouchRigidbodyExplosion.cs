using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
	public class TouchRigidbodyExplosion : MonoBehaviour
	{
        public float radius = 10f;
        public float power = 500f;

        void Update()
        {
            bool explosion = false;
            Vector3 position = Vector3.zero;
            if (Input.GetMouseButtonDown(0))
            {
                explosion = true;
                position = Input.mousePosition;
            }
            if (Input.touchCount > 0)
            {
                explosion = true;
                position = Input.GetTouch(0).position;
            }
            if (explosion)
            {
                Ray ray = Camera.main.ScreenPointToRay(position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    var colliders = Physics.OverlapSphere(hit.point, radius);
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        var rigidbody = colliders[i].GetComponent<Rigidbody>();
                        if (rigidbody == null) continue;

                        rigidbody.isKinematic = false;

                        rigidbody.AddExplosionForce(power, hit.point, radius);
                    }
                }
            }
        }
	}
}
