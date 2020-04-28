using UnityEngine;
using System.Collections;

namespace VoxelImporter
{
	public class CameraController : MonoBehaviour
	{
		public Transform transformCache { get; protected set; }

		public Transform transformLookAt;

		protected Vector3 defaultPosition;

		void Awake()
		{
			transformCache = transform;
			defaultPosition = transformCache.position - transformLookAt.position;
		}

		void Update()
		{
			var pos = transformLookAt.position + defaultPosition;
			transformCache.position = new Vector3(pos.x, transformCache.position.y, pos.z);
		}
	}
}
