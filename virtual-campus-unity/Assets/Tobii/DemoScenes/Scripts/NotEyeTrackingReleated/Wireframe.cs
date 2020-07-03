using UnityEngine;
using System.Collections.Generic;
using Tobii.Gaming;

public class Wireframe : MonoBehaviour
{
	public Color active;
	public Color inactive;
	private Color _current;

	private Material _material;
	private SkinnedMeshRenderer _skin;
	private Mesh _mesh;

	void Start()
	{
		_material = new Material(Shader.Find("GUI/Text Shader"));
		_skin = GetComponent<SkinnedMeshRenderer>();
		_current = inactive;
		_mesh = new Mesh();
	}

	void Update()
	{
		var presence = TobiiAPI.GetUserPresence();
		if (presence.IsUserPresent())
		{
			_current = Color.Lerp(_current, active, Time.deltaTime * 5);
		}
		else
		{
			_current = Color.Lerp(_current, inactive, Time.deltaTime * 5);
		}
	}

	void LateUpdate()
	{
		_skin.BakeMesh(_mesh);
	}

	void OnRenderObject()
	{
		var vertices = _mesh.vertices;
		var triangles = _mesh.triangles;

		GL.PushMatrix();
		_material.SetPass(0);
		GL.Begin(GL.LINES);

		GL.Color(_current);
		var linesArray = new List<Vector3>();
		for (int i = 0; i < triangles.Length / 3; i++)
		{
			linesArray.Add(transform.parent.TransformPointUnscaled(vertices[triangles[i * 3]]));
			linesArray.Add(transform.parent.TransformPointUnscaled(vertices[triangles[i * 3 + 1]]));
			linesArray.Add(transform.parent.TransformPointUnscaled(vertices[triangles[i * 3 + 2]]));
		}

		var lines = linesArray.ToArray();

		for (int i = 0; i < lines.Length / 3; i++)
		{
			GL.Vertex(lines[i * 3]);
			GL.Vertex(lines[i * 3 + 1]);

			GL.Vertex(lines[i * 3 + 1]);
			GL.Vertex(lines[i * 3 + 2]);

			GL.Vertex(lines[i * 3 + 2]);
			GL.Vertex(lines[i * 3]);
		}

		GL.End();
		GL.PopMatrix();
	}
}

public static class TransformExtensions
{

	public static Vector3 TransformPointUnscaled(this Transform transform, Vector3 position)
	{
		var localToWorldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		return localToWorldMatrix.MultiplyPoint3x4(position);
	}

}
