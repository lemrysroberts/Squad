using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Selectable))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Entity : MonoBehaviour , ISelectable
{
	private Rigidbody2D m_rigidBody 			= null;
	private BoxCollider2D m_collider 			= null;
	private Selectable m_selectable 			= null;
	private SpriteRenderer m_spriteRenderer 	= null;

	private Queue<EntityAction> m_actionQueue 	= new Queue<EntityAction>();
	private EntityAction m_currentAction		= null;

	// Use this for initialization
	void Start () 
	{

		m_selectable = GetComponent<Selectable>();
		m_spriteRenderer = GetComponent<SpriteRenderer>();

		Sprite entitySprite = SpriteHelper.LoadSprite("Textures/test_sprite_0");
		m_spriteRenderer.sprite = entitySprite;

		m_selectable.Target = this;
	
		m_rigidBody = GetComponent<Rigidbody2D>();
		m_collider = GetComponent<BoxCollider2D>();
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

				if(m_actionQueue.Count > 0)
				{
					m_currentAction = m_actionQueue.Dequeue();
					m_currentAction.Start();
				}
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

	public Vector2 Position
	{
		get { return (Vector2)gameObject.transform.position; }
		set { gameObject.transform.position = new Vector3(value.x, value.y, gameObject.transform.position.z); }
	}
}
