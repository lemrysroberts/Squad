using UnityEngine;
using System.Collections;

public abstract class InputListener : MonoBehaviour 
{
	public abstract bool MouseButtonDown(int mouseButton);
	public abstract bool MouseButtonUp(int mouseButton);

	public abstract void MouseMove(Vector2 worldPosition);
	public abstract void MouseOver();
}
