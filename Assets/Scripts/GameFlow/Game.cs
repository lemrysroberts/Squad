// Spawns everything and controls input routing. Probably shouldn't

using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour 
{
	private bool m_gameStarted 					= false;
	private Level m_level						= null;
	private List<Team> m_teams 					= new List<Team>();
	private List<Selectable> m_selectedObjects 	= new List<Selectable>();
	private Entity m_clickedEntity = null;
	private EntityAction m_currentAction = null;

	void Start () {}

	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			foreach(var team in m_teams)
			{
				team.ExecutePlan();
			}
		}
	}

	private void StartGame()
	{
		SpawnLevel();
		SpawnTeams();
		
		m_gameStarted = true;
	}

	private void SpawnLevel()
	{
		GameObject newLevelObject 		= new GameObject("level");
		newLevelObject.transform.parent = transform;

		m_level = newLevelObject.AddComponent<Level>();
		m_level.StartLevel();
	}

	private void SpawnTeams()
	{
		const int numTeams 			= 2;
		const int numTeamMembers 	= 3;
		
		for(int teamCount = 0; teamCount < numTeams; teamCount++)
		{
			GameObject newTeamObject 		= new GameObject("team_" + teamCount);
			newTeamObject.transform.parent 	= m_level.transform;
			
			Team newTeam = newTeamObject.AddComponent<Team>();
			
			newTeam.InitTeam(teamCount); 
			
			m_teams.Add(newTeam);
			
			for(int teamMemberCount = 0; teamMemberCount < numTeamMembers; teamMemberCount++)
			{
				newTeam.SpawnTeamMember(new Vector2(Random.Range(0.0f, 10.0f), Random.Range(0.0f, 10.0f)));
			}
		}
	}

	public void OnGUI()
	{
		if(!m_gameStarted)
		{
			if(GUI.Button(new Rect(10, 10, 90, 30), "Start Game"))
			{
				StartGame();
			}
		}
	}
}
