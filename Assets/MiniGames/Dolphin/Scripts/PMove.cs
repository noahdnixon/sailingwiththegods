using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMove : MonoBehaviour
{
	public GameObject dolphin;
	public GameObject seaMonster;
	//public GameObject pod;
	public float speed = 100.0f;
	public float turnSpeed = 100.0f;
	int dolphinCount = 0;
	private float move;
	private float turn;

	private void FixedUpdate() 
	{
		move = Input.GetAxis("Vertical") * speed;
		turn = Input.GetAxis("Horizontal") * turnSpeed;

		move *= Time.deltaTime;
		turn *= Time.deltaTime;

		transform.Translate(move, 0, 0);

		transform.Rotate(0, turn, 0);
	}

	public void OnTriggerEnter(Collider other) 
	{
		if (other.tag == "checkpoint") 
		{
			dolphinCount++;
			if (dolphinCount >= 5) 
			{
				//Instantiate(pod, new Vector3(gameObject.transform.position.x + 10, -.5f, gameObject.transform.position.z - 10), Quaternion.identity);
				Time.timeScale = 0;
				Debug.Log("You Win!");
			}
			Destroy(other.gameObject);
			int flip = Random.Range(1,10);
			Debug.Log(flip);
			if (flip > 3) 
			{
				var position = new Vector3(Random.Range(-125.0f, 125.0f), 0, Random.Range(-125.0f, 125.0f));
				Instantiate(dolphin, position, Quaternion.identity);
			}
			else 
			{
				var position = new Vector3(Random.Range(-125.0f, 125.0f), 0, Random.Range(-125.0f, 125.0f));
				var sm = Instantiate(seaMonster, position, Quaternion.identity);
				Destroy(sm, 10);
				position = new Vector3(Random.Range(-125.0f, 125.0f), 0, Random.Range(-125.0f, 125.0f));
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
