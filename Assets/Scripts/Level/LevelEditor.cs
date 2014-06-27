using UnityEngine;
using UnityEditor;
using System.Collections;

public class LevelEditor 
{
	private static bool Editing = false;

	private static LevelGrid m_grid = null;
	private static LevelLayout m_layout = null;
	private static Level m_level = null;

	public static void OnEnable()
	{
		m_level = GameObject.FindObjectOfType<Level>();
		if(m_level != null)
		{
			m_level.Start();
		}
		m_layout = new LevelLayout();
		m_layout.Init(100, 100);
	}

	public static void OnHubGUI()
	{
		if(m_level == null || m_layout == null)
		{
			m_level = GameObject.FindObjectOfType<Level>();
			return;
		}

		Editing = EditorGUILayout.Toggle("Editing", Editing);

		GUI.enabled = m_layout.Dirty;

		if(GUILayout.Button("Save"))
		{
			string path = EditorUtility.SaveFilePanelInProject("Save Level", "newlevel", "xml", "Save levels in \"Resources/Levels\"!");

			m_layout.Serialise(path);
		}

		GUI.enabled = true;

		if(GUILayout.Button("Load"))
		{
			string path = EditorUtility.OpenFilePanel("Open Level", Application.dataPath + "/Resources/Levels", "xml");

			if(!string.IsNullOrEmpty(path))
			{
				m_layout = LevelLayout.Deserialise(path);
				Level.Instance.LoadFromLayout(m_layout);
			}
		}
	}

	private static void OnEditing()
	{

	}

	public static void OnSceneGUI()
	{
		if(m_level == null) return;
		if(Editing)
		{
		//	if(m_grid.ContentsMesh == null) { m_grid.RebuildMeshes(); }

			var e = Event.current;
			
			Vector2 worldPos = Camera.main.ScreenToWorldPoint(Event.current.mousePosition);
			
			Ray targetRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			Plane zeroPlane = new Plane(new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f));
			
			float hit;
			if(zeroPlane.Raycast(targetRay, out hit))
			{
				var hitLocation = targetRay.GetPoint(hit);

				float size = HandleUtility.GetHandleSize(hitLocation) * 0.2f;
				Handles.SphereCap(0, hitLocation, Quaternion.identity, size);

				if((e.type == EventType.MouseDrag || e.type == EventType.MouseDown ) )
				{
					Point gridPosition = GameObject.FindObjectOfType<Level>().GetGrid().GetCellIndices(hitLocation);

					if(gridPosition != null)
					{
						m_layout.SetTileContents(gridPosition.x, gridPosition.y, e.button == 0);
						
						Level.Instance.LoadFromLayout(m_layout);
					}
				}
			}

			if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown ) && (e.button == 0 || e.button == 1) ||  (e.type == EventType.Layout))
			{
				Event.current.Use();
				
				
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			}
		}
	}
}
