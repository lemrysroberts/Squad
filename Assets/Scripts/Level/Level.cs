using UnityEngine;
using System.Collections.Generic;

public class Level : MonoBehaviour 
{
	void Start () {}

	public void StartLevel()
	{
		transform.localScale = new Vector3(100.0f, 100.0f, 1.0f);
		Vector3 position = transform.position;
		position.z = 1;
		transform.position = position;
	}
}
 