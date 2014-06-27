using UnityEngine;
using System.Collections.Generic;

public static class test
{
	public static int count = 0;
}

public class ActionMove : EntityAction
{
	private Vector2 m_origin = Vector2.zero;
	private GameObject m_planObject = null;
	private LineRenderer m_lineRenderer = null;
	private GameObject m_endActionSource = null;
	private List<Vector2> m_waypoints = new List<Vector2>();

	private Vector2 m_currentHoverPoint = Vector2.zero;
	private int m_wayPointIndex = 0;
	private float MoveSpeed = 2.0f;
	int val = 0;
	public ActionMove(Vector2 origin)
	{
		m_origin = origin;
		m_primaryMouseButton = 0;
	}

	public override void Start() 
	{

	}

	public override void Update()
	{
		Debug.Log("Ticking");
		if(m_owner.GetEntity() != null && m_wayPointIndex < m_waypoints.Count)
		{
			Vector2 direction = (m_waypoints[m_wayPointIndex] - (Vector2)m_owner.GetEntity().transform.position);
			m_owner.GetEntity().transform.position = m_owner.GetEntity().transform.position + (Vector3)direction.normalized * Time.deltaTime * MoveSpeed;

			if(direction.sqrMagnitude < 0.1f)
			{
				m_wayPointIndex++;
			}
		}
	}

	public override void End()
	{
		if(m_planObject != null)
		{
			m_lineRenderer = null;
			GameObject.Destroy(m_planObject);
			if(m_endActionSource != null) 	{ GameObject.Destroy(m_endActionSource); }
		}

	}
	
	public override bool IsComplete()
	{
		return m_wayPointIndex >= m_waypoints.Count;
	}

	public override void StartPlanning() 
	{

		Debug.Log("Starting planning... " + val);
		val = test.count;
		test.count = test.count + 1;

		DeletePlanning();

		m_planObject = new GameObject("plan");
		m_lineRenderer = m_planObject.AddComponent<LineRenderer>();

		m_lineRenderer.SetWidth(0.1f, 0.1f);
		m_lineRenderer.SetPosition(0, (Vector3)(m_origin));
		m_lineRenderer.SetPosition(1, (Vector3)(m_origin));

		m_lineRenderer.material = Resources.Load<Material>(GameData.Data_PathMaterial);

		// Clear an old plan if it exists
		if(m_owner != null)
		{
			//m_owner.GetEntity().ResetPlan();
		}
	}

	public override void EndPlanning() 
	{ 
		if(m_waypoints.Count == 0 && m_planObject != null)
		{
			m_lineRenderer = null;
			GameObject.Destroy(m_planObject);
			
		}
		else
		{
			m_lineRenderer.SetVertexCount(m_waypoints.Count + 1);
			if(m_owner != null)
			{
				//m_owner.GetEntity().AddActionToPlan(this);
			}
		}
		Debug.Log("Ending planning " + val);
	}

	public override void CancelPlanning() 
	{ 

	}

	public override void DeletePlanning()
	{
		Debug.Log("Deleting move");
		m_waypoints.Clear();

		if(m_planObject != null) 		{ GameObject.Destroy(m_planObject); }
		if(m_endActionSource != null) 	{ GameObject.Destroy(m_endActionSource); }

		if(m_childSource != null && m_childSource.Action != null)
		{
			Debug.Log("Deleting child...");
			m_childSource.Action.DeletePlanning();
		}
	}

	public override void UpdateHoverLocation(Vector2 location) 
	{ 
		m_currentHoverPoint = location;
		m_lineRenderer.SetPosition(m_waypoints.Count + 1, (Vector3)location);

		Vector2 entityPosition = GetEntity().Position;

		Route route = Level.Instance.GetGrid().GetRoute(entityPosition, location, 2);

		float offset = 0.2f;
		Vector2 offsetVec = new Vector2(offset, offset);

		if(route != null)
		{
			Level.Instance.GetGrid().RenderDebugRoute(route);
		}
	}

	public override bool Commit() 
	{
		m_waypoints.Add(m_currentHoverPoint);
		m_lineRenderer.SetVertexCount(m_waypoints.Count + 2);




		m_endActionSource = new GameObject("end_point");
		MeshRenderer renderer = m_endActionSource.AddComponent<MeshRenderer>();
		GeometryFactory factory = m_endActionSource.AddComponent<GeometryFactory>();
		Rigidbody2D rigidBody = m_endActionSource.AddComponent<Rigidbody2D>();
		rigidBody.isKinematic = true;
		BoxCollider2D collider = m_endActionSource.AddComponent<BoxCollider2D>();
		collider.isTrigger = true;
		m_endActionSource.transform.position = (Vector3)m_currentHoverPoint;
		m_endActionSource.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
		m_childSource = m_endActionSource.AddComponent<ActionSource>();
		m_childSource.m_parentAction = this;
		renderer.material = Resources.Load<Material>(GameData.Data_NodeMaterial);

		m_childSource.SetEntity(m_owner.GetEntity());

		EndPlanning();

		// Fake it for now
		EntityAction newAction = m_childSource.StartAction(0);

		InputManager.Instance.SetActiveListener(m_childSource);

		return true;
	}  // Return true if the action planning is finished
	

}
