///////////////////////////////////////////////////////////
// 
// SquadEditorHub.cs
//
// What it does: Provides a central panel for all my weird shitty functions
//
// Notes: 	
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SquadEditorHub :  EditorWindow 
{
    [MenuItem ("Squad/Show Editor Hub")]
    static void ShowWindow () 
	{
        EditorWindow.GetWindow(typeof(SquadEditorHub));
    }

	void OnEnable()
	{
		if(SceneView.onSceneGUIDelegate != this.OnSceneGUI)
		{
			SceneView.onSceneGUIDelegate += this.OnSceneGUI;
		}
	}
	
	void OnGUI()
	{
		CheckTextures();
		
		EditorGUILayout.BeginVertical((GUIStyle)"Box");

		GUILayout.Label("Squad Editor Hub", EditorStyles.boldLabel);

		OnLevelEditGUI();
	}

	void OnLevelEditGUI()
	{
		GUILayout.BeginVertical((GUIStyle)"Box");

		GUILayout.Label("Level Edit");

		LineBreak();

		EditorGUILayout.BeginHorizontal();

		LevelEditor.OnHubGUI();

		EditorGUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}

	void OnSceneGUI(SceneView sceneView)
	{
		LevelEditor.OnSceneGUI();
		sceneView.Repaint();
	}

	// Check all GUI textures are loaded and load them if not.
	void CheckTextures()
	{
		if (s_StartPaintingTexture == null)		{ s_StartPaintingTexture = (Texture2D)Resources.Load<Texture2D>("editor_images/editor_icon_paint"); }
	}

	void LineBreak()
	{
		GUILayout.Box("", GUILayout.Height(1), GUILayout.Width(Screen.width - 20));
	}

	static int s_buttonSize = 40;

	static Texture2D s_StartPaintingTexture		= null;
}
