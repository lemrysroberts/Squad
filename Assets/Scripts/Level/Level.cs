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

	[SerializeField]
	private GameObject m_gridObject = null;

	private static Level s_instance		= null;

	const float m_cellSize 				= 0.2f;

	private LevelGridRaycastHit m_raycastHit;
	private LevelComponent[] m_components;

	public static Level Instance { get { return s_instance; } }

	public LevelGrid GetGrid() { return m_grid; }

	public void Start () 
	{
		s_instance = this;

		m_raycastHit = new LevelGridRaycastHit();

		m_meshRenderer 	= GetComponent<MeshRenderer>();

		LevelLayout layout = LevelLayout.Deserialise(@"C:\Unity\Squad\Assets\Resources\Levels\newlevel.xml");

		LoadFromLayout(layout);

	}

	public void LoadFromLayout(LevelLayout layout)
	{
		m_grid 	= new LevelGrid(m_cellSize, 100, 100);
		m_walls	= new Walls(m_grid, layout);

		m_grid.RebuildGraphs();

		m_grid.RebuildMeshes();


		CreateGridObject();

		m_gridObject.transform.position = new Vector3(m_grid.GridStart.x, m_grid.GridStart.y, -1.0f);

	}

	public void OnDestroy()
	{
		if(m_gridObject != null) { GameObject.DestroyImmediate(m_gridObject); }
	}

	public void StartLevel()
	{
		Vector3 position = transform.position;
		position.z = 1;
		transform.position = position;

		m_components = GameObject.FindObjectsOfType<LevelComponent>();

		foreach(var component in m_components)
		{
			component.LevelStarted();
		}
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
		if(m_gridObject != null) { GameObject.DestroyImmediate(m_gridObject); }

		m_gridObject = new GameObject("Grid");
		m_gridObject.transform.position = m_meshRenderer.bounds.min + new Vector3(0.0f, 0.0f, -1.0f);
		
		m_debugGridRenderer 	= m_gridObject.AddComponent<MeshRenderer>();
		m_debugGridFilter 		= m_gridObject.AddComponent<MeshFilter>();

		m_debugGridRenderer.material = Resources.Load<Material>(GameData.Data_DebugGridMaterial);
		
		m_debugGridFilter.sharedMesh 			= m_grid.ContentsMesh;
	}

	private Vector2 rayStart = Vector2.zero;
	private Vector2 rayEnd = Vector2.zero;

	// TODO: Remove the ray-test code!
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Y))
		{
			m_grid.GetCell(10, 10).AddType(GridCellContentsType.Wall);
			m_grid.UpdateCellBlocked(10, 10, true);
			UpdateDebugMeshes();
		}

		/*
		if(Input.GetKeyDown(KeyCode.R))
		{
			Route route = m_grid.GetRoute(2);
			Debug.Log(route.m_routePoints.Count);
			for(int i = 0; i < route.m_routePoints.Count - 1; i++)
			{
				Debug.DrawLine((Vector3)route.m_routePoints[i].NodePosition  + new Vector3(0.0f, 0.0f, -1.0f), (Vector3)route.m_routePoints[i + 1].NodePosition  + new Vector3(0.0f, 0.0f, -1.0f), Color.magenta, 5.0f  );
			}
		}

		if(Input.GetKeyDown(KeyCode.T))
		{
			Route route = m_grid.GetRoute(1);
			Debug.Log(route.m_routePoints.Count);
			for(int i = 0; i < route.m_routePoints.Count - 1; i++)
			{
				Debug.DrawLine((Vector3)route.m_routePoints[i].NodePosition  + new Vector3(0.0f, 0.0f, -1.0f), (Vector3)route.m_routePoints[i + 1].NodePosition  + new Vector3(0.0f, 0.0f, -1.0f), Color.magenta, 5.0f  );
			}
		}
		*/

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
