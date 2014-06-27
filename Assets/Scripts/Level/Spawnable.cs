using UnityEngine;
using System.Collections;

public abstract class Spawnable : MonoBehaviour 
{
	protected Spawner m_parentSpawner = null;

	public Spawner ParentSpawner { set; get; }

	public abstract void OnSpawn();
	public abstract void OnDespawn();
}
