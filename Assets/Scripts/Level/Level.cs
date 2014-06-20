using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
public class Level : MonoBehaviour 
{
	private MeshRenderer m_meshRenderer = null;
	private LevelGrid m_grid 			= null;

	const float m_cellSize 				= 0.2f;

	void Start () 
	{
		m_meshRenderer 	= GetComponent<MeshRenderer>();

		m_grid 			= new LevelGrid(m_cellSize, m_meshRenderer.bounds.min, m_meshRenderer.bounds.max);

		CreateGridObject();

	}

	public void StartLevel()
	{
		Vector3 position = transform.position;
		position.z = 1;
		transform.position = position;
	}

	private void CreateGridObject()
	{
		GameObject gridObject = new GameObject("Grid");
		gridObject.transform.position = m_meshRenderer.bounds.min + new Vector3(0.0f, 0.0f, -1.0f);
		
		MeshRenderer renderer 	= gridObject.AddComponent<MeshRenderer>();
		MeshFilter filter 		= gridObject.AddComponent<MeshFilter>();

		renderer.material = Resources.Load<Material>(GameData.Data_DebugGridMaterial);
		
		filter.mesh 			= m_grid.DebugMesh;
	}
}
 