using UnityEngine;
using System.Collections;
using RL2600.System;
using RL2600.Player;

namespace RL2600.Score {
	public class Goal : MonoBehaviour {
		public Team team = Team.BLUE;

		void OnCollisionEnter2D(Collision2D col) {
			if (col.gameObject.tag == "Ball") {
				GameManager.score(team);
			}
		}
	}
}
