using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ActionSource))]
public class SquadMember : Entity
{
	private ActionSource m_actionSource				= null;


	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();
		m_actionSource = GetComponent<ActionSource>();

		m_actionSource.SetEntity(this);

		m_actionSource 		= GetComponent<ActionSource>();
		TestWeapon weapon = (TestWeapon)m_currentWeapon;

		weapon.AlwaysFiring = true;
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
		if(m_actionSource == null || m_actionSource.Action == null || m_state != EntityState.Alive)
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

	protected override void EntityKilled()
	{
		base.EntityKilled();
		m_perceptionGameObject.SetActive(false);
	}
}
