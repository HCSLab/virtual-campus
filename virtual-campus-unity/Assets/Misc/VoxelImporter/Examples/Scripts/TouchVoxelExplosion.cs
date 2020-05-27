using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
	public class TouchVoxelExplosion : MonoBehaviour
	{
        public float lifeTime = 1f;
        public bool realTimeBake = true;
        public bool rebirth = true;

        void Update()
        {
            bool explosion = false;
            Vector3 position = Vector3.zero;
            if (Input.GetMouseButton(0))
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
                    var colliders = Physics.OverlapSphere(hit.point, 1f);
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        var skinnedVoxelExplosion = colliders[i].GetComponent<VoxelSkinnedAnimationObjectExplosion>();
                        if (skinnedVoxelExplosion != null && realTimeBake)
                        {
                            if (!skinnedVoxelExplosion.enabled)
                            {
                                var rigidbody = colliders[i].GetComponent<Rigidbody>();
                                var rigidbodyEnabled = false;
                                if (rigidbody != null)
                                {
                                    rigidbodyEnabled = rigidbody.isKinematic;
                                    rigidbody.isKinematic = true;
                                }
                                var collider = colliders[i];
                                collider.enabled = false;

                                skinnedVoxelExplosion.SetExplosionCenter(skinnedVoxelExplosion.transform.worldToLocalMatrix.MultiplyPoint3x4(hit.point));

                                var animator = collider.GetComponent<Animator>();
                                var animatorEnabled = false;
                                if (animator != null)
                                {
                                    animatorEnabled = animator.enabled;
                                    animator.enabled = false;
                                }
                                skinnedVoxelExplosion.BakeExplosionPlay(lifeTime, () =>
                                {
                                    if (rebirth)
                                    {
                                        skinnedVoxelExplosion.ExplosionReversePlay(lifeTime, () =>
                                        {
                                            if (animator != null) animator.enabled = animatorEnabled;
                                            if (rigidbody != null) rigidbody.isKinematic = rigidbodyEnabled;
                                            collider.enabled = true;
                                        });
                                    }
                                    else
                                    {
                                        Destroy(skinnedVoxelExplosion.gameObject);
                                    }
                                });
                            }
                        }
                        else
                        {
                            var voxelExplosion = colliders[i].GetComponent<VoxelBaseExplosion>();
                            if (voxelExplosion == null) continue;

                            if (!voxelExplosion.enabled)
                            {
                                var rigidbody = colliders[i].GetComponent<Rigidbody>();
                                var rigidbodyEnabled = false;
                                if (rigidbody != null)
                                {
                                    rigidbodyEnabled = rigidbody.isKinematic;
                                    rigidbody.isKinematic = true;
                                }
                                var collider = colliders[i];
                                collider.enabled = false;

                                voxelExplosion.SetExplosionCenter(voxelExplosion.transform.worldToLocalMatrix.MultiplyPoint3x4(hit.point));

                                voxelExplosion.ExplosionPlay(lifeTime, () =>
                                {
                                    if (rebirth)
                                    {
                                        voxelExplosion.ExplosionReversePlay(lifeTime, () =>
                                        {
                                            if (rigidbody != null) rigidbody.isKinematic = rigidbodyEnabled;
                                            collider.enabled = true;
                                        });
                                    }
                                    else
                                    {
                                        Destroy(voxelExplosion.gameObject);
                                    }
                                });
                            }
                        }
                    }
                }
            }
        }
	}
}
