using UnityEngine;
using System;

namespace VoxelImporter
{
	public class PlayerController : MonoBehaviour
	{
		public Transform transformCache { get; protected set; }
		public CharacterController characterControllerCache { get; protected set; }
		public Animator animatorCache { get; protected set; }

		public float moveSpeed = 24f;
		public float rotateSpeed = 90f;

		protected int isWalkingID;

		public Vector3 velocity { get; protected set; }
		protected Vector3 addVelocity;

		protected GameObject weaponColliderClone;

		void Awake()
		{
			transformCache = transform;
			characterControllerCache = GetComponent<CharacterController>();
			animatorCache = GetComponent<Animator>();
            
			isWalkingID = Animator.StringToHash("IsWalking");
		}

        void FixedUpdate()
        {
            Vector3 forward = Vector3.zero;

            forward += Vector3.forward * Input.GetAxis("Vertical");
            forward += Vector3.right * Input.GetAxis("Horizontal");
            var movement = new Vector3(0, velocity.y, 0) + addVelocity;
            addVelocity = Vector3.zero;
            movement += forward * moveSpeed;
            characterControllerCache.Move(movement * Time.fixedDeltaTime);
            velocity = movement;
            if (forward.normalized.sqrMagnitude > 0f)
                transformCache.rotation = Quaternion.Lerp(transformCache.rotation, Quaternion.LookRotation(forward.normalized), 0.3f);
            animatorCache.SetBool(isWalkingID, (forward.sqrMagnitude > 0.01f));
        }
	}
}