using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : GridMovement
{
	public Button endTurn;

	void Start() 
	{
		Initialize();
		endTurn.onClick.AddListener(EndPlayerTurn);
	}

	void Update() 
	{
		//Don't allow character to do anything if it isn't their turn
		if(!turn) 
		{
			endTurn.gameObject.SetActive(false);
			return;
		}
		if(turn) 
		{
			endTurn.gameObject.SetActive(true);
		}
		//if moving disabled finding adjacent tiles or clicking tile to walk to
		if(!moving && !hasMoved) 
		{
			FindSelectableTiles();
			CheckMouseClick();
		}
		else 
		{
			Move();
		}
		if(hasMoved && !moving && attacking) 
		{
			ShowAttackRange();
			CheckMouseClick();
		}
	}

	//Check if player has clicked selectable tile
	//TODO: Check if player has clicked an enemy
	void CheckMouseClick() 
	{
		if(Input.GetMouseButtonUp(0)) 
		{
			//Ray from camera outwards to tile
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;

			//if player clicks a tile
			if(Physics.Raycast(ray, out hit)) 
			{
				if(hit.collider.tag == "Tile") 
				{
					Tile t = hit.collider.GetComponent<Tile>();

					if(t.selectable) 
					{
						MoveToTile(t);
					}
					if(t.attackRange) 
					{
						if(Physics.Raycast(hit.transform.position, Vector3.up, out hit, 1) && hit.collider.tag == "Tile") 
						{
							Attack();
						}
					}
				}

				else if(hit.collider.tag == "EnemyPiece") 
				{
					if(Physics.Raycast(hit.transform.position, Vector3.down, out hit, 1) && hit.collider.tag == "Tile") 
					{
						Tile t = hit.collider.GetComponent<Tile>();

						if(t.attackRange) 
						{
							Attack();
						}
					}
				}
			}
		}
	}

	void EndPlayerTurn() 
	{
		RemoveAttackRangeTiles();
		hasMoved = false;
		GridTurnManager.EndTurn();
	}
}
