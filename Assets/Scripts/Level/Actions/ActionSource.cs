using UnityEngine;
using System.Collections.Generic;

public enum ActionSourceInput
{
	None,

	InputPrimaryAction,
	InputSecondaryAction
}

public class ActionSource : InputListener
{
	public EntityAction m_parentAction = null;
	private EntityAction m_action = null;
	private Entity m_entity = null;
	private bool m_editingAction = false;

	public EntityAction Action { get { return m_action; } }
	
	public void SetEntity(Entity entity)
	{
		m_entity = entity;
	}

	public Entity GetEntity() { return m_entity; }

	public override bool MouseButtonDown(int mouseButton)
	{
		if(m_editingAction && m_action != null && mouseButton != m_action.PrimaryMouseButton)
		{
			if(m_action.Commit())
			{
				m_action.EndPlanning();
				m_editingAction = false;

			}
			return true;
		}
		else
		{
			// TODO: Clear any existing action!
			//ActionSourceInput inputType = ActionSourceInput.None;

			// TODO: This should store the old plan until the new one is committed
			if(m_action != null)
			{
				Debug.Log("Doing delete...");
				m_action.DeletePlanning();
			}

			StartAction(mouseButton);

		}
		
		return true;
	}

	public EntityAction StartAction(int actionID)
	{
		switch(actionID)
		{
			//case 0: inputType = ActionSourceInput.InputPrimaryAction;
			//case 1: inputType = ActionSourceInput.InputSecondaryAction;
		case 0 : m_action = new ActionMove(transform.position); break;
		case 1 : m_action = new ActionAim(); break;
			
		}

		m_action.SetOwner(this);
		m_action.SetEntity(m_entity);
		m_action.StartPlanning();

		m_editingAction = true;

		return m_action;
	}

	public void EndAction()
	{

	}
	
	public override bool MouseButtonUp(int mouseButton)
	{
		if(m_action != null)
		{
			if(mouseButton == m_action.PrimaryMouseButton)
			{
				m_action.EndPlanning();
				m_action = null;
				return false;
			}
		}
		
		return m_action != null;
	}
	
	public override void MouseOver()
	{
		
	}
	
	public override void MouseMove(Vector2 worldPosition)
	{
		if(m_action != null)
		{
			m_action.UpdateHoverLocation(worldPosition);
		}
	}
}
