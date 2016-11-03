using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Behavior {

	[RequireComponent (typeof (Rigidbody2D))]

	public class BobzMovement : MonoBehaviour, IMoveable {
		

		public int player_id;

		// Inputs
		float In_Steering;
		float In_Throttle;
		float In_EBrake;
		float In_Boost;

		// References
		private Rigidbody2D rb2d;

		void Awake() {
			player_id = GetComponentInParent<Player.Player>().id;
			rb2d = GetComponent<Rigidbody2D>();
		}

		public void move() {

		}
	}
}