using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour 
{
	public Entity ObjectToSpawn = null;
	public float SpawnDelay 		= 1.0f;

	private float m_timeToSpawn		= 0.0f;

	// Pool options

	// Use this for initialization
	void Start () 
	{
		m_timeToSpawn = SpawnDelay;
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_timeToSpawn -= Time.deltaTime;

		if(m_timeToSpawn <= 0.0f)
		{
			GameObject.Instantiate(ObjectToSpawn);
			m_timeToSpawn = SpawnDelay;
		}
	}
}
