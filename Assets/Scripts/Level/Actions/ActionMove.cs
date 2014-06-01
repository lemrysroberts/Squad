using UnityEngine;
using System.Collections;

public class ActionMove : EntityAction
{
	private Vector2 m_target = Vector2.zero;

	public ActionMove(Vector2 target)
	{
		m_target = target;
	}

	public override void Start() { }

	public override void Update()
	{
		if(m_owner != null)
		{
			Vector2 direction = (m_target - m_owner.Position).normalized;
			m_owner.Position = m_owner.Position + direction * Time.deltaTime;
		}
	}

	public override void End()
	{

	}
	
	public override bool IsComplete()
	{
		return false;
	}
}
