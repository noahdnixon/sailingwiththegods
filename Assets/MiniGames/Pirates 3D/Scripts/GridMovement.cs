using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
	//This is only true when it's this character's turn
	public bool turn = false;

	//List that shows tiles the player can select while it is their turn
	List<Tile> selectableTiles = new List<Tile>();

	//List that shows tiles the player can select for attack after movement
	public List<Tile> attackRangeTiles = new List<Tile>();

	//List for all tiles in grid for adjacency checks
	GameObject[] tiles;

	//Path for movement
	Stack<Tile> path = new Stack<Tile>();

	//Tile that player is on at beginning of each turn
	Tile currentTile;

	//Default moves per turn, speed of moves, attack range, attack damage, flag for player moving, and if the player has moved
	public int move = 5;
	public float moveSpeed = 2;
	public bool moving = false;
	public int range = 3;
	public bool hasMoved = false;
	public bool attacking = false;
	public bool isAlive = true;

	//How fast player moves from tile to tile and direction they're facing
	Vector3 velocity = new Vector3();
	Vector3 direction = new Vector3();

	//Determine where player is on a tile by getting their half height
	float halfHeight = 0;

	//For A*
	public Tile targetTile;

	private Animator animator;

	protected void Initialize() 
	{
		tiles = GameObject.FindGameObjectsWithTag("Tile");

		halfHeight = GetComponent<Collider>().bounds.extents.y;

		//Add this character to turn manager
		GridTurnManager.AddUnit(this);

		animator = GetComponent<Animator>();
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
	public void CalculateAdjacencyLists(Tile target) 
	{
		foreach(GameObject tile in tiles) 
		{
			Tile t = tile.GetComponent<Tile>();
			t.FindNeighbors(target);
		}
	}

	public void CalculateAttackAdjacencyLists(Tile target) 
	{
		foreach(GameObject tile in tiles) 
		{
			Tile t = tile.GetComponent<Tile>();
			t.FindAttackNeighbors(target);
		}
	}
	//BFS Search for selectable tiles
	public void FindSelectableTiles() 
	{
		CalculateAdjacencyLists(null);
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

	public void ShowAttackRange() 
	{
		CalculateAttackAdjacencyLists(null);
		GetCurrentTile();

		Queue<Tile> process = new Queue<Tile>();

		process.Enqueue(currentTile);
		currentTile.visited = true;

		
		while (process.Count > 0) 
		{
			Tile t = process.Dequeue();

			attackRangeTiles.Add(t);
			t.attackRange = true;

			//if we aren't outside our max move distance
			//process all nodes inside our move space
			if (t.distance < range) 
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
				animator.SetBool("Walking", true);
				CalculateDirection(target);
				SetVelocity();

				transform.forward = direction;
				transform.rotation = Quaternion.Euler(0, 0, 0);
				transform.position += velocity * Time.deltaTime;
			}
			else 
			{
				animator.SetBool("Walking", false);
				//Tile center reached
				transform.position = target;
				path.Pop();
			}
		}
		//We have reached the target tile - stop movement and end turn
		else 
		{
			RemoveSelectableTiles();
			moving = false;

			hasMoved = true;
			attacking = true;
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

	protected void RemoveAttackRangeTiles() 
	{
		if (currentTile != null) 
		{
			currentTile.current = false;
			currentTile = null;
		}
		foreach (Tile tile in attackRangeTiles) 
		{
			tile.Reset();
		}

		//Debug.Log("attackRangeTiles is empty.");
		attackRangeTiles.Clear();
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

	protected Tile FindFMin(List<Tile> list) 
	{
		Tile lowest = list[0];

		foreach(Tile t in list) 
		{
			if(t.fCost < lowest.fCost) 
			{
				lowest = t;
			}
		}

		//Pull off open list and add to closed list
		list.Remove(lowest);

		return lowest;
	}

	//Similar to MoveToTile
	protected Tile FindEndTile(Tile targTile) 
	{
		Stack<Tile> pathTemp = new Stack<Tile>();

		//Don't stand ON player, but stand NEXT to them
		Tile next = targTile.parent;

		while(next != null) 
		{
			pathTemp.Push(next);
			next = next.parent;
		}

		//if path is within this enemy's move range, return tile to move to
		if(pathTemp.Count <= move) 
		{
			return targTile.parent;
		}

		//if out of range...only move to edge of this enemy's move range
		Tile endTile = null;
		for(int i = 0; i <= move; i++) 
		{
			endTile = pathTemp.Pop();
		}

		return endTile;
	}

	//A* Algorithm for enemy movement
	//Research the algorithm for details on costs
	protected void FindPath(Tile target) 
	{
		CalculateAdjacencyLists(target);

		//Get starting location
		GetCurrentTile();

		//Open list contains any tile that HAS NOT been processed
		List<Tile> openList = new List<Tile>();

		//Closed list contains any tile that HAS been processed
		List<Tile> closedList = new List<Tile>();

		openList.Add(currentTile);

		currentTile.hCost = Vector3.Distance(currentTile.transform.position, target.transform.position);
		currentTile.fCost = currentTile.hCost;

		//Process open list and add to closed list
		while(openList.Count > 0) 
		{
			Tile t = FindFMin(openList);

			closedList.Add(t);

			//If we add target tile to closed list, we're done
			//Similar to CheckMouseClick on PlayerMovement.cs
			if(t == target) 
			{
				targetTile = FindEndTile(t);
				MoveToTile(targetTile);
				return;
			}

			//Process all tiles in adjacency list
			foreach(Tile tile in t.adjacencyList) 
			{
				//if already in closed list, do nothing, already been processed
				if(closedList.Contains(tile)) 
				{

				}
				//if in open list, check to see if a better path is available
				else if(openList.Contains(tile)) 
				{
					float gCostTemp = t.gCost + Vector3.Distance(tile.transform.position, t.transform.position);

					//if temp g cost is better than current tile's g cost, choose temp since it's a better path
					if(gCostTemp < tile.gCost) 
					{
						tile.parent = t;
						tile.gCost = gCostTemp;
						tile.fCost = tile.gCost + tile.hCost;
					}
				}
				//otherwise, process tile and add to open list
				else 
				{
					tile.parent = t;

					tile.gCost = t.gCost + Vector3.Distance(tile.transform.position, t.transform.position);
					tile.hCost = Vector3.Distance(tile.transform.position, target.transform.position);
					tile.fCost = tile.gCost + tile.hCost;

					openList.Add(tile);
				}
			}
		}
	}

	public void BeginTurn() 
	{
		turn = true;
	}

	public void EndTurn() 
	{
		hasMoved = false;
		attacking = false;
		turn = false;
	}
}
