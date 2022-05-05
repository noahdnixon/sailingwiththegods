using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : GridMovement
{
	public Button endTurn;
	//public Button retryGame;
	//public Text resultText;
	GameObject enemyTarget;
	public int health = 10;
	public int damage = 3;

	public Image bar;
	public GameObject healthBar;

	public AudioClip attackSound;
	public AudioClip hitSound;
	public AudioClip deathSound;
	public AudioSource playSounds;

	void Start() 
	{
		Initialize();
		endTurn.onClick.AddListener(EndPlayerTurn);
		isAlive = true;

		healthBar.SetActive(false);
		bar.fillAmount = (float)health / 10;

		//resultText.text = "";
		//retryGame.onClick.AddListener(RetryGame);
		//retryGame.gameObject.SetActive(false);
	}

	void Update() 
	{
		//PlayerHealth.text = "Player Health: " + health;
		if (health <= 0) 
		{
			GridTurnManager.RemoveUnit(this.gameObject);
			Destroy(this.gameObject);
			//this.gameObject.SetActive(false);
			isAlive = false;

			playSounds.PlayOneShot(deathSound);

			//resultText.text = "You Lose!!";
			//retryGame.gameObject.SetActive(true);
		}

		if (isAlive) 
		{
			//Don't allow character to do anything if it isn't their turn
			if (!turn) 
			{
				healthBar.SetActive(false);
				endTurn.gameObject.SetActive(false);
				return;
			}
			if (turn) 
			{
				healthBar.SetActive(true);
				bar.fillAmount = (float)health / 10;
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

							healthBar.SetActive(false);
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

							healthBar.SetActive(false);
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

					healthBar.SetActive(false);
					GridTurnManager.EndTurn();

				}
			}
		}
	}

	public void Attack(GameObject g) 
	{
		EnemyMovement e = g.GetComponent<EnemyMovement>();
		e.health -= damage;
		e.bar.fillAmount -= (float)damage / 10;
		playSounds.PlayOneShot(attackSound);
		e.playSounds.PlayOneShot(hitSound);

		attacking = false;
		hasMoved = true;
		moving = false;
		Debug.Log("You Attacked!");
		RemoveAttackRangeTiles();
		endTurn.gameObject.SetActive(false);

		healthBar.SetActive(false);
		GridTurnManager.EndTurn();
	}

	void EndPlayerTurn() 
	{
		RemoveAttackRangeTiles();
		healthBar.SetActive(false);
		attacking = false;
		moving = false;
		endTurn.gameObject.SetActive(false);

		GridTurnManager.EndTurn();
	}
}
