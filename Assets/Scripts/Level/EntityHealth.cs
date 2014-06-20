using UnityEngine;
using System.Collections;

public class EntityHealth : MonoBehaviour
{
	public delegate void EntityKilled();

	public bool DamageEnabled 		= true;
	public float StartHealth 		= 100.0f;

	private Entity m_owner 			= null;
	private float m_health 			= 0.0f;

	private EntityKilled m_onKill 	= null;

	public float CurrentHealth { get { return m_health; } }

	public void SetOnKill(EntityKilled onKilled)
	{
		m_onKill = onKilled;
	}

	public void Start()
	{
		m_health = StartHealth;
	}

	public void ApplyDamage(float damageAmount, DamageType damageType)
	{
		if(!DamageEnabled) { return; }

		// TODO: Add modifiers for shields, armour, etc.
		m_health -= damageAmount;

		if(m_health <= 0.0f && m_onKill != null)
		{
			m_onKill();
		}
	}
}
