using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyMovement : GridMovement
{
	//The computer controlled character we want to attack

	//These are the basic components of the enemy class. These include the target of movement, the target player that the enemy wants to move to,
	//their health and damage, and the various icons that go with them.
	GameObject target;
	GameObject playerTarget;
	public int health = 10;
	public int damage = 3;
	public Image bar;
	public GameObject icon;
	public GameObject healthBar;

	//This is the end turn button, which goes away when it is the enemy's turn
	public GameObject endTurn;

	//These are the audio clips of the enemy, which are also present on the player as well
	public AudioClip attackSound;
	public AudioClip hitSound;
	public AudioClip deathSound;
	public AudioSource playSounds;

	//On start, the boolean 'isAlive' is set to true, and the Health Bar is filled.
	//The function 'Initialize' is also called to activate.

	void Start()
    {
		isAlive = true;
		bar.fillAmount = (float)health / 10;
		Initialize();
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

			//Set just in case the enemy gets a turn while dead
			if (turn) 
			{
				GridTurnManager.EndTurn();
			}
		}

		//These are the functions that the enemy goes through while alive.
		if (isAlive) 
		{
			//Don't allow enemy to do anything if it isn't their turn
			if (!turn) 
			{
				hasMoved = false;
				attacking = false;
				return;
			}
			//When it is the enemy's turn, the state of their health bar is updated again, and it disables the end turn button
			if (turn) 
			{
				bar.fillAmount = (float)health / 10;
				endTurn.gameObject.SetActive(false);

				//Set just in case the enemy gets a turn while dead
				if (health <= 0) 
				{
					GridTurnManager.EndTurn();
				}
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
				//Move function is called when ready to move.
				Move();
			}
			if (hasMoved && !moving && attacking) 
			{
				//Once the enemy stops moving, thr attack phase begins.
				Debug.Log("Enemy Attack Phase is Starting.");
				ShowAttackRange();

				//If the player is in range, then attack. Otherwise, end the turn.
				if (FindAttackTarget()) 
				{
					Debug.Log("Starting Attack.");
					Attack(playerTarget);
				}
				else 
				{
					moving = false;
					attacking = false;
					GridTurnManager.EndTurn();
				}

			}

			//Deletes the highlighted tiles of the attack range before ending turn.
			if (hasMoved && !moving && !attacking) 
			{
				RemoveAttackRangeTiles();
			}
		}
	}

	//Find where enemy is going to move to
	void CalculatePath() 
	{
		Tile targetTile = GetTargetTile(target);
		FindPath(targetTile);
	}

	//Finds where the enemy will attack, and attacks whatever the variable 'playerTarget' is set as.
	bool FindAttackTarget() 
	{
		foreach (Tile t in attackRangeTiles) 
		{
			RaycastHit hit;
			if (Physics.Raycast(t.transform.position, Vector3.up, out hit, 1) && hit.collider.tag == "PlayerPiece") 
			{
				Debug.Log("Found player.");
				playerTarget = hit.collider.gameObject;
				return true;
			}
		}
		return false;
	}

	//The attack function for the enemy.
	public void Attack(GameObject g) 
	{
		attacking = false;
		hasMoved = false;
		moving = false;

		PlayerMovement p = g.GetComponent<PlayerMovement>();
		p.health = p.health - damage;
		p.bar.fillAmount -= (float)damage / 10;

		playSounds.PlayOneShot(attackSound);
		if (p.health <= 0) 
		{
			p.playSounds.PlayOneShot(deathSound);
		}
		else 
		{
			p.playSounds.PlayOneShot(hitSound);
		}

		Debug.Log("Enemy Attacked!");

		GridTurnManager.EndTurn();
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
