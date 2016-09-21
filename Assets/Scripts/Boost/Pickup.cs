using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Boost {
	
	[RequireComponent (typeof (BoxCollider2D))]
	[RequireComponent (typeof (Renderer))]

	public class Pickup : MonoBehaviour {
		private bool isActive = true;

		private Renderer r;
		private Shader activeShader;
		private Shader inactiveShader;

		void Start () {
			r = GetComponent<Renderer>();
			activeShader = Shader.Find("Particles/Alpha Blended");
			inactiveShader = Shader.Find("Particles/Multiply");
		}

		void OnTriggerStay2D(Collider2D other) {
			if (isActive && other.tag == "Car") {
				int id = other.gameObject.GetComponentInParent<Player.Player>().id;

				if (BoostManager.pickupBoost(id)) {
					StartCoroutine(pickup());
				}
			}
		}

		/*
		 * User functions
		 */

		private IEnumerator pickup() {
			isActive = false;
			r.material.shader = inactiveShader;
			yield return new WaitForSeconds(5);

			// Leave the pickup off if the game has ended
			if (!GameManager.getHasGameEnded()) {
				isActive = true;
				r.material.shader = activeShader;
			}
		}
	}
}
