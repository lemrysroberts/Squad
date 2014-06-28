using UnityEngine;
using System.Collections.Generic;

struct CandidateAnglePair
{
	Entity m_entity;
	float m_angle;
}

public class TestWeapon : Weapon 
{
	public bool AlwaysFiring					= false;

	public float ShotDamage						= 2.0f;
	public int ShotsPerMinute 					= 600;

	private float m_timeToNextShot 				= 0.0f;
	private float m_timeBetweenShots 			= 0.0f;

	List<Entity> m_shotCandidates 	= new List<Entity>();

	public TestWeapon(Entity owner)
		: base(owner)
	{
		m_range = 5.0f;
		m_timeBetweenShots = 60.0f / (float)ShotsPerMinute;
	}

	public override void StartFiring() { }

	public override void UpdateFiring()
	{
		m_timeToNextShot -= Time.deltaTime;

		if(m_timeToNextShot <= 0.0f)
		{
			// TODO: loop this to allow for shots to be fired faster than the frame-rate.

			TakeShot();
			m_timeToNextShot = m_timeBetweenShots;
		}
	}

	public override void StopFiring()
	{

	}

	private void TakeShot()
	{
		m_shotCandidates.Clear();

		foreach(var entity in m_owner.EntityPerception.Enemies)
		{
			Vector2 direction = (Vector2)entity.transform.position - (Vector2)m_owner.transform.position;

			if(ShotInRegion(direction))
			{
				m_shotCandidates.Add(entity);
			}
		}

		if(m_shotCandidates.Count == 0 && !AlwaysFiring) { return; }

		Vector2 shotDirection = Vector2.zero;
		int target = -1;

		if(!AlwaysFiring)
		{
			target = Random.Range(0, m_shotCandidates.Count);

			shotDirection = (m_shotCandidates[target].transform.position - m_owner.transform.position);
		}

		if(Random.Range(0, 5) != 0 || AlwaysFiring)
		{
			shotDirection = Vector3.Lerp(m_shotRegionStartDirection, m_shotRegionEndDirection, Random.value) * 10.0f;
			LevelGrid grid = Level.Instance.GetGrid();

			int hitMask = 1 << (int)GridCellContentsType.Wall;

			LevelGridRaycastHit hit = new LevelGridRaycastHit();

			if(grid.Raycast(m_owner.transform.position, m_owner.transform.position + (Vector3)shotDirection, hitMask, ref hit))
			{
				Vector2 hitPosition;
				grid.GetCellLocation(hit.x, hit.y, out hitPosition);
				hit.cell.ApplyDamage(ShotDamage, DamageType.Bullet, shotDirection);

				Debug.DrawLine((Vector3)hitPosition + new Vector3(0.0f, 0.0f, -1.2f) , (Vector3)hitPosition + new Vector3(0.2f, 0.2f, -1.2f), Color.green, 2.0f);
			}

		}
		else
		{
			m_shotCandidates[target].EntityHealth.ApplyDamage(ShotDamage, DamageType.Bullet);
		}

		Debug.DrawLine(m_owner.transform.position, m_owner.transform.position + (Vector3)shotDirection, Color.white, 0.2f);


		ScreenShake.Instance.AddShake(0.2f, m_owner.transform.position);
	}
}
