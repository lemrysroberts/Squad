//////////////////////////////////////////////////////////////////////////////////
/// LevelGrid.cs
/// 
/// Notes: Raycasting shamelessly nicked from xboxforums.create.msdn.com/forums/p/42727/252947.aspx
/// 
//////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class LevelGridRaycastHit
{
	public int x;
	public int y;
	public LevelGrid.GridCell cell;
}

public partial class LevelGrid
{
	public enum GridCellContents
	{
		Empty 	= 0,
		Blocked = 1
	}

	public class GridCell
	{
		public int m_contentsMask = 0;

		public GridCell()
		{
			if(Random.value > 0.9f)
			{
				m_contentsMask = (int)GridCellContents.Blocked;
			}
		}
	}

	private GridCell[,] m_cells = null;
	private int m_numCellsX 	= 1;
	private int m_numCellsY 	= 1;
	private float m_cellSize 	= 1.0f;
	private float m_gridSizeX	= 1.0f;
	private float m_gridSizeY	= 1.0f;
	private Vector2 m_gridStart = Vector2.zero;
	private Vector2 m_gridEnd 	= Vector2.zero;

	public LevelGrid(float cellSize, Vector2 start, Vector2 end)
	{
		m_cellSize = cellSize;

		// Determine the size of the requested grid.
		float width 	= end.x - start.x;
		float height 	= end.y - start.y;

		// Work out how many cells that is and bung one on to avoid fractions of cells.
		m_numCellsX 	= (int)(width / cellSize) + 1;
		m_numCellsY 	= (int)(height / cellSize) + 1;

		m_gridSizeX 	= m_numCellsX * m_cellSize;
		m_gridSizeY 	= m_numCellsY * m_cellSize;

		m_gridStart 	= start;
		m_gridEnd 		= m_gridStart + new Vector2(m_gridSizeX, m_gridSizeY);

		m_cells = new GridCell[m_numCellsX, m_numCellsY];

		for(int y = 0; y < m_numCellsY; y++)
		{
			for(int x = 0; x < m_numCellsX; x++)
			{
				m_cells[x, y] = new GridCell();
			}
		}


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
}

