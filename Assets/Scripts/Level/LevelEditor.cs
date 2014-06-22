using UnityEngine;
using UnityEditor;
using System.Collections;

public class LevelEditor 
{
	private static bool Editing = false;

	//private static Mesh

	public static void OnHubGUI()
	{
		Editing = EditorGUILayout.Toggle("Editing", Editing);
	}

	private static void OnEditing()
	{

	}

	public static void OnSceneGUI()
	{
		if(Editing)
		{
			var e = Event.current;
			
			Vector2 worldPos = Camera.main.ScreenToWorldPoint(Event.current.mousePosition);
			
			Ray targetRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			Plane zeroPlane = new Plane(new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f));
			
			float hit;
			if(zeroPlane.Raycast(targetRay, out hit))
			{
				var hitLocation = targetRay.GetPoint(hit);
				Handles.SphereCap(0, hitLocation, Quaternion.identity, 3.0f);
			}
		
			if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown ) && (e.button == 0 || e.button == 1) ||  (e.type == EventType.Layout))
			{
				Event.current.Use();
				
				
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			}


		}


	}
}
