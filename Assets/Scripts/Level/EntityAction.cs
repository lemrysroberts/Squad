using UnityEngine;
using System.Collections;

public abstract class EntityAction
{
	protected Entity m_owner = null;

	public void SetOwner(Entity owner)
	{
		m_owner = owner;
	}

	public abstract void Start();
	public abstract void Update();
	public abstract void End();

	public abstract bool IsComplete();
}
