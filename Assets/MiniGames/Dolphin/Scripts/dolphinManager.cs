using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dolphinManager : MonoBehaviour
{
	//creates the variables for the two ship models
	public GameObject shipModels1;
	public GameObject shipModels2;

	//if the player has the upgraded ship, change from the basic to the upgraded - defaults to basic
	private void Start() 
	{
		if (Globals.Game.Session.playerShipVariables.ship.upgradeLevel == 0) 
		{
			shipModels1.SetActive(true);
		}
		else if (Globals.Game.Session.playerShipVariables.ship.upgradeLevel == 1) 
		{
			shipModels2.SetActive(true);
		}
		
	}
	private void Update() 
	{
		//(built for testing) switches between the two ships
		if (Input.GetKeyDown("z")) 
		{
			shipModels1.SetActive(true);
		}
		else if (Input.GetKeyDown("x")) 
		{
			shipModels2.SetActive(true);
		}
	}
}
