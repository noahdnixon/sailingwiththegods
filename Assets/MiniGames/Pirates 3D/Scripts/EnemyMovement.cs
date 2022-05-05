using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyMovement : GridMovement
{
	//The player controlled character we want to move to
	GameObject target;
	GameObject playerTarget;
	public int health = 10;
	public int damage = 3;

	public Image bar;
	public GameObject healthBar;

	public AudioClip attackSound;
	public AudioClip hitSound;
	public AudioClip deathSound;
	public AudioSource playSounds;

	// Start is called before the first frame update
	void Start()
    {
		isAlive = true;
		healthBar.SetActive(false);
		bar.fillAmount = (float)health / 10;
		Initialize();
    }

    // Update is called once per frame
    void Update()
    {
		//EnemyHealth.text = "Enemy Health: " + health;


		if (health <= 0) 
		{
			isAlive = false;
			GridTurnManager.RemoveUnit(this.gameObject);
			Destroy(this.gameObject);
			//this.gameObject.SetActive(false);

			playSounds.PlayOneShot(deathSound);
		}

		if (isAlive) 
		{
			//Don't allow character to do anything if it isn't their turn
			if (!turn) 
			{
				hasMoved = false;
				attacking = false;
				return;
			}

			if (turn) 
			{
				bar.fillAmount = (float)health / 10;
				healthBar.SetActive(true);
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
			if (hasMoved && !moving && attacking) 
			{

				Debug.Log("Enemy Attack Phase is Starting.");
				ShowAttackRange();
				if (FindAttackTarget()) 
				{
					Debug.Log("Starting Attack.");
					Attack(playerTarget);
				}
				else 
				{
					moving = false;
					attacking = false;
					healthBar.SetActive(false);
					GridTurnManager.EndTurn();
				}

			}
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

	bool FindAttackTarget() 
	{
		foreach (Tile t in attackRangeTiles) 
		{
			RaycastHit hit;
			//Debug.DrawRay(t.transform.position, Vector3.up * 5f, Color.red, 5f);

			if (Physics.Raycast(t.transform.position, Vector3.up, out hit, 1) && hit.collider.tag == "PlayerPiece") 
			{
				Debug.Log("Found player.");
				playerTarget = hit.collider.gameObject;
				return true;
			}
		}
		return false;
	}

	public void Attack(GameObject g) 
	{
		attacking = false;
		hasMoved = false;
		moving = false;

		PlayerMovement p = g.GetComponent<PlayerMovement>();
		p.health = p.health - damage;
		p.bar.fillAmount -= (float)damage / 10;

		playSounds.PlayOneShot(attackSound);
		p.playSounds.PlayOneShot(hitSound);

		Debug.Log("Enemy Attacked!");

		healthBar.SetActive(false);
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
