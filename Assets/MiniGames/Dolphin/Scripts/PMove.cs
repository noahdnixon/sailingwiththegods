using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMove : MonoBehaviour
{
	public GameObject dolphin;
	public GameObject seaMonster;
	public float speed = 100.0f;
	public float turnSpeed = 100.0f;
	int dolphinCount = 0;

	private void FixedUpdate() 
	{
		float move = Input.GetAxis("Vertical") * speed;
		float turn = Input.GetAxis("Horizontal") * turnSpeed;

		move *= Time.deltaTime;
		turn *= Time.deltaTime;

		transform.Translate(0, 0, move);

		transform.Rotate(0, turn, 0);
	}

	public void OnTriggerEnter(Collider other) 
	{
		if (other.tag == "checkpoint") 
		{
			dolphinCount++;
			if (dolphinCount >= 10) 
			{
				Destroy(gameObject);
				Debug.Log("You Win!");
			}
			Destroy(other.gameObject);
			int flip = Random.Range(1,10);
			Debug.Log(flip);
			if (flip > 3) 
			{
				var position = new Vector3(Random.Range(-250.0f, 250.0f), 0, Random.Range(-250.0f, 250.0f));
				Instantiate(dolphin, position, Quaternion.identity);
			}
			else 
			{
				var position = new Vector3(Random.Range(-250.0f, 250.0f), 0, Random.Range(-250.0f, 250.0f));
				var sm = Instantiate(seaMonster, position, Quaternion.identity);
				Destroy(sm, 3);
				position = new Vector3(Random.Range(-250.0f, 250.0f), 0, Random.Range(-250.0f, 250.0f));
				Instantiate(dolphin, position, Quaternion.identity);
			}
		}
		else if (other.tag == "monster") 
		{
			Destroy(gameObject);
			Debug.Log("You Lose!");
		}
	}
}
