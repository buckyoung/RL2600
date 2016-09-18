using UnityEngine;
using System.Collections;
using RL2600.System;
using RL2600.Player;

namespace RL2600.Car {
	public class Car : MonoBehaviour {
		private SpriteRenderer spriteRenderer;
		private int id;

		void Start () {
			spriteRenderer = GetComponent<SpriteRenderer>();
			id = GetComponentInParent<Player.Player>().id;

			switch (PlayerManager.getPlayerTeam(id)) {
			case Team.BLUE:
				spriteRenderer.color = new Color(0.125f, 0.64f, 1.0f);
				break;
			case Team.RED:
				spriteRenderer.color = Color.red;
				break;
			}
		}
	}
}