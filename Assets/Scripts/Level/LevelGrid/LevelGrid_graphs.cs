/////////////////////////////////////////////////////////////////////
// 
// LevelGrid_graphs.cs
//
// What it does: Handles the updating of the level's node-graph that's used for pathing.
//
// Notes: Raycasting shamelessly nicked from xboxforums.create.msdn.com/forums/p/42727/252947.aspx
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
	private AIGraph m_defaultGraph = null;
	private AIGraphNode[][] m_defaultNodes = null;

	public void RebuildGraphs()
	{
		m_defaultGraph = new AIGraph();
		m_defaultNodes = new AIGraphNode[m_numCellsX][];

		const int maxAnnotationSize = 5;

		for(int x = 0; x < m_numCellsX; x++)
		{
			m_defaultNodes[x] = new AIGraphNode[m_numCellsY];

			for(int y = 0; y < m_numCellsY; y++)
			{
				m_defaultNodes[x][y] = m_defaultGraph.AddNode(m_gridStart + (new Vector2(x, y) * m_cellSize));

				// Calculate the accessibility of each cell by checking increasingly large areas in +x, +y,
				// up to maxAnnotationSize, maxAnnotationSize

				// Annotation of 0 if the cell itself is blocked
				if((m_cells[x, y].m_contentsMask & (1 << (int)GridCellContentsType.Wall)) != 0)
				{
					m_defaultNodes[x][y].Annotation = 0;
				} 
				else
				{
					// A single open cell has access of 1, so start there
					int annotation = 1;
					bool obstructionFound = false;
					for(int annotationLevel = 1; annotationLevel < maxAnnotationSize && !obstructionFound; annotationLevel++)
					{
						for(int localX = 0; localX < annotationLevel; localX++)
						{
							for(int localY = 0; localY < annotationLevel; localY++)
							{
								// Bounds
								if((x + localX) >= m_numCellsX || (y + localY) >= m_numCellsY)
								{
									obstructionFound = true;
									continue;
								}

								if((m_cells[x + localX, y + localY].m_contentsMask & (1 << (int)GridCellContentsType.Wall)) != 0)
								{
									obstructionFound = true;
									continue;
								}
							}
						}
						annotation = annotationLevel;
					}
					m_defaultNodes[x][y].Annotation = annotation - 1;
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
