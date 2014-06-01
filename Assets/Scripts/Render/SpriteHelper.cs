using UnityEngine;
using System.IO;
using System.Collections;

public class SpriteHelper
{
	// Remove this when Unity decides to let me load an individual, packed sprite
	public static Sprite LoadSprite(string path)
	{
		string directory = System.IO.Path.GetDirectoryName(path);
		string filename = System.IO.Path.GetFileNameWithoutExtension(path);

		Sprite[] sprites = Resources.LoadAll<Sprite>(directory); 
		if(sprites.Length > 0)
		{
			for(int spriteIndex = 0; spriteIndex < sprites.Length; spriteIndex++)
			{
				if(sprites[spriteIndex].name == filename)
				{
					Debug.Log("Loaded material!");
					return sprites[spriteIndex];
				}
			}
		}
		return null;
	}
}
