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
	private float MoveSpeed = 1.5f;
	int val = 0;

	private bool m_hoverpointValid = false;
	private Route m_route = null;
	private int m_currentRouteIndex = 0;
	private float m_lerpRate = 0.0f;
	private float m_lerpProgress = 0.0f;
	private Vector2 m_currentDirection = Vector2.zero;

	public ActionMove(Vector2 origin)
	{
		m_origin = origin;
		m_primaryMouseButton = 0;
	}

	public override void Start() 
	{
		if(m_route != null && m_route.m_routePoints.Count > 1)
		{
			m_currentDirection = m_route.m_routePoints[1].NodePosition - m_route.m_routePoints[0].NodePosition;
			m_lerpRate = MoveSpeed / m_currentDirection.magnitude;
			m_currentRouteIndex = 1;
		}
	}

	public override void Update()
	{
		if(m_owner.GetEntity() != null && m_route != null && m_currentRouteIndex + 1 < m_route.m_routePoints.Count)
		{
			m_lerpProgress += m_lerpRate * Time.deltaTime;

			Vector3 startVec = m_route.m_routePoints[m_currentRouteIndex].NodePosition + m_route.OffsetVector;
			Vector3 endVec = m_route.m_routePoints[m_currentRouteIndex + 1].NodePosition + m_route.OffsetVector;

			// As entities can sit off-cell, add the off-cell bit if this is the last node
			if(m_currentRouteIndex + 1 == m_route.m_routePoints.Count - 1)
			{
				endVec += (Vector3)m_route.FinalOffset;
			}

			m_owner.GetEntity().transform.position = Vector3.Lerp(startVec, endVec, m_lerpProgress); 

			if(m_lerpProgress >= 1.0f)
			{
				if(m_currentRouteIndex + 2 < m_route.m_routePoints.Count)
				{
					m_currentRouteIndex++;

					m_currentDirection = (m_route.m_routePoints[m_currentRouteIndex + 1].NodePosition - m_route.m_routePoints[m_currentRouteIndex].NodePosition);
					if(m_currentRouteIndex + 2 == m_route.m_routePoints.Count)
					{
						Debug.Log("Offset");
						m_currentDirection += m_route.FinalOffset;
					}


					m_lerpRate = MoveSpeed / m_currentDirection.magnitude;

							m_lerpProgress = 0.0f;
				}
				else
				{
					m_currentDirection = Vector2.zero;
					m_route = null;
				}
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

		Route route = Level.Instance.GetGrid().GetRoute(entityPosition, location, GetEntity().PathingClearance);

		Debug.Log("Points: " + route.m_routePoints.Count);

		float offset = 0.2f;
		Vector2 offsetVec = new Vector2(offset, offset);

		m_hoverpointValid = false;

		if(route != null)
		{
			if(route.m_routePoints.Count > 0)
			{
				m_hoverpointValid = true;
				m_route = route;
			}

			Level.Instance.GetGrid().RenderDebugRoute(route);
		}
	}

	public override bool Commit() 
	{
		if(!m_hoverpointValid) { return false; }

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
