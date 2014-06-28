using UnityEngine;
using System.Collections.Generic;

public class GridCell
{
	public int m_contentsMask = 0;

	public int x = -1;
	public int y = -1;

	private List<GridCellContents> m_contents = new List<GridCellContents>();
	private int[] m_contentTypeCounts;
	
	public GridCell()
	{
		m_contentTypeCounts = new int[System.Enum.GetNames(typeof(GridCellContentsType)).Length];
	}

	public void AddContents(GridCellContents contents)
	{
		m_contents.Add(contents);

		contents.SetOwnerCell(this);

		AddType(contents.GetContentsType());
	}

	public void RemoveContents(GridCellContents contents)
	{
		m_contents.Remove(contents);
		RemoveType(contents.GetContentsType());
	}

	public void ApplyDamage(float amount, DamageType type, Vector2 sourceDirection)
	{
		foreach(var contents in m_contents)
		{
			contents.ApplyDamage(amount, type, sourceDirection);
		}
	}

	public void RemoveType(GridCellContentsType type)
	{
		m_contentTypeCounts[(int)type]--;
		
		if(m_contentTypeCounts[(int)type] < 0)
		{
			Debug.LogError("Invalid contents count!");
		}
		else if(m_contentTypeCounts[(int)type] == 0)
		{
			m_contentsMask = m_contentsMask & (~(1 << (int)type));
		}
	}

	public void AddType(GridCellContentsType type)
	{
		m_contentsMask = m_contentsMask | (1 << (int)type);
		m_contentTypeCounts[(int)type]++;
	}
}