using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Behavior {

	[RequireComponent (typeof (Rigidbody2D))]

	public class MoveablePlayerGas : MonoBehaviour, IMoveable {
		public float speed = 1200.0f;

		private int id;
		private Rigidbody2D rb2d;

		void Start() {
			id = GetComponentInParent<Player.Player>().id;
			rb2d = GetComponent<Rigidbody2D>();
		}

		public void move() {
			// Gassing //
			var input = new Vector2(Input.GetAxis(id + "_AXIS_X"), Input.GetAxis(id + "_AXIS_Y"));

			// Dont decrement boost or add force if not gassing 
			if (input.y == 0.0f) {
				return;
			}

			rb2d.AddForce(transform.right * input.y * speed * BoostManager.getBoostModifier(id) * Time.deltaTime);

			if (input.x != 0.0f) {
				transform.rotation *= Quaternion.AngleAxis(-input.x * 3, Vector3.forward);
			}
		}
	}
}