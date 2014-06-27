// Spawns everything and controls input routing. Probably shouldn't

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Game : MonoBehaviour 
{
	private bool m_gameStarted 					= false;
	private Level m_level						= null;
	private List<Team> m_teams 					= new List<Team>();
	private List<Selectable> m_selectedObjects 	= new List<Selectable>();
	private Entity m_clickedEntity = null;
	private EntityAction m_currentAction = null;

	void Start () 
	{
		m_level = GameObject.FindObjectOfType<Level>();
	}

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
		m_level.StartLevel();
	}

	private void SpawnTeams()
	{
		const int numTeams 			= 1;
		const int numTeamMembers 	= 3;
		const float spawnWidth 		= 10.0f;

		SpawnPoint[] spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();
		
		for(int teamCount = 0; teamCount < numTeams; teamCount++)
		{
			SpawnPoint spawnPoint = spawnPoints.First(x => x.TeamID == teamCount);


			GameObject newTeamObject 		= new GameObject("team_" + teamCount);
			newTeamObject.transform.parent 	= m_level.transform;
			
			Team newTeam = newTeamObject.AddComponent<Team>();
			
			newTeam.InitTeam(teamCount); 
			
			m_teams.Add(newTeam);

			Vector3 spawnStartPoint = spawnPoint != null ? spawnPoint.transform.position : Vector3.zero;
			spawnStartPoint.x -= spawnWidth / 2.0f;

			float deltaX = spawnWidth / (float)numTeamMembers;
			
			for(int teamMemberCount = 0; teamMemberCount < numTeamMembers; teamMemberCount++)
			{
				Vector2 spawnPosition = (Vector2)spawnStartPoint;
				spawnPosition.x = spawnStartPoint.x + (deltaX * teamMemberCount);

				newTeam.SpawnTeamMember(spawnPosition);
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
