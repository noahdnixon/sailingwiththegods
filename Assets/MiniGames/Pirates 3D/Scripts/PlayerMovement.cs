using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : GridMovement
{
	//The player controlled character we want to move and attack with

	//These are the basic components of the player class. These include the target enemy that the player wants to attack,
	//their health and damage, and the various icons that go with them.

	GameObject enemyTarget;
	public int health = 10;
	public int damage = 3;
	public Image bar;
	public GameObject icon;
	public GameObject healthBar;

	//This is the end turn button, which goes away when it is the enemy's turn
	public Button endTurn;

	//These are the audio clips of the player, which are also present on the enemy as well
	public AudioClip attackSound;
	public AudioClip hitSound;
	public AudioClip deathSound;
	public AudioSource playSounds;

	//On start, the boolean 'isAlive' is set to true, and the Health Bar is filled.
	//The function 'Initialize' is also called to activate.
	void Start() 
	{
		Initialize();
		endTurn.onClick.AddListener(EndPlayerTurn);
		isAlive = true;
		bar.fillAmount = (float)health / 10;
	}

	void Update() 
	{
		//This activates when the unit is at or below 0 health, signifying a death.
		if (health <= 0) 
		{
			isAlive = false;
			icon.SetActive(false);
			healthBar.SetActive(false);
			transform.rotation = Quaternion.Euler(0, 0, 90);
			this.gameObject.tag = "Untagged";

			//Set just in case the player gets a turn while dead
			if (turn) 
			{
				GridTurnManager.EndTurn();
			}
		}
		//These are the functions that the player goes through while alive.
		if (isAlive) 
		{
			//Don't allow character to do anything if it isn't their turn
			if (!turn) 
			{
				return;
			}

			//When it is the enemy's turn, the state of their health bar is updated again, and it enables the end turn button.
			if (turn) 
			{
				bar.fillAmount = (float)health / 10;
				endTurn.gameObject.SetActive(true);

				//Set just in case the player gets a turn while dead
				if (health <= 0) 
				{
					GridTurnManager.EndTurn();
				}
			}
			//if moving disabled finding adjacent tiles or clicking tile to walk to
			if (!moving && !hasMoved) 
			{
				FindSelectableTiles();
				CheckMouseClick();
			}
			else 
			{
				//Move function is called when ready to move.
				Move();
			}
			if (hasMoved && !moving && attacking) 
			{
				//The attack phase begins once movement ends
				ShowAttackRange();
				CheckMouseClick();
			}
		}
	}

	//Check if player has clicked selectable tile
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

						//If the tile is the one containing the player piece, end the turn. This is also applicable in movement.
						if (Physics.Raycast(hit.transform.position, Vector3.up, out hit, 1) && hit.collider.tag == "PlayerPiece") 
						{
							GridTurnManager.EndTurn();
						}
					}
					if(t.attackRange) 
					{
						//If the tile belongs to the enemy piece, attack the enemy.
						if(Physics.Raycast(hit.transform.position, Vector3.up, out hit, 1) && hit.collider.tag == "EnemyPiece") 
						{
							Attack(hit.collider.gameObject);
						}
						//Else end the turn if clicking on the plyer.
						if(Physics.Raycast(hit.transform.position, Vector3.up, out hit, 1) && hit.collider.tag == "PlayerPiece") 
						{
							GridTurnManager.EndTurn();
						}
					}
				}

				//If the enemy get clicked instead of the tile
				else if(hit.collider.tag == "EnemyPiece") 
				{
					enemyTarget = hit.collider.gameObject;

					//If the enemy is in range, then attack. Else do nothing
					if(Physics.Raycast(hit.transform.position, Vector3.down, out hit, 1) && hit.collider.tag == "Tile") 
					{
						Tile t = hit.collider.GetComponent<Tile>();

						if(t.attackRange) 
						{
							Attack(enemyTarget);
						}
					}
				}
				//End the turn if you click on the current player.
				else if (hit.collider.gameObject == this.gameObject) 
				{
					GridTurnManager.EndTurn();
				}
			}
		}
	}

	//The attack function that the player uses
	public void Attack(GameObject g) 
	{
		EnemyMovement e = g.GetComponent<EnemyMovement>();
		e.health -= damage;
		e.bar.fillAmount -= (float)damage / 10;
		playSounds.PlayOneShot(attackSound);

		if (e.health <= 0) 
		{
			e.playSounds.PlayOneShot(deathSound);
		}
		else 
		{
			e.playSounds.PlayOneShot(hitSound);
		}

		attacking = false;
		hasMoved = true;
		moving = false;
		Debug.Log("You Attacked!");
		RemoveAttackRangeTiles();
		endTurn.gameObject.SetActive(false);

		GridTurnManager.EndTurn();
	}

	//An end turn script that essentially does what 'GridMovement.EndTurn' does.
	void EndPlayerTurn() 
	{
		RemoveAttackRangeTiles();
		attacking = false;
		moving = false;
		endTurn.gameObject.SetActive(false);

		GridTurnManager.EndTurn();
	}
}
