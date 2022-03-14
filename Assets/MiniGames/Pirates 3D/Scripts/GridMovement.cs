using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
	//List that shows tiles the player can select while it is their turn
	List<Tile> selectableTiles = new List<Tile>();

	//List for all tiles in grid for adjacency checks
	GameObject[] tiles;

	//Path for movement
	Stack<Tile> path = new Stack<Tile>();

	//Tile that player is on at beginning of each turn
	Tile currentTile;

	//Default moves per turn, speed of moves, and flag for player moving
	public int move = 5;
	public float moveSpeed = 2;
	public bool moving = false;

	//How fast player moves from tile to tile and direction they're facing
	Vector3 velocity = new Vector3();
	Vector3 direction = new Vector3();

	//Determine where player is on a tile by getting their half height
	float halfHeight = 0;

	protected void Initialize() 
	{
		tiles = GameObject.FindGameObjectsWithTag("Tile");

		halfHeight = GetComponent<Collider>().bounds.extents.y;
	}

	//Get starting point for pathfinding
	public void GetCurrentTile() 
	{
		currentTile = GetTargetTile(gameObject);
		currentTile.current = true;
	}

	//Get tile we want to move to
	public Tile GetTargetTile(GameObject target) 
	{
		RaycastHit hit;
		Tile tile = null;
		if(Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1)) 
		{
			tile = hit.collider.GetComponent<Tile>();
		}

		return tile; 
	}

	//Calculate adjacency list for each tile
	public void CalculateAdjacencyLists() 
	{
		foreach(GameObject tile in tiles) 
		{
			Tile t = tile.GetComponent<Tile>();
			t.FindNeighbors();
		}
	}

	//BFS Search for selectable tiles
	public void FindSelectableTiles() 
	{
		CalculateAdjacencyLists();
		GetCurrentTile();

		Queue<Tile> process = new Queue<Tile>();

		process.Enqueue(currentTile);
		currentTile.visited = true;

		//Make all adjacent tiles selectable
		while(process.Count > 0) 
		{
			Tile t = process.Dequeue();

			selectableTiles.Add(t);
			t.selectable = true;

			//if we aren't outside our max move distance
			//process all nodes inside our move space
			if (t.distance < move) 
			{
				foreach (Tile tile in t.adjacencyList) 
				{
					if (!tile.visited) 
					{
						tile.parent = t;
						tile.visited = true;
						tile.distance = 1 + t.distance;
						process.Enqueue(tile);
					}
				}
			}
		}
	}


	public void MoveToTile(Tile tile) 
	{
		path.Clear();
		tile.target = true;
		moving = true;

		//Start with target tile(end location) and work backwards
		Tile next = tile;
		//Once start tile is reached, stack will have entire path
		//We can then move along that path in proper order
		while(next != null) 
		{
			path.Push(next);
			next = next.parent;
		}
	}

	public void Move() 
	{
		//if path still has a move left
		if(path.Count > 0) 
		{
			//Get tile we are moving to
			Tile t = path.Peek();
			Vector3 target = t.transform.position;

			//To make sure we move ONTO instead of INTO tile
			target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

			//Move to tile
			if(Vector3.Distance(transform.position, target) >= 0.05f) 
			{
				CalculateDirection(target);
				SetVelocity();

				transform.forward = direction;
				transform.position += velocity * Time.deltaTime;
			}
			else 
			{
				//Tile center reached
				transform.position = target;
				path.Pop();
			}
		}
		//We have reached the target tile
		else 
		{
			RemoveSelectableTiles();
			moving = false;
		}
	}

	//When character has reached target tile and turn ends for them
	//Reset and clear everything
	protected void RemoveSelectableTiles() 
	{
		if(currentTile != null) 
		{
			currentTile.current = false;
			currentTile = null;
		}
		foreach(Tile tile in selectableTiles) 
		{
			tile.Reset();
		}

		selectableTiles.Clear();
	}

	//Direction
	void CalculateDirection(Vector3 target) 
	{
		direction = target - transform.position;
		direction.Normalize();
	}

	//Speed
	void SetVelocity() 
	{
		velocity = direction * moveSpeed;
	}
}
