using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
public class Level : MonoBehaviour 
{
	private MeshRenderer m_meshRenderer = null;
	private LevelGrid m_grid 			= null;
	private Walls m_walls				= null;

	private MeshRenderer m_debugGridRenderer = null;
	private MeshFilter m_debugGridFilter = null;

	private static Level s_instance		= null;

	const float m_cellSize 				= 0.2f;

	private LevelGridRaycastHit m_raycastHit;

	public static Level Instance { get { return s_instance; } }

	public LevelGrid GetGrid() { return m_grid; }

	void Start () 
	{
		s_instance = this;

		m_raycastHit = new LevelGridRaycastHit();

		m_meshRenderer 	= GetComponent<MeshRenderer>();

		m_grid 			= new LevelGrid(m_cellSize, m_meshRenderer.bounds.min, m_meshRenderer.bounds.max);
		m_walls			= new Walls(m_grid);

		m_grid.RebuildMeshes();

		CreateGridObject();

	}

	public void StartLevel()
	{
		Vector3 position = transform.position;
		position.z = 1;
		transform.position = position;
	}

	public void UpdateDebugMeshes()
	{
		m_grid.RebuildMeshes();

		if(m_debugGridFilter != null)
		{
			m_debugGridFilter.sharedMesh = null;
			m_debugGridFilter.sharedMesh = m_grid.ContentsMesh;
		}
	}

	private void CreateGridObject()
	{
		GameObject gridObject = new GameObject("Grid");
		gridObject.transform.position = m_meshRenderer.bounds.min + new Vector3(0.0f, 0.0f, -1.0f);
		
		m_debugGridRenderer 	= gridObject.AddComponent<MeshRenderer>();
		m_debugGridFilter 		= gridObject.AddComponent<MeshFilter>();

		m_debugGridRenderer.material = Resources.Load<Material>(GameData.Data_DebugGridMaterial);
		
		m_debugGridFilter.sharedMesh 			= m_grid.ContentsMesh;
	}

	private Vector2 rayStart = Vector2.zero;
	private Vector2 rayEnd = Vector2.zero;

	// TODO: Remove the ray-test code!
	void Update()
	{
		if(Input.GetMouseButton(0))
		{
			rayStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		if(Input.GetMouseButton(1))
		{
			rayEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		int mask = (1 << (int)GridCellContentsType.Wall);
		GridCell hitCell;

		bool hit = m_grid.Raycast(rayStart, rayEnd, mask, ref m_raycastHit);

		Debug.DrawLine(rayStart, rayEnd, Color.magenta);

		if(hit)
		{
			Vector2 hitLocation;

			m_grid.GetCellLocation(m_raycastHit.x, m_raycastHit.y, out hitLocation);

			Debug.DrawLine(hitLocation, hitLocation + new Vector2(m_cellSize, m_cellSize), Color.green);
			Debug.DrawLine(hitLocation + new Vector2(0.0f, m_cellSize), hitLocation + new Vector2(m_cellSize, 0.0f), Color.green);
		}
	}

	void LateUpdate()
	{
		m_walls.LateUpdate();
	}

	private int rayIterations = 20;
}
