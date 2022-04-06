using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTurnManager : MonoBehaviour
{
	//List of all characters
	static Dictionary<string, List<GridMovement>> characters = new Dictionary<string, List<GridMovement>>();

	//Queue to determine whose turn it is on a team
	static Queue<GridMovement> whoseTeam = new Queue<GridMovement>();

	//Queue to determine whose team is going
	static Queue<string> whoseTurn = new Queue<string>();

    // Start is called before the first frame update
    void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
		if (whoseTeam.Count == 0) 
		{
			InitializeTeamTurnQueue();
		}
	}

	//Determine whose team should be going
	static void InitializeTeamTurnQueue() 
	{
		List<GridMovement> teamList = characters[whoseTurn.Peek()];

		//Initialize next team before their turn starts
		foreach(GridMovement character in teamList) 
		{
			whoseTeam.Enqueue(character);
		}

		StartTurn();
	}

	static void StartTurn() 
	{
		if(whoseTeam.Count > 0) 
		{
			whoseTeam.Peek().BeginTurn();
		}
	}

	public static void EndTurn() 
	{
		GridMovement character = whoseTeam.Dequeue();
		character.EndTurn();

		//if any other character needs to go on a team, start their turn
		if(whoseTeam.Count > 0) 
		{
			StartTurn();
		}
		//else move on to next team
		else 
		{
			string team = whoseTurn.Dequeue();
			//Move team to back of Queue so they can go again
			whoseTurn.Enqueue(team);
			//Next team has its turn
			InitializeTeamTurnQueue();
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

			if(!whoseTurn.Contains(character.tag)) 
			{
				whoseTurn.Enqueue(character.tag);
			}
		}
		//else if character already exists, assign it in list
		else 
		{
			list = characters[character.tag];
		}

		list.Add(character);
	}

	//TODO: Remove character if killed
}
