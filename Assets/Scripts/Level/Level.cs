using UnityEngine;
using System.Collections.Generic;

public class Level : MonoBehaviour 
{
	void Start () {}

	public void StartLevel()
	{
		SpriteRenderer newSprite	= gameObject.AddComponent<SpriteRenderer>();
		newSprite.material.color = Color.white;
		//newSprite.sprite = 

		// TODO: Why the tits won't Resources.Load find individual sprites when packed :{
		Sprite[] sprites = Resources.LoadAll<Sprite>("Textures"); 
		if(sprites.Length > 0)
		{
			for(int spriteIndex = 0; spriteIndex < sprites.Length; spriteIndex++)
			{
				if(sprites[spriteIndex].name == "tile_floor")
				{
					newSprite.sprite = sprites[spriteIndex];
					Debug.Log("Loaded material!");
					break;
				}
			}

			//newSprite.sprite = floorMat as Sprite;
		}

		transform.localScale = new Vector3(100.0f, 100.0f, 1.0f);
		Vector3 position = transform.position;
		position.z = 1;
		transform.position = position;
	}
}
 