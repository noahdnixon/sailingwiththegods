using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : GridMovement
{
	public Button endTurn;
	//public Button retryGame;
	//public Text PlayerHealth;
	//public Text resultText;
	GameObject enemyTarget;
	public int health = 10;
	public int damage = 3;

	void Start() 
	{
		Initialize();
		endTurn.onClick.AddListener(EndPlayerTurn);
		isAlive = true;

		//resultText.text = "";
		//retryGame.onClick.AddListener(RetryGame);
		//retryGame.gameObject.SetActive(false);
	}

	void Update() 
	{
		//PlayerHealth.text = "Player Health: " + health;

		if (health <= 0) 
		{
			gameObject.SetActive(false);
			isAlive = false;
			GridTurnManager.RemoveUnit();
			//resultText.text = "You Lose!!";
			//retryGame.gameObject.SetActive(true);
		}

		if (isAlive) 
		{
			//Don't allow character to do anything if it isn't their turn
			if (!turn) 
			{
				return;
			}
			if (turn) 
			{
				endTurn.gameObject.SetActive(true);
			}
			//if moving disabled finding adjacent tiles or clicking tile to walk to
			if (!moving && !hasMoved) 
			{
				FindSelectableTiles();
				CheckMouseClick();
			}
			else 
			{
				Move();
			}
			if (hasMoved && !moving && attacking) 
			{
				ShowAttackRange();
				CheckMouseClick();
			}
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

						if (Physics.Raycast(hit.transform.position, Vector3.up, out hit, 1) && hit.collider.tag == "PlayerPiece") 
						{
							EndTurn();
							GridTurnManager.EndTurn();
						}
					}
					if(t.attackRange) 
					{
						if(Physics.Raycast(hit.transform.position, Vector3.up, out hit, 1) && hit.collider.tag == "EnemyPiece") 
						{
							Attack(hit.collider.gameObject);
						}
						if(Physics.Raycast(hit.transform.position, Vector3.up, out hit, 1) && hit.collider.tag == "PlayerPiece") 
						{
							EndTurn();
							GridTurnManager.EndTurn();
						}
					}
				}

				else if(hit.collider.tag == "EnemyPiece") 
				{
					enemyTarget = hit.collider.gameObject;

					if(Physics.Raycast(hit.transform.position, Vector3.down, out hit, 1) && hit.collider.tag == "Tile") 
					{
						Tile t = hit.collider.GetComponent<Tile>();

						if(t.attackRange) 
						{
							Attack(enemyTarget);
						}
					}
				}
				else if (hit.collider.gameObject == this.gameObject) 
				{
					EndTurn();
					GridTurnManager.EndTurn();

				}
			}
		}
	}

	public void Attack(GameObject g) 
	{
		EnemyMovement e = g.GetComponent<EnemyMovement>();
		e.health -= damage;

		attacking = false;
		hasMoved = true;
		moving = false;
		Debug.Log("You Attacked!");
		RemoveAttackRangeTiles();
		endTurn.gameObject.SetActive(false);

		EndTurn();
		GridTurnManager.EndTurn();
	}

	void EndPlayerTurn() 
	{
		RemoveAttackRangeTiles();
		hasMoved = true;
		attacking = false;
		moving = false;
		endTurn.gameObject.SetActive(false);

		EndTurn();
		GridTurnManager.EndTurn();
	}
}
