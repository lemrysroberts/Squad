/////////////////////////////////////////////////////////////////////////
///	Weapon.cs
/// 
/// Lots of wonky maths in here to once again prove the shoddy state of my education.
/////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public abstract class Weapon
{
	public float Range { get { return m_range; } }

	protected Entity m_owner 					= null;
	protected float m_range						= 0.0f;
	protected Vector3 m_shotRegionStartDirection;
	protected Vector3 m_shotRegionEndDirection;
	protected float m_shotRegionDotProduct	= 0.0f;

	protected Weapon(Entity owner)
	{
		m_owner = owner;
	}

	public void SetShotRegion(float startAngle, float endAngle)
	{
		m_shotRegionStartDirection 	= (Quaternion.Euler(0.0f, 0.0f, startAngle) * Vector3.right).normalized;
		m_shotRegionEndDirection 	= (Quaternion.Euler(0.0f, 0.0f, endAngle) * Vector3.right).normalized;

		// Switch which vector comes first to ensure that ShotInRegion checks are consistent.
		// The user can draw arc edges in the order they choose, so this has to be performed.
		Vector3 cross = Vector3.Cross(m_shotRegionStartDirection, m_shotRegionEndDirection);
		if(Vector3.Dot(Vector3.forward, cross) > 0.0f)
		{
			Vector3 temp 				= m_shotRegionStartDirection;
			m_shotRegionStartDirection 	= m_shotRegionEndDirection;
			m_shotRegionEndDirection 	= temp;
		}
	}

	public abstract void StartFiring();
	public abstract void UpdateFiring();
	public abstract void StopFiring();

	// Check the signs of relative angles to determine whether a direction is bounded by aim.
	protected bool ShotInRegion(Vector3 shotDirection)
	{
		shotDirection = shotDirection.normalized;

		Vector3 startCross = Vector3.Cross(shotDirection, m_shotRegionStartDirection);
		if(Vector3.Dot(Vector3.forward, startCross) < 0.0f)
		{
			return false; // target is before the aim-region's start.
		}

		Vector3 endCross = Vector3.Cross(shotDirection, m_shotRegionEndDirection);
		if(Vector3.Dot(Vector3.forward, endCross) > 0.0f)
		{
			return false; // target is after the aim-region's end.
		}

		// All good. Well done, everybody.
		return true;
	}
}
