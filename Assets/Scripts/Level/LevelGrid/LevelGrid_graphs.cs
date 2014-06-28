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
// To-do: 	Local updates to blocked cells: 
//			When a cell is blocked, check all cells that could be dependent upon the blocked cell (maxAnnotationSize ^ 2 cells).
//			If any cell has an annotation equal to that of the distance to the blocked cell, decrement it.
//
//////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public partial class LevelGrid
{
	const int MaxClearance = 5;

	private class ClearanceBlockers
	{
		public int[] blockerCount;

		public ClearanceBlockers()
		{
			blockerCount = new int[MaxClearance];
		}
	}

	private AIGraph m_defaultGraph = null;
	private AIGraphNode[][] m_defaultNodes = null;
	private ClearanceBlockers[][] m_blockers = null;

	public void RebuildGraphs()
	{
		m_defaultGraph = new AIGraph();
		m_defaultNodes = new AIGraphNode[m_numCellsX][];
		m_blockers	   = new ClearanceBlockers[m_numCellsX][];

		for(int x = 0; x < m_numCellsX; x++)
		{
			m_defaultNodes[x] = new AIGraphNode[m_numCellsY];
			m_blockers[x]  = new ClearanceBlockers[m_numCellsY];

			for(int y = 0; y < m_numCellsY; y++)
			{
				m_defaultNodes[x][y] = m_defaultGraph.AddNode(m_gridStart + (new Vector2(x, y) * m_cellSize));

				m_blockers[x][y] = new ClearanceBlockers();

				// Calculate the accessibility of each cell by checking increasingly large areas in +x, +y,
				// up to maxAnnotationSize, maxAnnotationSize

				// Annotation of 0 if the cell itself is blocked


				{
					// A single open cell has access of 1, so start there
					int annotation = 1;

					// Iterate the rows down and left, correcting their maximum clearance if they included the blocked cell.
					for(int i = 1; i < MaxClearance; i++)
					{
						for(int xIndex = 0; xIndex < i; xIndex++)
						{
							if( (x + xIndex) >= m_numCellsX ||
							    (y + i) >= m_numCellsY ||
								(m_cells[x + xIndex, y + i].m_contentsMask & (1 << (int)GridCellContentsType.Wall)) != 0)
							{
								m_blockers[x][y].blockerCount[i]++;
							}
						}
						
						for(int yIndex = 0; yIndex < (i + 1); yIndex++)
						{
							if( (x + i) >= m_numCellsX ||
							    (y + yIndex) >= m_numCellsY ||
								(m_cells[x + i, y + yIndex].m_contentsMask & (1 << (int)GridCellContentsType.Wall)) != 0)
							{
								m_blockers[x][y].blockerCount[i]++;
							}
						}
					}

					bool blockerFound = false;

					for(int i = 0; i < MaxClearance && !blockerFound; i++)
					{
						if(m_blockers[x][y].blockerCount[i] > 0)
						{

							blockerFound = true;
							continue;
						}
						else
						{
							m_defaultNodes[x][y].Annotation = i + 1;
						}

					}

					if((m_cells[x, y].m_contentsMask & (1 << (int)GridCellContentsType.Wall)) != 0)
					{
						m_defaultNodes[x][y].Annotation = 0;
						m_blockers[x][y].blockerCount[0]++;
					} 
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

	private void CellBlocked(int x, int y)
	{
		const int maxAnnotationSize = 6;

		m_defaultNodes[x][y].Annotation = 0;

		// Iterate the rows down and left, correcting their maximum clearance if they included the blocked cell.
		for(int i = 1; i < maxAnnotationSize; i++)
		{
			for(int xIndex = 0; xIndex < i + 1; xIndex++)
			{
				m_blockers[x - xIndex][y - i].blockerCount[i]++;
				if(m_defaultNodes[x - xIndex][y - i].Annotation > (i + 1))
				{
					m_defaultNodes[x - xIndex][y - i].Annotation = (i + 1);
				}
			}

			for(int yIndex = 0; yIndex < i; yIndex++)
			{
				m_blockers[x - i][y - yIndex].blockerCount[i]++;
				if(m_defaultNodes[x - i][y - yIndex].Annotation > (i + 1))
				{
					m_defaultNodes[x - i][y - yIndex].Annotation = (i + 1);
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

				blockerFound = false;
				
				for(int j = 0; j < MaxClearance && !blockerFound; j++)
				{
					if(m_blockers[x - xIndex][y - i].blockerCount[j] > 0)
					{ 
						blockerFound = true; 
						continue;
					}
					else
					{
						m_defaultNodes[x - xIndex][y - i].Annotation = j + 1;
					}
					
				}

			}
			
			for(int yIndex = 0; yIndex < i; yIndex++)
			{
				m_blockers[x - i][y - yIndex].blockerCount[i]--;

				blockerFound = false;
				
				for(int j = 0; j < MaxClearance && !blockerFound; j++)
				{
					if(m_blockers[x - i][y - yIndex].blockerCount[j] > 0)
					{ 
						blockerFound = true; 
						continue;
					}
					else
					{
						m_defaultNodes[x - i][y - yIndex].Annotation = j + 1;
					}
					
				}
			}
		}

		blockerFound = false;
		
		for(int j = 0; j < MaxClearance && !blockerFound; j++)
		{
			if(m_blockers[x][y].blockerCount[j] > 0)
			{ 
				blockerFound = true; 
				continue;
			}
			else
			{
				m_defaultNodes[x][y].Annotation = j + 1;
			}
			
		}

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
