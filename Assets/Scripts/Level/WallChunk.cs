using UnityEngine;
using System.Collections.Generic;

public class WallChunk : GridCellContents
{
	private float m_health 		= 5.0f;
	private bool m_destroyed 	= false;
	private Walls m_parentWalls	= null;
	private GridCell m_owner	= null;

	public WallChunk(Walls parentWalls)
	{
		m_parentWalls = parentWalls;
	}

	public void SetOwnerCell(GridCell cell)
	{
		m_owner = cell;
	}

	public void ApplyDamage(float amount, DamageType type, Vector2 damageDirection) 
	{
		m_health -= amount;
		
		if(m_health <= 0.0f && !m_destroyed)
		{
			// Tell the walls system that this chunk is destroyed so it can prompt a mesh rebuild
			m_parentWalls.ChunkDestroyed(this);

			// Update the chunk's collision type, but that's all really...
			m_owner.RemoveType(GetContentsType());
			m_destroyed = true;
			m_owner.AddType(GetContentsType());
		}
	}
	
	public GridCellContentsType GetContentsType()
	{
		if(m_destroyed)
		{
			return GridCellContentsType.Empty;
		}

		return GridCellContentsType.Wall;
	}
}
