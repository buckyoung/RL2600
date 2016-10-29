using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Boost {
	
	[RequireComponent (typeof (BoxCollider2D))]
	[RequireComponent (typeof (Renderer))]

	public class Pickup : MonoBehaviour {
		private const float MAX_AMT = 35;
		private const int RECHARGE_TIME = 5; // boostpad will be recharged over this many seconds

		private Renderer r;

		private bool isInactive = false; 
		private float amount = 35;

		private Coroutine rechargeCoroutine;

		void Start () {
			r = GetComponent<Renderer>();

//			r.material.SetColor("_TintColor", Color); // TODO 
		}

		// OnStay so you can hang out over the boost pad and continually collect
		void OnTriggerStay2D(Collider2D other) {
			if (getIsActive() && other.tag == "Car") {
				int id = other.gameObject.GetComponentInParent<Player.Player>().id;

				if (BoostManager.canPickupBoost(id)) {
					// Stop any existing coroutines 
					if (rechargeCoroutine != null) {
						StopCoroutine(rechargeCoroutine);
					}

					// Pickup the boost
					pickup(id);

					// Begin recharging boost pad
					rechargeCoroutine = StartCoroutine(recharge());
				}
			}
		}

		// AI uses this to determine boostpad target
		public bool getIsActive() {
			return !isInactive;
		}

		// After goal
		public void resetPickup() {
			setIsInactive(false);
			amount = MAX_AMT;
		}

		// private 

		private void setIsInactive(bool value) {
			isInactive = value;
			Debug.Log(isInactive);
		}

		private void pickup(int id) {
			BoostManager.incrementBoost(id, amount);
			amount = 0;
			setIsInactive(true);
			rechargeCoroutine = null;
		}

		private IEnumerator recharge() {
			Debug.Log("Recharging");
			yield return new WaitForSeconds(2);

			int timer = RECHARGE_TIME;

			timer--;

			while (timer > 0) {
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
						// 10/29 NOTE THIS MIGHT NOT BE TRUE ANYMORE, with the new recharge system
						yield return null; 
					}

					amount += ((float)1/RECHARGE_TIME) * MAX_AMT;

					setIsInactive(false);

					Debug.Log(amount);

					if (amount > MAX_AMT) {
						amount = MAX_AMT;
					}

					yield return new WaitForSeconds(1);

					timer--;
				} else {
					// Game has ended
					StopCoroutine(rechargeCoroutine);
				}
			}

			resetPickup();
		}
	}
}
