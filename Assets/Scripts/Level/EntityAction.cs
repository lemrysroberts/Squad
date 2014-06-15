using UnityEngine;
using System.Collections.Generic;

public abstract class EntityAction
{
	protected ActionSource m_owner 	= null;
	protected Entity m_entity		= null;

	public void SetOwner(ActionSource owner) { m_owner = owner; }
	public void SetEntity(Entity entity) { m_entity = entity; }

	protected Entity GetEntity() { return m_owner != null ? m_owner.GetEntity() : null; }

	public abstract void Start();
	public abstract void Update();
	public abstract void End();
 	public abstract bool IsComplete();

	public abstract void StartPlanning();
	public abstract void EndPlanning();
	public abstract void CancelPlanning();
	public abstract void DeletePlanning();
	public abstract void UpdateHoverLocation(Vector2 location);
	public abstract bool Commit(); // Return true if the action planning is finished

	public int PrimaryMouseButton { get { return m_primaryMouseButton; } }
	public ActionSource ChildSource { get { return m_childSource; } }

	protected int m_primaryMouseButton 	= -1;
	protected ActionSource m_childSource = null;
}
