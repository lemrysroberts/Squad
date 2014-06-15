using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour 
{
	private InputListener m_activeListener = null;

	private static InputManager s_instance = null;

	public static InputManager Instance { get { return s_instance; } }

	void Start()
	{
		if(s_instance != null)
		{
			Debug.LogError("InputManager already registered");
		}
		s_instance = this;
	}

	// Update is called once per frame
	void Update () 
	{
		Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPosition, Vector2.zero);


		if(m_activeListener != null)
		{
			m_activeListener.MouseMove(mouseWorldPosition);

			if(Input.GetMouseButtonDown(0)) { m_activeListener.MouseButtonDown(0); }
			else if(Input.GetMouseButtonDown(1)) { m_activeListener.MouseButtonDown(1); }
			else if(Input.GetMouseButtonUp(0)) { m_activeListener = m_activeListener.MouseButtonUp(0) ? m_activeListener : null; }
			else if(Input.GetMouseButtonUp(1)) { m_activeListener.MouseButtonUp(1); }
		}
		else
		{
			foreach(var hit in hits)
			{
				InputListener listener = hit.collider.gameObject.GetComponent<InputListener>();

				if(listener != null)
				{
					if(Input.GetMouseButtonDown(0)) { m_activeListener = listener.MouseButtonDown(0) ? listener : null; }
					if(Input.GetMouseButtonDown(1)) { m_activeListener = listener.MouseButtonDown(1) ? listener : null; }
					if(Input.GetMouseButtonUp(0)) { m_activeListener = listener.MouseButtonUp(0) ? listener : null; }
					if(Input.GetMouseButtonUp(1)) { m_activeListener = listener.MouseButtonUp(1) ? listener : null; }

					if(m_activeListener != null) { break; }
				}
			}
		}
	}

	public void SetActiveListener(InputListener listener)
	{
		m_activeListener = listener;
	}
}
