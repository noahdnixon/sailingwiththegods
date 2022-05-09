using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DolphinEncounter : RandomEvents.PositiveEvent
{
	//increase weight to have minigame occur more often
	public override float Weight() {
		return 2f;
	}

	public override void Execute() {

		var finalMessage = "You are startled as a pod of dolphins leap up next to your ship. The crew begins to laugh and point as the dolphins dart alongside the bow." + 
			"They must want to play! You decide to take up the chase. Maybe they will lead you somewhere interesting or grant a divine blessing.";

		Notifications.ShowANotificationMessage(finalMessage);

		MiniGames.EnterScene("Dolphin");
	}
}
