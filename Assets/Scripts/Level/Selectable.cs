using UnityEngine;
using System.Collections;

public class Selectable : MonoBehaviour 
{
	public ISelectable Target = null;

	public bool Select()
	{
		if(Target != null)
		{
			return Target.Select();
		}
		return false;
	}

	public void Deselect()
	{
		if(Target != null)
		{
			Target.Deselect();
		}
	}
}
