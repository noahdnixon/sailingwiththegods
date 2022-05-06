using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PMove : MonoBehaviour
{
	//creates variables for the dolphin and sea monster prefabs
	public GameObject dolphin;
	public GameObject seaMonster;

	//creates variable for the dolphins found counter
	int dolphinCount = 0;

	//creates variables for the ship's turn and movement speed, as well as the variables for their use
	public float speed = 100.0f;
	public float turnSpeed = 100.0f;
	private float move;
	private float turn;

	//creates and sets the countdown clock to 60 seconds
	public float countdown = 60;

	//creates UI variables for the dolphin count, countdown clock, and win/lose message
	public Text dolphinCountText;
	public Text timeRemaining;
	public Text finalState;

	private void Update() 
	{
		//updates the dolphins found UI if needed
		dolphinCountText.text = "Dolphins Found: " + dolphinCount;

		//if the countdown is not at 0, decrease the countdown and display as normal
		if (countdown > 0) 
		{
			countdown -= Time.deltaTime;
			timeRemaining.text = "Time Remaining: " + countdown;
		}
		//otherwise, freeze gameplay and display loss message
		else if (countdown <= 0) 
		{
			Time.timeScale = 0;
			finalState.text = "You Lost the Dolphins!";
		}

		//if all 5 dolphins have been found, destroy the dolphin, freeze the game, and display win message
		if (dolphinCount >= 5) 
		{
			Time.timeScale = 0;
			finalState.text = "You Successfully Followed the Dolphins!";
		}
	}

	private void FixedUpdate() 
	{
		//moves and turns player according to set variables
		move = Input.GetAxis("Vertical") * speed;
		turn = Input.GetAxis("Horizontal") * turnSpeed;

		move *= Time.deltaTime;
		turn *= Time.deltaTime;

		transform.Translate(move, 0, 0);

		transform.Rotate(0, turn, 0);
	}

	public void OnTriggerEnter(Collider other) 
	{
		//if the player collides with an object, check if it has the tag "checkpoint" (for the dolphins)
		if (other.tag == "checkpoint") 
		{
			//increments dolphins found and destroys the dolphin instance
			dolphinCount++;
			Destroy(other.gameObject);

			//"rolls a die." if the roll is greater than 8, spawn a new dolphin at a random location in the play board.
			//otherwise, spawn a dolphin at a random location as well as a sea monster, which is destroyed after 10 seconds
			int roll = Random.Range(1, 10);
			if (roll > 8) 
			{
				var position = new Vector3(Random.Range(-125.0f, 125.0f), 0, Random.Range(-125.0f, 125.0f));
				Instantiate(dolphin, position, Quaternion.identity);
			}
			else 
			{
				var position = new Vector3(Random.Range(-125.0f, 125.0f), 0, Random.Range(-125.0f, 125.0f));
				Instantiate(dolphin, position, Quaternion.identity);
				var smPosition = new Vector3(position.x + 40, position.y, position.z + 40);
				var sm = Instantiate(seaMonster, smPosition, Quaternion.identity);
				Destroy(sm, 15);
			}
		}
		//otherwise, if the object has tag "monster," freeze time and display loss message
		else if (other.tag == "monster") 
		{
			Time.timeScale = 0;
			finalState.text = "Your Ship Was Taken by the Depths!";
		}
	}
}
