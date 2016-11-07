using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Boost {
	
	[RequireComponent (typeof (BoxCollider2D))]
	[RequireComponent (typeof (Renderer))]

	public class Pickup : MonoBehaviour {
		private const float MAX_AMT = 35;
		private const int RECHARGE_TIME = 5; // Boostpad will be recharged over this many seconds

		private Renderer r;

		private float amount = 35;

		private Coroutine rechargeCoroutine;

		void Start () {
			r = GetComponent<Renderer>();

			resetPickup();
		}

		// OnStay so you can hang out over the boost pad and continually collect
		void OnTriggerStay2D(Collider2D other) {
			if (getIsActive() && other.tag == "Car") {
				int id = other.gameObject.GetComponentInParent<Player.Player>().id;

				if (BoostManager.canPickupBoost(id)) {
					pickup(id);
				}
			}
		}

		// AI uses this to determine boostpad target
		public bool getIsActive() {
			return amount != 0;
		}

		// After goal
		public void resetPickup() {
			stopRechargeCoroutine();

			amount = MAX_AMT;

			setColor();
		}

		// private 

		// The min/max/range here is used to exagerate the boost regen process
		// The colors will be in a small scale in the middle of the color line
		// Then POP into full boost color when necessary
		private void setColor() {
			float min = 0.28f; // ~70
			float max = 0.68f; // ~170
			float range = max - min; // 0.40f

			float charge = amount / MAX_AMT; // percentage

			float tint = charge * range + min;

			// Jump to 255 on full charge
			if (charge == 1.0f) {
				tint = 1.0f;
			}

			r.material.SetColor("_TintColor", new Color(tint, tint, tint, 0.5f));
		}

		private void stopRechargeCoroutine() {
			if (rechargeCoroutine != null) {
				StopCoroutine(rechargeCoroutine);
				rechargeCoroutine = null;
			}
		}

		private void pickup(int id) {
			// Stop existing coroutine
			stopRechargeCoroutine();

			// Pickup
			BoostManager.incrementBoost(id, amount);
			amount = 0;

			setColor();

			// Start recharge
			rechargeCoroutine = StartCoroutine(recharge());
		}

		private void incrementAmount(float amt) {
			amount += amt;

			if (amount > MAX_AMT) {
				amount = MAX_AMT;
			}

			setColor();
		}

		// A player picks up a boost pad
		// 2 second delay while the boostpad is completely inactive
		// Then over the course of RECHARGE_TIME seconds, the boost regenerates to a maximum of 75% of MAX_AMT
		// After RECHARGE_TIME seconds, the boost pad resets to 100%
		private IEnumerator recharge() {
			yield return new WaitForSeconds(2); // Inactive for 2 seconds after pickup

			float timer = RECHARGE_TIME;

			while (timer > 0.0f) {
				// Leave the pickup off if the game has ended
				if (!GameManager.getHasGameEnded()) {
					// Leave the pickup off while time is paused
					while (TimeManager.getIsPaused()) {
						yield return null; 
					}

					incrementAmount((((float)1/RECHARGE_TIME) * (MAX_AMT * 0.75f)) / 10);
					yield return new WaitForSeconds(0.1f);

					timer -= 0.1f;
				} else { // Game has ended
					stopRechargeCoroutine();
				}
			}

			resetPickup();
		}
	}
}
