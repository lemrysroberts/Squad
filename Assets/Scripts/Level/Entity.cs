using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Selectable))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(ActionSource))]
public class Entity : MonoBehaviour , ISelectable
{
	public float WeaponRange					= 5.0f;

	private Rigidbody2D m_rigidBody 			= null;
	private BoxCollider2D m_collider 			= null;
	private Selectable m_selectable 			= null;
	private SpriteRenderer m_spriteRenderer 	= null;
	private ActionSource m_actionSource			= null;

	private Queue<EntityAction> m_actionQueue 	= new Queue<EntityAction>();
	private Queue<EntityAction> m_planQueue		= new Queue<EntityAction>();

	private EntityAction m_currentAction		= null;

	// Use this for initialization
	void Start () 
	{
		GetComponent<ActionSource>().SetEntity(this);

		m_selectable = GetComponent<Selectable>();
		m_spriteRenderer = GetComponent<SpriteRenderer>();

		m_actionSource = GetComponent<ActionSource>();

		Sprite entitySprite = SpriteHelper.LoadSprite("Textures/test_sprite_0");
		m_spriteRenderer.sprite = entitySprite;

		m_selectable.Target = this;
	
		m_rigidBody = GetComponent<Rigidbody2D>();
		m_collider = GetComponent<BoxCollider2D>();
		m_actionSource = GetComponent<ActionSource>();
		m_collider.size = new Vector2(entitySprite.bounds.size.x, entitySprite.bounds.size.y);
		m_rigidBody.isKinematic = true;

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

	public Vector2 Position
	{
		get { return (Vector2)gameObject.transform.position; }
		set { gameObject.transform.position = new Vector3(value.x, value.y, gameObject.transform.position.z); }
	}
}
