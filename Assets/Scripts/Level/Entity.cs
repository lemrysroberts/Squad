using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Selectable))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EntityHealth))]
public class Entity : Spawnable , ISelectable
{
	protected enum EntityState
	{
		Alive,
		Dead
	}

	public int PathingClearance						= 2;

	public float PerceptionRange					= 5.0f;
	public float focusMinAngle						= -45.0f;
	public float focusMaxAngle						= 45.0f;

	protected Rigidbody2D m_rigidBody 				= null;
	protected BoxCollider2D m_collider 				= null;
	protected Selectable m_selectable 				= null;
	protected SpriteRenderer m_spriteRenderer 		= null;
	protected Perception m_perception				= null;
	protected EntityHealth m_health					= null;
	protected GameObject m_perceptionGameObject		= null;
	protected CircleCollider2D m_perceptionCollider	= null;

	protected EntityAction m_currentAction			= null;
	protected Weapon m_currentWeapon				= null;
	protected int m_teamID							= -1;
	protected EntityState m_state					= EntityState.Alive;

	protected Queue<EntityAction> m_actionQueue 	= new Queue<EntityAction>();
	protected Queue<EntityAction> m_planQueue		= new Queue<EntityAction>();

	public Perception EntityPerception 	{ get { return m_perception; } }
	public EntityHealth EntityHealth 	{ get { return m_health; } }

	public void SetTeamID(int TeamID) 	{ m_teamID = TeamID; }
	public int GetTeamID()	 			{ return m_teamID; }

	public override void OnSpawn()
	{

	}

	public override void OnDespawn()
	{

	}

	// Use this for initialization
	protected virtual void Start () 
	{
		m_currentWeapon = new TestWeapon(this);

		m_selectable 		= GetComponent<Selectable>();
		m_spriteRenderer 	= GetComponent<SpriteRenderer>();

		m_rigidBody 		= GetComponent<Rigidbody2D>();
		m_collider 			= GetComponent<BoxCollider2D>();
		m_health 			= GetComponent<EntityHealth>();

		Sprite entitySprite = SpriteHelper.LoadSprite("Textures/test_sprite_0");
		m_spriteRenderer.sprite = entitySprite;

		m_selectable.Target = this;
	
		m_rigidBody.isKinematic = false;
		m_rigidBody.gravityScale = 0;

		m_collider.size = new Vector2(entitySprite.bounds.size.x, entitySprite.bounds.size.y);

		m_perceptionGameObject = new GameObject("perception");
		m_perceptionGameObject.transform.parent = transform;
		m_perceptionGameObject.transform.localPosition = Vector3.zero;
		m_perceptionCollider = m_perceptionGameObject.AddComponent<CircleCollider2D>();
		m_perceptionCollider.isTrigger = true;
		m_perceptionCollider.radius = m_currentWeapon.Range;

		m_perception = m_perceptionGameObject.AddComponent<Perception>();
		m_perception.SetOwner(this);

		m_health.SetOnKill(new EntityHealth.EntityKilled(EntityKilled));

		renderer.material.color = Color.white;
	}

	// Update is called once per frame
	void Update () 
	{
		if(m_state != EntityState.Alive) { return; }

		if(m_currentAction != null)
		{
			m_currentAction.Update();

			if(m_currentAction.IsComplete())
			{
				m_currentAction.End();
				m_currentAction = null;

				if(m_actionQueue.Count > 0)
				{
					m_currentAction = m_actionQueue.Dequeue();
					m_currentAction.Start();
				}
			}
		}
		else
		{
			if(m_actionQueue.Count > 0)
			{
				m_currentAction = m_actionQueue.Dequeue();
				m_currentAction.Start();
			}
		}
	}

	public void AddAction(EntityAction newAction)
	{
		m_actionQueue.Enqueue(newAction);
	}



	public bool Select()
	{
		Debug.Log("Selected");
		Material mat = renderer.material;

		if(mat != null)
		{
			mat.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		}

		return true; 
	}

	public void Deselect()
	{
		Material mat = renderer.material;
		
		if(mat != null) 
		{
			mat.color = Color.white;
		}
	}

	void OnGUI()
	{
		Vector2 position = Camera.main.WorldToScreenPoint(transform.position);
		position.y += 60.0f;
		position.x -= 20.0f;

		GUI.Label(new Rect(position.x, Screen.height - position.y, 200.0f, 50.0f), "Health: " + m_health.CurrentHealth);		
	}

	protected virtual void EntityKilled()
	{
		Debug.Log("Killed");
		m_state = EntityState.Dead;

	}

	public Vector2 Position
	{
		get { return (Vector2)gameObject.transform.position; }
		set { gameObject.transform.position = new Vector3(value.x, value.y, gameObject.transform.position.z); }
	}

	public Weapon CurrentWeapon { get { return m_currentWeapon; } }
}
