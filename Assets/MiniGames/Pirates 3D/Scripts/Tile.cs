using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	//Current tile
	public bool current = false;

	//Tile we want to move to
	public bool target = false;

	//If tile is within our move range
	public bool selectable = false;

	//Used for obstacles or if other characters are on tile
	public bool walkable = true;

	//BFS Player Movement Variables
	//Used for BFS player movement
	public List<Tile> adjacencyList = new List<Tile>();

	//Determines if tile has been processed
	public bool visited = false;
	//Used for pathfinding
	public Tile parent = null;
	//How far each tile is from current tile
	public int distance = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		//Use color to show where the players/enemies can move
        if(current) 
		{
			GetComponent<Renderer>().material.color = Color.magenta;
		}
		else if(target) 
		{
			GetComponent<Renderer>().material.color = Color.green;
		}
		else if(selectable)
		{
			GetComponent<Renderer>().material.color = Color.red;
		}
		else 
		{
			GetComponent<Renderer>().material.color = Color.white;
		}

    }

	//Reset all variables after each turn
	public void Reset() 
	{
		current = false;
		target = false;
		selectable = false;
		adjacencyList.Clear();
		visited = false;
		parent = null;
		distance = 0;
	}

	public void FindNeighbors() 
	{
		Reset();

		CheckTile(Vector3.forward);
		CheckTile(-Vector3.forward);
		CheckTile(Vector3.right);
		CheckTile(-Vector3.right);
	}

	public void CheckTile(Vector3 direction) 
	{
		//Check for adjacent tiles
		Vector3 halfExtents = new Vector3(0.25f, 1.0f, 0.25f);
		Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

		//Add adjacent tiles that satisfy conditions to adjacent tile list
		foreach(Collider coll in colliders) 
		{
			Tile tile = coll.GetComponent<Tile>();

			if(tile != null && tile.walkable) 
			{
				RaycastHit hit;

				//if tile doesn't have someone/something on it, add to list
				if(!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1)) 
				{
					adjacencyList.Add(tile);
				}
			}
		}
	}
}
