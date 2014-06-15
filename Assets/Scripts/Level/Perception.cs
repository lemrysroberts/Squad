using UnityEngine;
using System.Collections.Generic;

public class Perception : MonoBehaviour 
{
	private Entity m_owner = null;

	private List<Entity> m_entitiesInPerception = new List<Entity>();
	private List<Entity> m_enemiesInPerception 	= new List<Entity>();

	public List<Entity> Enemies { get { return m_enemiesInPerception; } }

	public void SetOwner(Entity owner)
	{
		m_owner = owner;
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		Debug.Log("Entered perception");

		Entity entity = collider.gameObject.GetComponent<Entity>();
		if(entity != null)
		{
			m_entitiesInPerception.Add(entity);

			if(entity.GetTeamID() != m_owner.GetTeamID())
			{
				m_enemiesInPerception.Add(entity);
			}
		}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		Entity entity = collider.gameObject.GetComponent<Entity>();
		if(entity != null)
		{
			m_entitiesInPerception.Remove(entity);

			if(entity.GetTeamID() != m_owner.GetTeamID())
			{
				m_enemiesInPerception.Remove(entity);
			}
		}
	}

	void Update()
	{
	}

	void OnGUI()
	{
		Vector2 position = Camera.main.WorldToScreenPoint(transform.position);
		position.y -= 10.0f;
		position.x -= 20.0f;

		GUI.Label(new Rect(position.x, Screen.height - position.y, 200.0f, 50.0f), "Entities: " + m_entitiesInPerception.Count);
	}

}
