﻿/////////////////////////////////////////////////////////////////////
// 
// LevelGrid.cs
//
// What it does: Hold the grid-based information for the level for collision and pathing.
//
// Notes: Raycasting shamelessly nicked from xboxforums.create.msdn.com/forums/p/42727/252947.aspx
//		  Partial class with LevelGrid_debug
//		  Partial class with LevelGrid_graphs
// 
// To-do: 	Overlap functions
//			Path smoothing
//
//////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class LevelGridRaycastHit
{
	public int x;
	public int y;
	public GridCell cell;
}

public class Point
{
	public int x;
	public int y;
}

public partial class LevelGrid
{
	private GridCell[,] m_cells = null;
	private int m_numCellsX 	= 1;
	private int m_numCellsY 	= 1;
	private float m_cellSize 	= 1.0f;
	private float m_gridSizeX	= 1.0f;
	private float m_gridSizeY	= 1.0f;
	private Vector2 m_gridStart = Vector2.zero;
	private Vector2 m_gridEnd 	= Vector2.zero;

	public int NumCellsX { get { return m_numCellsX; } }
	public int NumCellsY { get { return m_numCellsY; } }

	public Vector2 GridStart { get { return m_gridStart; } }

	public LevelGrid(float cellSize, int numCellsX, int numCellsY)
	{
		m_cellSize = cellSize;

		// Work out how many cells that is and bung one on to avoid fractions of cells.
		m_numCellsX 	= numCellsX;
		m_numCellsY 	= numCellsY;

		m_gridSizeX 	= m_numCellsX * m_cellSize;
		m_gridSizeY 	= m_numCellsY * m_cellSize;

		m_gridStart 	= new Vector2(-m_gridSizeX / 2.0f, -m_gridSizeY / 2.0f);
		m_gridEnd 		= new Vector2(m_gridSizeX / 2.0f, m_gridSizeY / 2.0f);

		m_cells = new GridCell[m_numCellsX, m_numCellsY];

		for(int y = 0; y < m_numCellsY; y++)
		{
			for(int x = 0; x < m_numCellsX; x++)
			{
				m_cells[x, y] = new GridCell();
				m_cells[x, y].x = x;
				m_cells[x, y].y = y;
			}
		}
	}

	public void RebuildMeshes()
	{
		RebuildDebugMesh();
		RebuildContentsMesh();
	}

	// Bounds checking on input rays should be done outside of this method to avoid shitting it up further
	public bool Raycast(Vector2 rayStart, Vector2 rayEnd, int contentsMask, ref LevelGridRaycastHit outHit)
	{
		rayStart -= m_gridStart;
		rayEnd -= m_gridStart;

		float x1 = rayStart.x;  
		float y1 = rayStart.y;  
		float x2 = rayEnd.x;  
		float y2 = rayEnd.y;  
		
		int lowX = Mathf.Max((int)(x1 / m_cellSize), 0);  
		int lowY = Mathf.Max((int)(y1 / m_cellSize), 0);  
		int highX = Mathf.Min( (int)(x2 / m_cellSize), m_numCellsX - 1);  
		int highY = Mathf.Min( (int)(y2 / m_cellSize), m_numCellsY - 1);  

		highX = Mathf.Max(highX, 0);
		highY = Mathf.Max(highY, 0);

		
		int dx = ((x1 < x2) ? 1 : ((x1 > x2) ? -1 : 0));  
		int dy = ((y1 < y2) ? 1 : ((y1 > y2) ? -1 : 0));  
		
		float minx = m_cellSize * ((int)(x1 / m_cellSize));  
		float maxx = minx + m_cellSize;  
		float tx = ((x1 > x2) ? (x1 - minx) : maxx - x1) / Mathf.Abs(x2 - x1);  
		float miny = m_cellSize * ((int)(y1 / m_cellSize));  
		float maxy = miny + m_cellSize;  
		float ty = ((y1 > y2) ? (y1 - miny) : maxy - y1) / Mathf.Abs(y2 - y1);  
		
		float deltatx = m_cellSize / Mathf.Abs(x2 - x1);  
		float deltaty = m_cellSize / Mathf.Abs(y2 - y1);  

		const int maxIterations = 200;
		int currentIterations = 0;

		while (currentIterations < maxIterations)  
		{  
			currentIterations++;
			
			if (tx <= ty )  
			{  
				if (lowX == highX)  
				{  
					return false; 
				}  
				tx 		+= deltatx;  
				lowX 	+= dx;  

				if((m_cells[lowX, lowY].m_contentsMask & contentsMask) != 0)
				{
					outHit.x = lowX;
					outHit.y = lowY;
					outHit.cell = m_cells[lowX, lowY];
					return true;
				}
			}  
			else if (ty <= tx)  
			{  
				if (lowY == highY)  
				{  
					return false;
				}  
				ty += deltaty;  
				lowY += dy;  

				if((m_cells[lowX, lowY].m_contentsMask & contentsMask) != 0)
				{
					outHit.x = lowX;
					outHit.y = lowY;
					outHit.cell = m_cells[lowX, lowY];
					return true;
				}
			}  
		}

		return false;
	}

	public bool GetCellLocation(int x, int y, out Vector2 outLocation)
	{
		outLocation = new Vector2(0.0f, 0.0f);

		if(x < 0 || y < 0 || x >= m_numCellsX || y >= m_numCellsY) { return false; }

		outLocation.x = m_gridStart.x + (x * m_cellSize);
		outLocation.y = m_gridStart.y + (y * m_cellSize);

		return true;
	}

	public GridCell GetCell(int x, int y)
	{
		return m_cells[x, y];
	}

	public Point GetCellIndices(Vector2 worldLocation)
	{
		Point newPoint = new Point();

		Vector2 relativeLocation = worldLocation - m_gridStart;

		int x = (int)(relativeLocation.x / m_cellSize);
		int y = (int)(relativeLocation.y / m_cellSize);

		if(x < m_numCellsX && x >= 0 && y < m_numCellsY && y >= 0)
		{
			newPoint.x = x;
			newPoint.y = y;

			return newPoint;
		}

		return null;
	}
}


