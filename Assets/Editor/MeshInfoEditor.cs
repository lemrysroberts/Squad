///////////////////////////////////////////////////////////
// 
// MeshInfoEditor.cs
//
// What it does: 
//
// Notes: 	
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(MeshInfo))] 
public class MeshInfoEditor : Editor
{
	public override void OnInspectorGUI()
	{
		MeshInfo info = (MeshInfo)target;

		MeshFilter filter = info.GetComponent<MeshFilter>();

		if(filter == null)
		{
			GUILayout.Label("No MeshFilter found.");
			return;
		}

		Mesh mesh = filter.sharedMesh;

		if(mesh == null)
		{
			GUILayout.Label("No Mesh found.");
			return;
		}

		foreach(var vertex in mesh.vertices)
		{
			GUILayout.Label(vertex.x.ToString("0.000") + ", " + vertex.y.ToString("0.00000000") + ", " + vertex.z.ToString("0.000"));
		}

		GUILayout.Label("UVs");

		foreach (var uv in mesh.uv)
		{
			GUILayout.Label(uv.x.ToString("0.000") + ", " + uv.y.ToString("0.000"));
		}

		GUILayout.Label("Vert Count: " + mesh.vertexCount);
		
	}
}
