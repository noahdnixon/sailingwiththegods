using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : GridMovement
{
	//The player controlled character we want to move to
	GameObject target;

    // Start is called before the first frame update
    void Start()
    {
		Initialize();
    }

    // Update is called once per frame
    void Update()
    {
		//Don't allow character to do anything if it isn't their turn
		if (!turn) 
		{
			hasMoved = false;
			return;
		}
		//if moving disabled finding adjacent tiles
		if (!moving && !hasMoved) 
		{
			//Show potential moves of the enemy
			FindSelectableTiles();

			//Find target and path to them
			FindNearestTarget();
			CalculatePath();

			//Turns tile this enemy wants to move to green
			targetTile.target = true;
		}
		else 
		{
			Move();
		}
		if (hasMoved) 
		{
			GridTurnManager.EndTurn();
		}

	}

	//Find where enemy is going to move to
	void CalculatePath() 
	{
		Tile targetTile = GetTargetTile(target);
		FindPath(targetTile);
	}

	void FindNearestTarget() 
	{
		//All targets will be characters with "PlayerPiece" tag
		GameObject[] targets = GameObject.FindGameObjectsWithTag("PlayerPiece");

		GameObject nearest = null;
		float distance = Mathf.Infinity;

		//foreach target in our list of "PlayerPiece" game objects,
		//calculate distance of each from this enemy and choose closest
		foreach(GameObject targ in targets) 
		{
			float d = Vector3.Distance(transform.position, targ.transform.position);

			//Find min
			if(d < distance) 
			{
				distance = d;
				nearest = targ;
			}
		}

		target = nearest;
	}
}
