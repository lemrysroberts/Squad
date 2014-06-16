using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Selectable))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(ActionSource))]
[RequireComponent(typeof(EntityHealth))]
public class Entity : MonoBehaviour , ISelectable
{
	public float PerceptionRange					= 5.0f;

	public float focusMinAngle						= -45.0f;
	public float focusMaxAngle						= 45.0f;

	private Rigidbody2D m_rigidBody 				= null;
	private BoxCollider2D m_collider 				= null;
	private Selectable m_selectable 				= null;
	private SpriteRenderer m_spriteRenderer 		= null;
	private ActionSource m_actionSource				= null;
	private Perception m_perception					= null;
	private EntityHealth m_health					= null;

	private Queue<EntityAction> m_actionQueue 		= new Queue<EntityAction>();
	private Queue<EntityAction> m_planQueue			= new Queue<EntityAction>();

	private EntityAction m_currentAction			= null;
	private Weapon m_currentWeapon					= null;
	private int m_teamID							= -1;

	public Perception EntityPerception 	{ get { return m_perception; } }
	public EntityHealth EntityHealth 	{ get { return m_health; } }

	public void SetTeamID(int TeamID) 	{ m_teamID = TeamID; }
	public int GetTeamID()	 			{ return m_teamID; }

	// Use this for initialization
	void Start () 
	{
		m_currentWeapon = new TestWeapon(this);

		GetComponent<ActionSource>().SetEntity(this);

		m_selectable 		= GetComponent<Selectable>();
		m_spriteRenderer 	= GetComponent<SpriteRenderer>();
		m_actionSource 		= GetComponent<ActionSource>();
		m_rigidBody 		= GetComponent<Rigidbody2D>();
		m_collider 			= GetComponent<BoxCollider2D>();
		m_health 			= GetComponent<EntityHealth>();

		Sprite entitySprite = SpriteHelper.LoadSprite("Textures/test_sprite_0");
		m_spriteRenderer.sprite = entitySprite;

		m_selectable.Target = this;
	
		m_rigidBody.isKinematic = false;
		m_rigidBody.gravityScale = 0;

		m_collider.size = new Vector2(entitySprite.bounds.size.x, entitySprite.bounds.size.y);

		m_actionSource = GetComponent<ActionSource>();

		GameObject actionObject = new GameObject("perception");
		actionObject.transform.parent = transform;
		actionObject.transform.localPosition = Vector3.zero;
		CircleCollider2D perceptionCollider = actionObject.AddComponent<CircleCollider2D>();
		perceptionCollider.isTrigger = true;
		perceptionCollider.radius = m_currentWeapon.Range;
		m_perception = actionObject.AddComponent<Perception>();
		m_perception.SetOwner(this);

		renderer.material.color = Color.white;
	}

	// Update is called once per frame
	void Update () 
	{
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

	public void AddActionToPlan(EntityAction newPlanAction)
	{
		m_planQueue.Enqueue(newPlanAction);
	}

	public void ResetPlan()
	{
		m_planQueue.Clear();
	}

	public void ExecutePlan()
	{
		if(m_actionSource == null || m_actionSource.Action == null)
		{
			return;
		}

		if(m_currentAction != null)
		{
			m_currentAction.End();
			m_currentAction = null;
		}

		m_actionQueue.Clear();

		ActionSource currentSource = m_actionSource;
		while(currentSource != null && currentSource.Action != null)
		{
			m_actionQueue.Enqueue(currentSource.Action);
			currentSource = currentSource.Action.ChildSource;
		}

		m_planQueue.Clear();
		Debug.Log("Executing plan " + m_actionQueue.Count);
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

	public Vector2 Position
	{
		get { return (Vector2)gameObject.transform.position; }
		set { gameObject.transform.position = new Vector3(value.x, value.y, gameObject.transform.position.z); }
	}

	public Weapon CurrentWeapon { get { return m_currentWeapon; } }
}
