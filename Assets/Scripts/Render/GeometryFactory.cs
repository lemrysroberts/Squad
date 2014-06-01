using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof (MeshFilter))]
public class GeometryFactory : MonoBehaviour 
{
	public enum GeometryType
	{
		Plane,	
		ScreenPlane
	}
	
	public GeometryType geometryType = GeometryType.Plane;
	public bool ScaleUVs = false;
	public float UVScale0 = 1.0f;
	public float UVScale1 = 1.0f;
	public Camera ScreenPlaneTargetCamera = null;
	public bool ForceRebuildOnStart = false;

	[SerializeField]
	private bool m_meshBuilt = false;

	[SerializeField]
	private Mesh m_mesh = null;

	private float m_height = 0.0f;

	// Use this for initialization
	void Awake () 
	{
		if (!m_meshBuilt || ForceRebuildOnStart)
		{
			RebuildMesh();	
		}
	}

	public void RebuildMesh()
	{
		MeshFilter mesh = GetComponent<MeshFilter>();

		if (mesh != null)
		{
			if (ScaleUVs)
			{
				switch (geometryType)
				{
					case GeometryType.Plane:
						{
							m_mesh = CreatePlane(transform.localScale.x * UVScale0, transform.localScale.z * UVScale0, transform.localScale.x * UVScale1, transform.localScale.z * UVScale1, m_height); 
							mesh.sharedMesh = m_mesh; 
							break;
						}

					case GeometryType.ScreenPlane:
						{
							m_mesh = CreateScreenPlane(ScreenPlaneTargetCamera, transform.localScale.x * UVScale0, transform.localScale.z * UVScale0, transform.localScale.x * UVScale1, transform.localScale.z * UVScale1, m_height); 
							mesh.sharedMesh = m_mesh;
							break;
						}
				}
			}
			else
			{
				switch (geometryType)
				{
					case GeometryType.Plane:
						{
							m_mesh = CreatePlane(1.0f, 1.0f, 1.0f, 1.0f, m_height);
							mesh.sharedMesh = m_mesh;
							break;
						}

					case GeometryType.ScreenPlane:
						{
							m_mesh = CreateScreenPlane(ScreenPlaneTargetCamera, 1.0f, 1.0f, 1.0f, 1.0f, m_height);
							mesh.sharedMesh = m_mesh;
							break;
						}
				}
			}
			m_meshBuilt = true;
		}
		else
		{
			Debug.LogError("Mesh is null");
		}
	}
	
	public static Mesh CreatePlane(float UVXScale0, float UVYScale0, float UVXScale1, float UVYScale1, float height)
	{
		Mesh newMesh = new Mesh();
		
		newMesh.name = "GeometryFactory:Plane";
		
		Vector3[] 	vertices 	= new Vector3[4];
		Vector3[] 	normals 	= new Vector3[4];
		Vector2[] 	uvs0 		= new Vector2[4];
		Vector2[] 	uvs1 		= new Vector2[4];
		int[] 		triangles 	= new int[6];
		
		vertices[0] = new Vector3(-0.5f, -0.5f, height);
		vertices[1] = new Vector3(0.5f, -0.5f, height);
		vertices[2] = new Vector3(-0.5f, 0.5f, height);
		vertices[3] = new Vector3(0.5f, 0.5f, height);
		
		normals[0] = new Vector3(0.0f, 0.0f, 1.0f);
		normals[1] = new Vector3(0.0f, 0.0f, 1.0f);
		normals[2] = new Vector3(0.0f, 0.0f, 1.0f);
		normals[3] = new Vector3(0.0f, 0.0f, 1.0f);
		
		uvs0[0] = new Vector2(0.0f, 0.0f);
		uvs0[1] = new Vector2(UVXScale0, 0.0f);
		uvs0[2] = new Vector2(0.0f, UVYScale0);
		uvs0[3] = new Vector2(UVXScale0, UVYScale0);
		
		uvs1[0] = new Vector2(0.0f, 0.0f);
		uvs1[1] = new Vector2(UVXScale1, 0.0f);
		uvs1[2] = new Vector2(0.0f, UVYScale1);
		uvs1[3] = new Vector2(UVXScale1, UVYScale1);
		
		triangles[0] = 0;
		triangles[1] = 2;
		triangles[2] = 1;
		triangles[3] = 1;
		triangles[4] = 2;
		triangles[5] = 3;
		
		newMesh.vertices = vertices;
		newMesh.normals = normals;
		newMesh.uv = uvs0;
		newMesh.uv1 = uvs1;
		newMesh.triangles = triangles;
		
		return newMesh;
	}
	
	public static Mesh CreateScreenPlane(Camera targetCamera, float UVXScale0, float UVYScale0, float UVXScale1, float UVYScale1, float vertHeight)
	{
		if(targetCamera == null)
		{
			Debug.LogWarning("Target Camera not set when creating screen-plane. Defaulting to plane.");
			return CreatePlane(UVXScale0, UVYScale0, UVXScale1, UVYScale1, vertHeight);
		}
		
		Mesh newMesh = new Mesh();
		
		newMesh.name = "GeometryFactory:ScreenPlane";
		
		float width = targetCamera.orthographicSize / 2.0f;
		float height = (targetCamera.orthographicSize) / 2.0f;
		
		Vector3[] 	vertices 	= new Vector3[4];
		Vector3[] 	normals 	= new Vector3[4];
		Vector2[] 	uvs0 		= new Vector2[4];
		Vector2[] 	uvs1 		= new Vector2[4];
		int[] 		triangles 	= new int[6];
		
		vertices[0] = new Vector3(-width, -height, vertHeight);
		vertices[1] = new Vector3(width, -height, vertHeight);
		vertices[2] = new Vector3(-width, height, vertHeight);
		vertices[3] = new Vector3(width, height, vertHeight);
		
		normals[0] = new Vector3(0.0f, 0.0f, 1.0f);
		normals[1] = new Vector3(0.0f, 0.0f, 1.0f);
		normals[2] = new Vector3(0.0f, 0.0f, 1.0f);
		normals[3] = new Vector3(0.0f, 0.0f, 1.0f);
		
		uvs0[0] = new Vector2(0.0f, 0.0f);
		uvs0[1] = new Vector2(UVXScale0, 0.0f);
		uvs0[2] = new Vector2(0.0f, UVYScale0);
		uvs0[3] = new Vector2(UVXScale0, UVYScale0);
		
		uvs1[0] = new Vector2(0.0f, 0.0f);
		uvs1[1] = new Vector2(UVXScale1, 0.0f);
		uvs1[2] = new Vector2(0.0f, UVYScale1);
		uvs1[3] = new Vector2(UVXScale1, UVYScale1);
		
		triangles[0] = 0;
		triangles[1] = 2;
		triangles[2] = 1;
		triangles[3] = 1;
		triangles[4] = 2;
		triangles[5] = 3;
		
		newMesh.vertices = vertices;
		newMesh.normals = normals;
		newMesh.uv = uvs0;
		newMesh.uv1 = uvs1;
		newMesh.triangles = triangles;
		
		return newMesh;
	}
}
