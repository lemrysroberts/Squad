using UnityEngine;
using System.Collections.Generic;

public class ActionAim : EntityAction
{
	private enum PlanProgress
	{
		StartEdge,
		EndEdge,
		Complete
	}

	private GameObject m_startBarObject = null;
	private GameObject m_endBarObject 	= null;
	private GameObject m_wedgeObject 	= null;

	private LineRenderer m_lineRenderer	= null;

	private Mesh m_wedgeMesh;
	private MeshFilter m_wedgeFilter;
 
	private float m_startAngle 			= 0.0f;
	private float m_endAngle 			= 0.0f;
	private Vector2 m_currentHoverPoint = Vector2.zero;
	private PlanProgress m_progress 	= PlanProgress.StartEdge;
	
	public ActionAim()
	{
		m_primaryMouseButton = 1;
	}

	public override void Start()
	{
		if(m_entity.CurrentWeapon != null)
		{
			m_entity.CurrentWeapon.SetShotRegion(m_startAngle, m_endAngle);

			m_entity.CurrentWeapon.StartFiring();
		}
	}

	public override void Update()
	{
		if(m_entity.CurrentWeapon != null)
		{
			m_entity.CurrentWeapon.UpdateFiring();
		}
	}
	
	public override void End()
	{
		if(m_entity.CurrentWeapon != null)
		{
			m_entity.CurrentWeapon.StopFiring();
		}

		if(m_startBarObject != null) { GameObject.Destroy(m_startBarObject); }
		if(m_endBarObject != null) { GameObject.Destroy(m_endBarObject); }
		if(m_wedgeObject != null) {GameObject.Destroy(m_wedgeObject); m_wedgeFilter = null; }

		m_lineRenderer = null;
	}
	
	public override bool IsComplete()
	{
		return false;
	}
	
	public override void StartPlanning() 
	{ 
		m_startBarObject = new GameObject("plan0");
		m_lineRenderer = m_startBarObject.AddComponent<LineRenderer>();
		
		m_lineRenderer.SetWidth(0.05f, 0.05f);
		m_lineRenderer.SetPosition(0, (Vector3)(m_owner.transform.position));
		
		// Clear an old plan if it exists
		if(m_owner != null)
		{
			Debug.Log("Starting plan");
		//	m_owner.GetEntity().ResetPlan();
		}
	}
	
	public override void EndPlanning() 
	{ 
		if(m_progress == PlanProgress.Complete)
		{
		//	m_owner.GetEntity().AddActionToPlan(this);
		}
		else
		{
			if(m_startBarObject != null) { GameObject.Destroy(m_startBarObject); }
			if(m_endBarObject != null) { GameObject.Destroy(m_endBarObject); }
			if(m_wedgeObject != null) {GameObject.Destroy(m_wedgeObject); m_wedgeFilter = null; }
			
			m_lineRenderer = null;
		}
	}

	public override void DeletePlanning()
	{
		GameObject.Destroy(m_startBarObject);
		GameObject.Destroy(m_endBarObject);
		GameObject.Destroy(m_wedgeObject);
	}
	
	public override void CancelPlanning() 
	{ 
		Debug.Log("Deleting ActionAim");

		if(m_startBarObject != null) { GameObject.Destroy(m_startBarObject); }
		if(m_endBarObject != null) { GameObject.Destroy(m_endBarObject); }
		if(m_wedgeObject != null) {GameObject.Destroy(m_wedgeObject); m_wedgeFilter = null; }
		
		m_lineRenderer = null;
	}
	
	public override void UpdateHoverLocation(Vector2 location) 
	{ 
		m_currentHoverPoint = location;

		Vector2 direction = location - ((Vector2)m_owner.transform.position);
		m_lineRenderer.SetPosition(1, (m_owner.transform.position + ((Vector3)direction.normalized * m_owner.GetEntity().CurrentWeapon.Range)));

		if(m_progress == PlanProgress.EndEdge)
		{
			const int subdivisions = 50; 

			int vertCount 	= subdivisions + 1;
			int indexCount 	= 3 * (subdivisions - 1);

			Vector3 currentPos = m_owner.transform.position + ((Vector3)direction.normalized * m_owner.GetEntity().CurrentWeapon.Range);

			m_endAngle = Mathf.Atan2(direction.normalized.y, direction.normalized.x) * Mathf.Rad2Deg;

			Vector3[] verts = new Vector3[vertCount];
			Vector3[] normals = new Vector3[vertCount];
			Vector2[] uvs = new Vector2[vertCount];


			verts[0] = m_owner.transform.position;
			uvs[0] = (Vector2)m_owner.transform.position;
			normals[0] = Vector3.back;

			for(int i = 0; i < subdivisions; i++)
			{
						float newAngle = Mathf.LerpAngle(m_startAngle, m_endAngle, (1.0f / (float)(subdivisions - 1)) * (float)i); 

				Vector3 newVector = Quaternion.Euler(0.0f, 0.0f, (newAngle)) * Vector3.right;

				verts[i + 1] = (Vector3)m_owner.transform.position + (newVector * m_owner.GetEntity().CurrentWeapon.Range);
				normals[i + 1] = Vector3.back;
				uvs[i + 1] = (Vector2)(verts[i + 1]);
			}

			int[] indices = new int[indexCount];

			for(int i = 0; i < subdivisions - 1; i++)
			{
				indices[i * 3] = 0;
				indices[i * 3 + 1] = i + 1;
				indices[i * 3 + 2] = i + 2;
			}
			
			m_wedgeMesh.vertices = verts;
			m_wedgeMesh.uv = uvs;
			m_wedgeMesh.normals = normals;
			m_wedgeMesh.SetIndices(indices, MeshTopology.Triangles, 0);

			m_wedgeMesh.RecalculateBounds();

			m_wedgeFilter.mesh = null;
			m_wedgeFilter.mesh = m_wedgeMesh;
		}
	}
	
	public override bool Commit() 
	{
		switch(m_progress)
		{
			case PlanProgress.StartEdge:
			{
				m_endBarObject = new GameObject("plan1");
				m_lineRenderer = m_endBarObject.AddComponent<LineRenderer>();
				
				m_lineRenderer.SetWidth(0.05f, 0.05f);
				m_lineRenderer.SetPosition(0, (Vector3)(m_owner.transform.position));
				m_progress = PlanProgress.EndEdge;

				Vector2 direction = m_currentHoverPoint - (Vector2)m_owner.transform.position;
				m_startAngle = Mathf.Atan2(direction.normalized.y, direction.normalized.x) * Mathf.Rad2Deg;
			
				m_wedgeMesh = new Mesh();

				m_wedgeObject = new GameObject("wedge");
				MeshRenderer renderer = m_wedgeObject.AddComponent<MeshRenderer>();
				m_wedgeFilter = m_wedgeObject.AddComponent<MeshFilter>();
				Material newMaterial = Resources.Load<Material>(GameData.Data_AimMaterial);
				renderer.material = newMaterial;

				GlobalTextureScroll scroll = m_wedgeObject.AddComponent<GlobalTextureScroll>();
				scroll.TargetTexture = "_MainTex";
				scroll.UScroll = -0.01f;
				scroll.VScroll = -0.005f;
				m_wedgeFilter.mesh = m_wedgeMesh;

				break;
			}
			case PlanProgress.EndEdge: 

			{
				m_progress = PlanProgress.Complete;
				EndPlanning();
				InputManager.Instance.SetActiveListener(null);
				//m_childAction = new ActionAim();
				//m_childAction.SetOwner(m_owner);

				return true;
			}
		}
		
		return false;
	}
	
}
