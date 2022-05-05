using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTurnManager : MonoBehaviour
{
	//List of all characters
	static Dictionary<string, List<GridMovement>> characters = new Dictionary<string, List<GridMovement>>();

	//Queue to determine whose turn it is on a team
	static Queue<GridMovement> whoseTurn = new Queue<GridMovement>();

	//Queue to determine whose team is going
	static Queue<string> whoseTeam = new Queue<string>();

    // Start is called before the first frame update
    void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
		if (whoseTurn.Count == 0) 
		{
			InitializeTeamTurnQueue();
		}
	}

	//Determine whose team should be going
	static void InitializeTeamTurnQueue() 
	{
		List<GridMovement> teamList = characters[whoseTeam.Peek()];

		//Initialize next team before their turn starts
		foreach(GridMovement character in teamList) 
		{
			whoseTurn.Enqueue(character);
		}

		StartTurn();
	}

	static void StartTurn() 
	{
		if(whoseTurn.Count > 0) 
		{
			whoseTurn.Peek().BeginTurn();
		}
	}

	public static void EndTurn() 
	{
		GridMovement character = whoseTurn.Dequeue();
		character.EndTurn();

		Debug.Log("Turn is ending.");
		//if any other character needs to go on a team, start their turn
		if(whoseTurn.Count > 0) 
		{
			StartTurn();
			Debug.Log("Start next teammate turn.");
		}
		//else move on to next team
		else 
		{
			string team = whoseTeam.Dequeue();
			//Move team to back of Queue so they can go again
			whoseTeam.Enqueue(team);
			//Next team has its turn
			InitializeTeamTurnQueue();
			Debug.Log("Move to next team.");
		}
	}

	//Add character to characters list
	public static void AddUnit(GridMovement character) 
	{
		List<GridMovement> list;

		//if character has not been added to dictionary or already in turn queue, add em
		if(!characters.ContainsKey(character.tag)) 
		{
			list = new List<GridMovement>();
			characters[character.tag] = list;

			if(!whoseTeam.Contains(character.tag)) 
			{
				whoseTeam.Enqueue(character.tag);
			}
		}
		//else if character already exists, assign it in list
		else 
		{
			list = characters[character.tag];
		}

		list.Add(character);
	}

	public static void RemoveUnit(GameObject unit) 
	{
		whoseTurn.Dequeue();
		characters.Remove(unit.name);
		Debug.Log("Unit is out of turn order.");
	}
}
