using UnityEngine;
using System.Collections;

public class LevelGrid
{
	struct GridCell
	{

	}

	private Mesh m_debugMesh 	= null;
	private GridCell[,] m_cells = null;
	private int m_numCellsX 	= 1;
	private int m_numCellsY 	= 1;
	private float m_cellSize 	= 1.0f;
	private float m_gridSizeX	= 1.0f;
	private float m_gridSizeY	= 1.0f;
	private Vector2 m_gridStart = Vector2.zero;
	private Vector2 m_gridEnd 	= Vector2.zero;

	public Mesh DebugMesh { get { return m_debugMesh; } }

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

		RebuildDebugMesh();
	}

	private void RebuildDebugMesh()
	{
		m_debugMesh = new Mesh();

		int vertexCount 	= ((m_numCellsX + 1) + (m_numCellsY + 1)) * 2;
		int indexCount 		= vertexCount;

		Vector3[] vertices 	= new Vector3[vertexCount];
		int[] indices 		= new int[indexCount];

		int vertIndex 		= 0;
		int index 			= 0;

		// Vertices
		for(int x = 0; x < m_numCellsX + 1; x++)
		{
			vertices[vertIndex] = new Vector3(x * m_cellSize, 0.0f, 0.0f);
			indices[vertIndex] = vertIndex;
			vertIndex++;

			vertices[vertIndex] = new Vector3(x * m_cellSize, m_gridSizeY, 0.0f);
			indices[vertIndex] = vertIndex;
			vertIndex++;
		}

		for(int y = 0; y < m_numCellsY + 1; y++)
		{
			vertices[vertIndex] = new Vector3(0.0f, y * m_cellSize, 0.0f);
			indices[vertIndex] = vertIndex;
			vertIndex++;
			
			vertices[vertIndex] = new Vector3(m_gridSizeX, y * m_cellSize, 0.0f);
			indices[vertIndex] = vertIndex;
			vertIndex++;
		}

		m_debugMesh.vertices = vertices;
		m_debugMesh.SetIndices(indices, MeshTopology.Lines, 0);
		m_debugMesh.RecalculateBounds();
	}
}

