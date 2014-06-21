///////////////////////////////////////////////////////////
// 
// LevelGrid_debug.cs
//
// What it does: 
//
// Notes: 	
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public partial class LevelGrid
{
	private Mesh m_debugMesh 	= null;
	private Mesh m_contentsMesh = null;

	public Mesh DebugMesh { get { return m_debugMesh; } }
	public Mesh ContentsMesh { get { return m_contentsMesh; } }

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
	
	private void RebuildContentsMesh()
	{
		m_contentsMesh = new Mesh();
		
		Vector3[] verts = new Vector3[m_numCellsX * m_numCellsY * 4];
		Color[] colors = new Color[m_numCellsX * m_numCellsY * 4];
		int[] indices = new int[m_numCellsX * m_numCellsY * 6];
		
		int vertIndex = 0;
		int index = 0;
		
		for(int y = 0; y < m_numCellsY; y++)
		{
			for(int x = 0; x < m_numCellsX; x++)
			{
				verts[vertIndex] = new Vector3(x * m_cellSize, y * m_cellSize, 0.0f);
				verts[vertIndex + 1] = new Vector3((x + 1) * m_cellSize, y * m_cellSize, 0.0f);
				verts[vertIndex + 2] = new Vector3(x * m_cellSize, (y + 1) * m_cellSize, 0.0f);
				verts[vertIndex + 3] = new Vector3((x + 1) * m_cellSize, (y + 1) * m_cellSize, 0.0f);
				
				Color color = ((m_cells[x, y].m_contentsMask & (int)LevelGrid.GridCellContents.Blocked) != 0) ? Color.red : Color.white;
				
				colors[vertIndex] = color;
				colors[vertIndex + 1] = color;
				colors[vertIndex + 2] = color;
				colors[vertIndex + 3] = color;
				
				indices[index] = vertIndex;
				indices[index + 1] = vertIndex + 2;
				indices[index + 2] = vertIndex + 1; 
				indices[index + 3] = vertIndex + 1;
				indices[index + 4] = vertIndex + 2;
				indices[index + 5] = vertIndex + 3;
				
				vertIndex+=4;
				index+= 6;
			}
		}
		
		
		m_contentsMesh.vertices = verts;
		m_contentsMesh.colors = colors;
		m_contentsMesh.SetIndices(indices, MeshTopology.Triangles, 0);
		m_contentsMesh.RecalculateBounds();
	}
}
