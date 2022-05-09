using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewPirateAttack : RandomEvents.NegativeEvent
{

	//increased weight to have minigame occur more often
	public override float Weight() {
		return 10f;
	}

	//checks to see if the player is within one of the pirate zones
	//if player is not in a pirate zone, the minigame will not occur during gameplay
	public override bool isValid() {
		if (Session.playerShipVariables.zonesList.Count > 0) { return base.isValid(); }
		else { return false; }
	}

	public override void Execute() {

		var finalMessage = "Pirate Attack!";

		Notifications.ShowANotificationMessage(finalMessage);

		MiniGames.EnterScene("Pirates3D");
	}
}
