using UnityEngine;
using System.Collections;

public interface ISelectable
{
	// Return true to consume the event
	bool Select();
	void Deselect();
}
