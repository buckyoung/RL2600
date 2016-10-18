using UnityEngine;
using System.Collections;
using RL2600.System;
using RL2600.Player;

namespace RL2600.Score {
	public class Goal : MonoBehaviour {
		CircleCollider2D cc2d;
		PointEffector2D pe2d;

		public Team team = Team.BLUE;

		void Start() {
			// Support explosion after goal
			cc2d = GetComponent<CircleCollider2D>();
			cc2d.enabled = false;

			pe2d= GetComponent<PointEffector2D>();
			pe2d.enabled = false;
		}

		void OnCollisionEnter2D(Collision2D col) {
			if (col.gameObject.tag == "Ball") {
// 				TODO: create explosion after goal
//				cc2d.enabled = true; 
//				pe2d.enabled = true;

				GameManager.score(team);

//				StartCoroutine(disableExplosion()); // TODO Disable explosion force after some time
			}
		}

		private IEnumerator disableExplosion() {
			yield return new WaitForSeconds(1);

			cc2d.enabled = false;
			pe2d.enabled = false;
		}
	}
}
