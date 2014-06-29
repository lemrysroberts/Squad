/////////////////////////////////////////////////////////////////////
// 
// LevelGrid_graphs.cs
//
// What it does: Handles the updating of the level's node-graph that's used for pathing.
//
// Notes: Clearance path-finding nabbed from http://aigamedev.com/open/tutorials/clearance-based-pathfinding/
//		  Partial class with LevelGrid_debug
//		  Partial class with LevelGrid
// 
// To-do: 	Optimising the horror, probably.
//			There are a lot of mysterious "+1"s in this file that are going to be very confusing.
//
//////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public partial class LevelGrid
{
	// The largest clearance value that the grid will ever bother handling
	const int MaxClearance = 5;

	// Tracks how many blockers are present for each clearance level of each cell.
	private class ClearanceBlockers
	{
		public int[] blockerCount;
		public ClearanceBlockers() { blockerCount = new int[MaxClearance]; }
	}

	private AIGraph m_defaultGraph 				= null;
	private AIGraphNode[][] m_defaultNodes 		= null;
	private ClearanceBlockers[][] m_blockers 	= null;

	// Create all the cells of the grid, populate initial blocker counts and link all nodes.
	public void RebuildGraphs()
	{
		m_defaultGraph = new AIGraph();
		m_defaultNodes = new AIGraphNode[m_numCellsX][];
		m_blockers	   = new ClearanceBlockers[m_numCellsX][];

		// Create the cells
		for(int x = 0; x < m_numCellsX; x++)
		{
			m_defaultNodes[x] 	= new AIGraphNode[m_numCellsY];
			m_blockers[x]  		= new ClearanceBlockers[m_numCellsY];

			for(int y = 0; y < m_numCellsY; y++)
			{
				m_defaultNodes[x][y] 	= m_defaultGraph.AddNode(m_gridStart + (new Vector2(x, y) * m_cellSize));
				m_blockers[x][y] 		= new ClearanceBlockers();

				// Iterate the rows down and left, incrementing blocker counts when a blocked cell is encountered.
				for(int clearance = 1; clearance < MaxClearance; clearance++)
				{
					for(int xIndex = 0; xIndex < clearance; xIndex++)
					{
						// Check bounds, then blockers
						if( (x + xIndex) 	>= m_numCellsX 	||
						    (y + clearance) >= m_numCellsY 	||
						   	(m_cells[x + xIndex, y + clearance].m_contentsMask & (1 << (int)GridCellContentsType.Wall)) != 0)
						{
							m_blockers[x][y].blockerCount[clearance]++;
						}
					}
					
					for(int yIndex = 0; yIndex < (clearance + 1); yIndex++)
					{
						// Check bounds, then blockers
						if( (x + clearance) >= m_numCellsX ||
						    (y + yIndex) 	>= m_numCellsY ||
						   (m_cells[x + clearance, y + yIndex].m_contentsMask & (1 << (int)GridCellContentsType.Wall)) != 0)
						{
							m_blockers[x][y].blockerCount[clearance]++;
						}
					}
				}

				// Re-calculate the annotation for the current cell now that blocker-counts are set.
				UpdateCellAnnotation(x, y);

				// If the cell itself is blocked, handle that
				if((m_cells[x, y].m_contentsMask & (1 << (int)GridCellContentsType.Wall)) != 0)
				{
					m_defaultNodes[x][y].Annotation = 0;
					m_blockers[x][y].blockerCount[0]++;
				} 
			}
		}

		// Link all the nodes
		for(int x = 0; x < m_numCellsX; x++)
		{
			for(int y = 0; y < m_numCellsY; y++)
			{
				if(x > 0) 				m_defaultNodes[x][y].NodeLinks.Add(m_defaultNodes[x-1][y]);
				if(x < m_numCellsX - 1) m_defaultNodes[x][y].NodeLinks.Add(m_defaultNodes[x+1][y]);
				if(y > 0) 				m_defaultNodes[x][y].NodeLinks.Add(m_defaultNodes[x][y - 1]);
				if(y < m_numCellsY - 1) m_defaultNodes[x][y].NodeLinks.Add(m_defaultNodes[x][y + 1]);

				if(x > 0 && y > 0)								m_defaultNodes[x][y].NodeLinks.Add(m_defaultNodes[x-1][y - 1]);
				if(x < m_numCellsX - 1 && y > 0) 				m_defaultNodes[x][y].NodeLinks.Add(m_defaultNodes[x+1][y - 1]);
				if(x > 0 && y < m_numCellsY - 1) 					m_defaultNodes[x][y].NodeLinks.Add(m_defaultNodes[x-1][y+1]);
				if(y < m_numCellsY - 1 && x < m_numCellsX - 1) 	m_defaultNodes[x][y].NodeLinks.Add(m_defaultNodes[x+1][y + 1]);
			}
		}
	}

	public void UpdateCellBlocked(int x, int y, bool blocked)
	{
		if(blocked)
		{
			CellBlocked(x, y);
		}
		else
		{
			CellUnblocked(x, y);
		}
	}

	// This determines a cell's maximum clearance based on the presence of blockers at each clearance tier
	private void UpdateCellAnnotation(int x, int y)
	{
		bool blockerFound = false;

		for(int clearance = 0; clearance < MaxClearance && !blockerFound; clearance++)
		{
			if(m_blockers[x][y].blockerCount[clearance] > 0)
			{
				blockerFound = true;
				continue;
			}
			else
			{
				m_defaultNodes[x][y].Annotation = clearance + 1;
			}
		}
	}

	private void CellBlocked(int x, int y)
	{
		m_defaultNodes[x][y].Annotation = 0;
		m_blockers[x][y].blockerCount[0]++;

		// Iterate the rows down and left, correcting their maximum clearance if they included the blocked cell.
		for(int i = 1; i < MaxClearance; i++)
		{
			// Check along the clearance row
			if((y - i) > -1) // Bounds check y
			{
				// i +1 to include the shared corner
				for(int xIndex = 0; xIndex < i + 1 && (x - xIndex) > -1;xIndex++)			
				{
					m_blockers[x - xIndex][y - i].blockerCount[i]++;
					if(m_defaultNodes[x - xIndex][y - i].Annotation > i)
					{
						m_defaultNodes[x - xIndex][y - i].Annotation = (i);
					}
				}
			}

			// Check down the clearance column
			if((x - i) > -1) // Bounds check x
			{
				for(int yIndex = 0; yIndex < i && (y - yIndex) > -1; yIndex++)
				{
					m_blockers[x - i][y - yIndex].blockerCount[i]++;
					if(m_defaultNodes[x - i][y - yIndex].Annotation > (i ))
					{
						m_defaultNodes[x - i][y - yIndex].Annotation = (i );
					}
				}
			}
		}
	}

	private void CellUnblocked(int x, int y)
	{
		m_blockers[x][y].blockerCount[0] = 0;

		bool blockerFound = false;

		// Iterate the rows down and left, correcting their maximum clearance if they included the blocked cell.
		for(int i = 1; i < MaxClearance; i++)
		{
			// i - 1 to prevent overlapping corners and double decrements
			for(int xIndex = 0; xIndex < i + 1; xIndex++)
			{
				m_blockers[x - xIndex][y - i].blockerCount[i]--;
				UpdateCellAnnotation(x - xIndex, y - i);
			}
			
			for(int yIndex = 0; yIndex < i; yIndex++)
			{
				m_blockers[x - i][y - yIndex].blockerCount[i]--;
				UpdateCellAnnotation(x - i, y - yIndex);
			}
		}

		UpdateCellAnnotation(x, y);
	}

	public Route GetRoute(Vector2 origin, Vector2 target, int clearance)
	{
		// The supplied points are the centre of an object.
		// The annotations assume an object growing up and right. As such, offset by half the clearance
		// to find the actual tile to search from.
		float offset 		= ((float)clearance * m_cellSize) / 2.0f;
		Vector2 offsetVec 	= new Vector2(offset, offset);

		origin -= offsetVec;
		target -= offsetVec;

		Point startCell = GetCellIndices(origin); 
		Point endCell 	= GetCellIndices(target);

		Vector2 finalCellPos;

		GetCellLocation(endCell.x, endCell.y, out finalCellPos);

		// Out of bounds
		if(startCell == null || endCell == null) { return null; }

		// TODO: Not this. Obviously.
		RouteFinder routeFinder = new RouteFinder(m_numCellsX * m_numCellsY);

		// Use (clearance + 1) to allow in-cell movement. It effectively adds a cell of buffer.
		Route result = routeFinder.FindRoute(m_defaultGraph, m_defaultNodes[startCell.x][startCell.y], m_defaultNodes[endCell.x][endCell.y], clearance + 1);

		// Set the offset used in the search, so the calling function can reconstruct the centres used.
		result.OffsetVector = offsetVec;

		result.FinalOffset = target - finalCellPos;

		return result;
	}

	public void RenderDebugRoute(Route route)
	{
		for(int i = 0; i < route.m_routePoints.Count - 1; i++)
		{
			if(i == (route.m_routePoints.Count - 2))
			{
				Debug.DrawLine(	(Vector3)(route.m_routePoints[i].NodePosition + route.OffsetVector) + new Vector3(0.0f, 0.0f, -1.0f), 
				               (Vector3)(route.m_routePoints[i + 1].NodePosition + route.OffsetVector + route.FinalOffset ) + new Vector3(0.0f, 0.0f, -1.0f), 
				               Color.magenta );
			}
			else
			{
				Debug.DrawLine(	(Vector3)(route.m_routePoints[i].NodePosition + route.OffsetVector) + new Vector3(0.0f, 0.0f, -1.0f), 
				               (Vector3)(route.m_routePoints[i + 1].NodePosition + route.OffsetVector ) + new Vector3(0.0f, 0.0f, -1.0f), 
				               Color.magenta );
			}
		}
	}
}
