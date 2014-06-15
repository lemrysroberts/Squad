using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour 
{
	public float FalloffSquared 			= 10.0f;
	public float DampenRate					= 1.0f;

	private static ScreenShake s_instance 	= null;

	private float m_currentShake 			= 0.0f;
	private Vector2 m_shakeOffset 			= Vector2.zero;

	public static ScreenShake Instance { get { return s_instance; } }

	// Use this for initialization
	void Start () 
	{
		if(s_instance != null)
		{
			Debug.LogError("ScreenShake already registered!");
		}
		s_instance = this;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 actualCameraPosition = transform.position - (Vector3)m_shakeOffset;

		m_shakeOffset.x = (Mathf.PerlinNoise(Random.value, Random.value) * 2.0f - 1.0f) * m_currentShake;
		m_shakeOffset.y = (Mathf.PerlinNoise(Random.value, Random.value) * 2.0f - 1.0f) * m_currentShake;

		transform.position = actualCameraPosition + (Vector3)m_shakeOffset;

		m_currentShake -= (DampenRate * Time.deltaTime);
		m_currentShake = Mathf.Max(0.0f, m_currentShake);
	}

	public void AddShake(float shakeAmount, Vector2 shakeSource)
	{
		float distanceSquared = ((Vector2)transform.position - shakeSource).sqrMagnitude;

		m_currentShake = Mathf.Max(m_currentShake, (shakeAmount * Mathf.Clamp01(FalloffSquared / distanceSquared)));
		m_currentShake = Mathf.Clamp01(m_currentShake);
	}
}
