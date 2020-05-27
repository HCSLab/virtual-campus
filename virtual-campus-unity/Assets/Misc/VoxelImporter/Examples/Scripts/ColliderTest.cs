using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
	public class ColliderTest : MonoBehaviour
	{
		public GameObject addObject;
		public enum Primitive
		{
			Random = -1,
			Sphere = 0,
			Capsule = 1,
			Cube = 3,
		}
		public Primitive primitive = Primitive.Random;
        //sepalate
        public bool autoBirth = true;
		public float sepalateTimeMin = 0.5f;
		public float sepalateTimeMax = 1f;
        //random
        public float randomRadius = 1f;
		public float randomScaleMin = 0.5f;
		public float randomScaleMax = 1.5f;
		//delete
		public float groundY = -10f;

		private float timer;
		private float timerBeforeBirth;
		private List<GameObject> createList = new List<GameObject>();
        private int count;

        void Update()
		{
			if (autoBirth)
			{
				float sepalatetime = Random.Range(sepalateTimeMin, sepalateTimeMax);
				if (timer - timerBeforeBirth > sepalatetime)
				{
					Add();
					timerBeforeBirth = timer;
				}
			}
			for (int i = 0; i < createList.Count; i++)
			{
				var o = createList[i];
                if(o == null)
                {
                    createList.RemoveAt(i--);
                    continue;
                }
                if (o.transform.position.y < groundY)
				{
					Destroy(o);
					createList.RemoveAt(i--);
					continue;
				}
			}
			timer += Time.deltaTime;
		}

		public void Add()
		{
			GameObject o = null;
			if (addObject != null)
            {
                o = GameObject.Instantiate<GameObject>(addObject);
			}
			else
			{
				PrimitiveType primitiveType;
				if (primitive == Primitive.Random)
				{
					switch (Random.Range(0, 3))
					{
					case 0: primitiveType = PrimitiveType.Sphere; break;
					case 1: primitiveType = PrimitiveType.Capsule; break;
					default: primitiveType = PrimitiveType.Cube; break;
					}
				}
				else
				{
					primitiveType = (PrimitiveType)primitive;
				}
				o = GameObject.CreatePrimitive(primitiveType);
			}
			{
				o.layer = gameObject.layer;
				o.transform.SetParent(transform);
				o.transform.localPosition = new Vector3(Random.Range(-1f, 1f) * randomRadius, Random.Range(-1f, 1f) * randomRadius, Random.Range(-1f, 1f) * randomRadius);
				o.transform.localRotation = Random.rotation;
				float scale = Random.Range(randomScaleMin, randomScaleMax);
				o.transform.localScale = new Vector3(scale, scale, scale);
				var rigidbody = o.AddComponent<Rigidbody>();
                var meshFilter = o.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    rigidbody.mass = scale * (meshFilter.sharedMesh.bounds.size.x * meshFilter.sharedMesh.bounds.size.y * meshFilter.sharedMesh.bounds.size.z);
                }
                else
                {
                    rigidbody.mass = scale;
                }
            }
            {
                o.name += count.ToString();
            }
			createList.Add(o);
            count++;
        }
	}
}