using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Score {
	public class Goal : MonoBehaviour {
		public int id = 1;

		void OnCollisionEnter2D(Collision2D col) {
			if (col.gameObject.tag == "Ball") {
				ScoreManager.score(id);
			}
		}
	}
}
