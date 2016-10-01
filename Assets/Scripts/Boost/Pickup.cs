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
			inactiveShader = Shader.Find("Standard");
		}

		void OnTriggerStay2D(Collider2D other) {
			if (isActive && other.tag == "Car") {
				int id = other.gameObject.GetComponentInParent<Player.Player>().id;

				if (BoostManager.pickupBoost(id)) {
					StartCoroutine(pickup());
				}
			}
		}

		public bool getIsActive() {
			return isActive;
		}

		public void setIsActive(bool value) {
			isActive = value;

			if (value) {
				r.material.shader = activeShader;
			}
		}

		// private 

		private IEnumerator pickup() {
			isActive = false;
			r.material.shader = inactiveShader;
			yield return new WaitForSeconds(5);

			// Leave the pickup off if the game has ended
			if (!GameManager.getHasGameEnded()) {
				// Leave the pickup off while time is paused
				while (TimeManager.getIsPaused()) {
					// NOTE: This is an imperfect solution. A player could
					// 1) pick up a boost pad at time T and immediately pause the game,
					// 2) wait until time T+d where d is the delay in time while a 
					// boost pad replenishes and 3) unpause the game... in this case,
					// the boost will _immediately_ replenish in the eyes 
					// of the car
					yield return null; 
				}

				isActive = true;
				r.material.shader = activeShader;
			}
		}
	}
}
