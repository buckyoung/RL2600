using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Behavior {

	[RequireComponent (typeof (Player.Player))]
	[RequireComponent (typeof (Rigidbody2D))]

	public class MoveablePlayer : MonoBehaviour, IMoveable {
		public float speed = 1200.0f;
		public int startingAngle = 0;

		private int id;
		private Rigidbody2D rb2d;

		void Start() {
			id = GetComponent<Player.Player>().id;
			rb2d = GetComponent<Rigidbody2D>();
		}

		/*
		 * User Functions
		 */

		public void move() {
			var movement = new Vector2(Input.GetAxis(id + "_AXIS_X"), Input.GetAxis(id + "_AXIS_Y"));

			// Dont decrement boost or add force if not moving
			if (movement == Vector2.zero) {
				return;
			}

			rb2d.AddForce(movement * speed * BoostManager.getBoostModifier(id) * Time.deltaTime);

			if (movement != Vector2.zero) {
				float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis(angle + startingAngle, Vector3.forward);
			}
		}
	}
}