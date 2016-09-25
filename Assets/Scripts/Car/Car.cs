using UnityEngine;
using System.Collections;
using RL2600.System;
using RL2600.Player;
using RL2600.Behavior;

namespace RL2600.Car {
	public class Car : MonoBehaviour {
		private SpriteRenderer spriteRenderer;
		private int id;

		void Start () {
			spriteRenderer = GetComponent<SpriteRenderer>();
			id = GetComponentInParent<Player.Player>().id;

			// Set car color based on team
			if (PlayerManager.getPlayerTeam(id) == Team.BLUE) {
				spriteRenderer.color = new Color(0.125f, 0.64f, 1.0f);
			} else {
				spriteRenderer.color = Color.red;
			}

			// Set movement script based on isAI
			if (PlayerManager.getIsAI(id)) {
				gameObject.AddComponent<Behavior.MoveableAI>();
			} else {
				gameObject.AddComponent<Behavior.MoveablePlayer>();
			}
		}
	}
}