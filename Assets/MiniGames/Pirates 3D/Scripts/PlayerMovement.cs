using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : GridMovement
{
	void Start() 
	{
		Initialize();
	}

	void Update() 
	{
		//Don't allow character to do anything if it isn't their turn
		if(!turn) 
		{
			return;
		}
		//if moving disabled finding adjacent tiles or clicking tile to walk to
		if(!moving) 
		{
			FindSelectableTiles();
			CheckMouseClick();
		}
		else 
		{
			Move();
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
				}
			}
		}
	}
}
