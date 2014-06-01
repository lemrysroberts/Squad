using UnityEngine;
using System.Collections;

public class Debug_CameraMove : MonoBehaviour 
{
	public float MoveSpeed = 1.0f;
	public float LerpSpeed = 0.1f;

	private Vector3 m_targetPosition;

	// Use this for initialization
	void Start () 
	{
		m_targetPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 delta = Vector3.zero;

		if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { delta.x += MoveSpeed * Time.deltaTime; }
		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) 	{ delta.x -= MoveSpeed * Time.deltaTime; }
		if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) 	{ delta.y += MoveSpeed * Time.deltaTime; }
		if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) 	{ delta.y -= MoveSpeed * Time.deltaTime; }

		m_targetPosition += delta;

		transform.position = Vector3.Lerp(transform.position, m_targetPosition, LerpSpeed);
	}
}
