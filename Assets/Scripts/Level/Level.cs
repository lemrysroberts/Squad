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
		
		filter.mesh 			= m_grid.ContentsMesh;
	}

	private Vector2 rayStart = Vector2.zero;
	private Vector2 rayEnd = Vector2.zero;

	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			rayStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		if(Input.GetMouseButtonDown(1))
		{
			rayEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}


		int mask = (int)LevelGrid.GridCellContents.Blocked;
		LevelGrid.GridCell hitCell;

		bool hit = m_grid.Raycast(rayStart, rayEnd, mask, out hitCell);
	}

	void OnGUI()
	{
		if(GUI.Button(new Rect(300.0f, 20.0f, 200.0f, 50.0f), "Ray test"))
		{
			int mask = (int)LevelGrid.GridCellContents.Blocked;
			LevelGrid.GridCell hitCell;

		//	bool hit = m_grid.Raycast(mask, out hitCell);
		}

		int.TryParse( GUI.TextField(new Rect(600.0f, 10.0f, 60.0f, 40.0f), rayIterations.ToString()), out rayIterations);
	}

	private int rayIterations = 20;
}
