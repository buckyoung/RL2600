using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Boost {
	
	[RequireComponent (typeof (BoxCollider2D))]
	[RequireComponent (typeof (Renderer))]

	public class Pickup : MonoBehaviour {
		private Renderer r;

		void Start () {
			r = GetComponent<Renderer>();
		}
		
		void OnTriggerStay2D(Collider2D other) {
			if (r.enabled && other.tag == "Player") {
				int id = other.gameObject.GetComponent<Player.Player>().id;

				if (BoostManager.pickupBoost(id)) {
					StartCoroutine(pickup());
				}
			}
		}

		/*
		 * User functions
		 */

		private IEnumerator pickup() {
			r.enabled = false;
			yield return new WaitForSeconds(5);

			// Leave the pickup off if the game has ended
			if (!GameManager.getHasGameEnded()) {
				r.enabled = true;
			}
		}
	}
}
