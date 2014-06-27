/////////////////////////////////////////////////////////////////////
// 
// LevelLayout.cs
//
// What it does: It should store all the level-data, but for now it's just the
//				 dimensions and whether each cell is blocked
//
// Notes: 
// 
// To-do: All of it.
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.IO;
using System.Collections;

using Newtonsoft.Json;

public class LevelLayout
{
	public int m_levelSizeX = 0;
	public int m_levelSizeY = 0;

	private bool m_dirty 		= false;

	public bool[][] m_contents 	= null;

	public bool[][] GetContents() { return m_contents; }
	public bool Dirty { get { return m_dirty; } }

	public void Init(int sizeX, int sizeY) 
	{
		m_levelSizeX = sizeX;
		m_levelSizeY = sizeY;

		m_contents = new bool[m_levelSizeX][];

		for(int x = 0; x < m_levelSizeX; x++)
		{
			m_contents[x] = new bool[m_levelSizeY];
			for(int y = 0; y < m_levelSizeY; y++)
			{
				m_contents[x][y] = false;
			}
		}
	}

	public void SetTileContents(int x, int y, bool contents)
	{
		m_contents[x][y] = contents;
		m_dirty = true;
	}

	// TODO: move the serialisation. It would be great if it wasn't manual, but c#'s xml serialisation is pish
	public void Serialise(string path)
	{
		using (StreamWriter sw = new StreamWriter(path))
		{
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.Formatting = Formatting.Indented;
				
				writer.WriteStartObject();

				writer.WritePropertyName("size_x");
				writer.WriteValue(m_levelSizeX);

				writer.WritePropertyName("size_y");
				writer.WriteValue(m_levelSizeY);
				
				writer.WritePropertyName("tiles");
				writer.WriteStartArray();
				for(int x = 0; x < m_levelSizeX; x++)
				{
					for(int y = 0; y < m_levelSizeY; y++)
					{
						writer.WriteStartObject();
						
						writer.WritePropertyName("blocked");
						writer.WriteValue(m_contents[x][y]);

						writer.WriteEndObject();
					}
				}
				writer.WriteEndArray();
				
				writer.WriteEndObject();
			}
		}
	}

	public static LevelLayout Deserialise(string path)
	{
		LevelLayout newLayout = new LevelLayout();

		using(TextReader tr = new StreamReader(path))
		{
			using(JsonReader reader = new JsonTextReader(tr))
			{
				int tileCount = 0;

				while(reader.Read())
				{
					if(reader.TokenType == JsonToken.PropertyName)
					{
						if (reader.Value.ToString() == "size_x") 
						{
							reader.Read();
							if(int.TryParse(reader.Value.ToString(), out newLayout.m_levelSizeX))
							{
								newLayout.m_contents = new bool[newLayout.m_levelSizeX][];
							}
						}

						if (reader.Value.ToString() == "size_y") 
						{
							reader.Read();
							if(int.TryParse(reader.Value.ToString(), out newLayout.m_levelSizeY))
							{
								for(int x = 0; x < newLayout.m_levelSizeX; x++)
								{
									newLayout.m_contents[x] = new bool[newLayout.m_levelSizeY];
								}
							}
						}
						
						// Categories
						if (reader.Value.ToString() == "blocked") 
						{
							reader.Read();
							int x = tileCount / newLayout.m_levelSizeX;
							int y = tileCount % newLayout.m_levelSizeX;

							bool.TryParse(reader.Value.ToString(), out newLayout.m_contents[x][y]);

							tileCount++;
						}
					}
				}
			}
		}

		return newLayout;
	}
}
