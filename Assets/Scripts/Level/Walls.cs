using UnityEngine;
using System.Collections.Generic;

public class Walls 
{
	private LevelGrid m_grid = null;
	private List<WallChunk> m_chunks = new List<WallChunk>();
	private List<WallChunk> m_destroyedThisFrame = new List<WallChunk>();

	public Walls(LevelGrid grid)
	{
		m_grid = grid;

		for(int x = 0; x < m_grid.NumCellsX; x++)
		{
			for(int y = 0; y < m_grid.NumCellsY; y++)
			{
				if(Random.value > 0.9f)
				{
					WallChunk newChunk = new WallChunk(this);
					m_chunks.Add(newChunk);

					m_grid.GetCell(x, y).AddContents(newChunk);
				}
			}
		}
	}

	public void ChunkDestroyed(WallChunk chunk)
	{
		m_destroyedThisFrame.Add(chunk);
	}

	public void LateUpdate()
	{
		// I assume this will do something more useful in the near future.
		if(m_destroyedThisFrame.Count > 0)
		{
			Level.Instance.UpdateDebugMeshes();
			m_destroyedThisFrame.Clear();
		}
	}
}
