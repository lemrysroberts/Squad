using UnityEngine;
using System.Collections.Generic;

public class Team : MonoBehaviour 
{
	private bool m_initialised 			= false;
	private int m_teamID 				= -1;

	private List<Entity> m_teamMembers = new List<Entity>();

	public void InitTeam(int teamID)
	{
		if(!m_initialised)
		{
			m_teamID = teamID;
			m_initialised = true;
		}
		else
		{
			Debug.Log("Initialising team twice.");
		}
	}

	public void SpawnTeamMember(Vector2 location)
	{
		GameObject newTeamMemberObject = new GameObject("team_member");
		newTeamMemberObject.transform.parent = this.transform;
		newTeamMemberObject.transform.position = new Vector3(location.x, location.y, -1.0f);

		Entity newTeamMember 		= newTeamMemberObject.AddComponent<Entity>();


		m_teamMembers.Add(newTeamMember);
	}

	public void ExecutePlan()
	{
		foreach(var entity in m_teamMembers)
		{
			entity.ExecutePlan();
		}
	}

	// Use this for initialization
	void Start () {	}
	
	// Update is called once per frame
	void Update () { }
}
