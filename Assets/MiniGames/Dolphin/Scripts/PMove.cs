using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMove : MonoBehaviour
{
	public GameObject dolphin;
	public GameObject seaMonster;
	public float speed = .5f;
	int dolphinCount = 0;

	private void Update() 
	{
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		Vector3 moveDirection = new Vector3(x, 0, y);

		transform.position += (moveDirection * speed);
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
				var position = new Vector3(Random.Range(-20.0f, 20.0f), 0, Random.Range(-20.0f, 20.0f));
				Instantiate(dolphin, position, Quaternion.identity);
			}
			else 
			{
				var position = new Vector3(Random.Range(-20.0f, 20.0f), 0, Random.Range(-20.0f, 20.0f));
				var sm = Instantiate(seaMonster, position, Quaternion.identity);
				Destroy(sm, 3);
				position = new Vector3(Random.Range(-20.0f, 20.0f), 0, Random.Range(-20.0f, 20.0f));
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
