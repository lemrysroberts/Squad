using UnityEngine;
using System.Collections;

public enum GridCellContentsType
{
	Empty 	= 0,
	Wall	= 1,
	Cover	= 2
}

public interface GridCellContents
{
	void SetOwnerCell(GridCell cell);
	void ApplyDamage(float amount, DamageType type, Vector2 damageDirection);
	GridCellContentsType GetContentsType();
}
