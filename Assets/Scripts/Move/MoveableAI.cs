using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Behavior {

	[RequireComponent (typeof (Player.Player))]
	[RequireComponent (typeof (Rigidbody2D))]

	public class MoveableAI : MonoBehaviour, IMoveable {
		public float speed = 1200.0f;
		public int startingAngle = 0;

		private int id;
		private Rigidbody2D rb2d;

		private GameObject ball;
		private GameObject pickups;
		private GameObject[] boostPads = new GameObject[6];
		private Renderer[] boostPadRenderers = new Renderer[6];
		private GameObject target;

		private int boostPadIndex = 0;

		private bool isWaiting = false;
		private int boostThreshold = 5;

		private bool justGotBoost = false;

		void Start() {
			id = GetComponent<Player.Player>().id;
			rb2d = GetComponent<Rigidbody2D>();

			ball = GameObject.Find("Ball");

			// Store all boost pads
			pickups = GameObject.Find("PickupContainer");
			int i = 0;

			foreach (Transform child in pickups.transform) {
				if (child != pickups.transform){
					boostPads[i] = child.gameObject;
					boostPadRenderers[i] = child.GetComponent<Renderer>();

					i++;
				}
			}

		}

		/*
		 * User Functions
		 */

		public void move() {
			chooseTarget();

			if (isWaiting) { return; }

			var movement = target.transform.position - transform.position;
		
			// Normalize
			movement = movement.normalized;

			// Maintain current Z
			movement.z = transform.position.z;

			rb2d.AddForce(movement * speed * BoostManager.getBoostModifier(id) * Time.deltaTime);

			if (movement != Vector3.zero) {
				float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis(angle + startingAngle, Vector3.forward);
			}

			// Randomly pause movement
			int num = Random.Range(0, 256);

			if (num < 4) {
				StartCoroutine(startWait());
			}
		}

		// Private 

		private IEnumerator startWait() {
			float num = Random.Range(0.0f, 1.6f);
			isWaiting = true;
			yield return new WaitForSeconds(num);
			isWaiting = false;
		}

		private void chooseTarget() {
			if (BoostManager.getBoost(id) < boostThreshold) {
				// Pick another pad (clockwise -1) if target pad is not enabledds
				if (!boostPadRenderers[boostPadIndex].enabled) {
					boostPadIndex = (boostPadIndex + 1) % 6;
				}

				target = boostPads[boostPadIndex];
				isWaiting = false;
				boostThreshold = Random.Range(5, 80); // Pick a new threshold -- 
				// this is broken here because it constantly picks a new threshold while travelling for boost -- 
				// thats why the car looks indecisive and glitches all around
				// BUT -- I'm leaving it because it creates a more interesting / unpredictable behavior
				justGotBoost = true;
			} else {
				target = ball;

				if (justGotBoost) {
					boostPadIndex = Random.Range(0, 6); // pick a new boost pad
					// This is where the threshold SHOULD be -- only should pick a new threshold after its gotten its fill
					justGotBoost = false;
				}
			}
		}
	}
}