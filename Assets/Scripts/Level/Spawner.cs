using UnityEngine;
using System.Collections;

public class Spawner : LevelComponent 
{
	public Entity ObjectToSpawn = null;
	public float SpawnDelay 		= 1.0f;

	private float m_timeToSpawn		= 0.0f;
	private bool m_started			= false;

	// Pool options

	public override void LevelStarted()
	{
		m_timeToSpawn = SpawnDelay;

		m_started = true;
	}

	// Update is called once per frame
	void Update () 
	{
		if(!m_started) return;

		m_timeToSpawn -= Time.deltaTime;

		if(m_timeToSpawn <= 0.0f)
		{
			GameObject.Instantiate(ObjectToSpawn);
			m_timeToSpawn = SpawnDelay;
		}
	}
}
